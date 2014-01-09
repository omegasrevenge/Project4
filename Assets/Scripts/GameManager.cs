using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameMode { Login, Map, Fight, Base };
    public struct ObjectPos
    {
        public string ID;
        public float Lon;
        public float Lat;

        public ObjectPos(string id, float lon, float lat)
        {
            ID = id;
            Lon = lon;
            Lat = lat;
        }
    }
    public string ServerURL = "http://pixeltamer.net:7774/rpc/";
    private const float OwnUpdateFreq = 60*3;
    private const float PositionUpdateFreq = 60*1;
    private const float PositionUpdateFreqMove = 5;
    private const float PlayerQueryFreq = 60*2;

	private static GameManager _instance;

	public POI[] POIs = new POI[0];
	public int pois_version = 0;
	public float pois_timeQ;
	public bool pois_valid = false;
	public string lastFarmResult = "";

    [SerializeField]
	public Player Player = new Player();
	public string SessionID = "";
    public GameMode CurrentGameMode = GameMode.Login;

    public ObjectPos[] PlayersOnMap;
    private Dictionary<string, Player> _playerCache = new Dictionary<string, Player>();
    private List<string> _playerQueue = new List<string>();
    private bool _playerQueryActive;
        
    [SerializeField]
    private float _lastOwnPlayerUpdate;
    [SerializeField]
    private float _lastPositionUpdate = -1000;
    private Vector2 _lastPosition = new Vector2();
    [SerializeField]
    private float _lastPlayerQuery = -1000;

    public bool LoggedIn { get { return SessionID.Length != 0; } }

    public static GameManager Singleton
    {
        get 
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("GameManager");
                _instance = obj.AddComponent<GameManager>();
            }
            return _instance; 
        }
    }


	private void Awake()
	{
        //Check for Singleton
		if (_instance == null)
			_instance = this;
		else if (_instance != this)
			Debug.LogError("Second instance of GameManager.");
	}

    private void Update()
    {
        if (!LoggedIn) return;

        //Update own player information
        if (Time.time >= OwnUpdateFreq + _lastOwnPlayerUpdate)
            GetOwnPlayer();

        //MAP MODE:
        if (CurrentGameMode == GameMode.Map)
        {
            //Send player position to server
            float updatefrequency = PositionUpdateFreq;
            if (MapUtils.DistanceInKm(_lastPosition, LocationManager.GetCurrentPosition()) > 0.025)
                updatefrequency = PositionUpdateFreqMove;
            if (Time.time >= updatefrequency + _lastPositionUpdate)
                SendPosition();
            //Request players around you
            if (Time.time >= PlayerQueryFreq + _lastPlayerQuery)
                SearchForPlayers();
        }

        //Load queued players
        if (!_playerQueryActive && _playerQueue.Count > 0)
            StartCoroutine(CGetPlayers());


		if (!pois_valid && pois_timeQ <= 0)
			StartCoroutine(GetPois());

		pois_timeQ -= Time.deltaTime;
    }


    //DEBUG
    public void Test()
    {
        foreach (ObjectPos objectPos in PlayersOnMap)
        {
            Player player = GetPlayer(objectPos.ID);
            if(player != null)
                Debug.Log(player.Name);

        }
    }

    /// <summary>
    /// Returns a valid URL for the current session to use RPC functions.
    /// </summary>
    /// <param name="function"></param>
    /// <returns></returns>
    public string GetSessionURL(string function)
    {
        return ServerURL + function + "?pid=" + Player.PlayerID + "&sid=" + SessionID;
    }


    public void Login(string token)
    {
        StartCoroutine(CLogin(token));
    }

    /// <summary>
    /// Gets SessionID by PlayerID (Get from OAuth).
    /// </summary>
    /// <param name="playerID"></param>
	public void Login(string playerID, string password, bool local)
	{
        if (local) ServerURL = "http://localhost:7774/rpc/";
        StartCoroutine(CLogin(playerID, password));
	}

    private IEnumerator CLogin(string token)
    {
        WWW request = new WWW(ServerURL + "login_google?token=" + token);
        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json)) yield break;
        SessionID = (string)json["data"]["sid"];
        Player.PlayerID = (string)json["data"]["pid"]; 
        GetOwnPlayer();
    }

	private IEnumerator CLogin(string playerID, string password)
	{
	    string url = ServerURL + "login_password?pid=" + playerID + "&pass=" + password;
        Debug.Log("URL: "+url);
        WWW request = new WWW(url);

		yield return request;

		JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json)) yield break;
		SessionID = (string)json["data"];
		Player.PlayerID = playerID;
        GetOwnPlayer();

	}

    /// <summary>
    /// Loads own Player Data (First login with playerID).
    /// </summary>
	public void GetOwnPlayer()
	{
        if (!LoggedIn) return;
		_lastOwnPlayerUpdate = Time.time;
		StartCoroutine(CGetOwnPlayer());
	}

	private IEnumerator CGetOwnPlayer()
	{
		WWW request = new WWW(GetSessionURL("pinfo"));

		yield return request;

		JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json)) yield break;
		Player.ReadJson(json["data"]);
	    if (CurrentGameMode == GameMode.Login)
	        CurrentGameMode = GameMode.Map;
	}

    /// <summary>
    /// Updates the player's position on the server.
    /// </summary>
    public void SendPosition()
    {
        if (!LoggedIn) return;
		_lastPositionUpdate = Time.time;
		StartCoroutine(CSendPosition());
    }

    private IEnumerator CSendPosition()
    {
        Vector2 pos = LocationManager.GetCurrentPosition();
        _lastPosition = pos;
        WWW request = new WWW(GetSessionURL("setpos")+"&lon="+pos.x+"&lat="+pos.y);

        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json)) yield break;
        Player.Position = pos;
    }

	public void SendBasePosition()
	{
		if (!LoggedIn) return;
		StartCoroutine(CSendBasePosition());
	}

	private IEnumerator CSendBasePosition()
	{
		Vector2 pos = LocationManager.GetCurrentPosition();
		WWW request = new WWW(GetSessionURL("setbasepos") + "&lon=" + pos.x + "&lat=" + pos.y);

		yield return request;

		JSONObject json = JSONParser.parse(request.text);
		if (!CheckResult(json)) yield break;
		Player.BasePosition = pos;
	}

	public void PoiFarm(POI poi)
	{
		if (!LoggedIn) return;
		StartCoroutine(CPoiFarm(poi));
	}

	private IEnumerator CPoiFarm(POI poi)
	{
		WWW request = new WWW(GetSessionURL("poifarm") + "&mappos=" + poi.MapPos() + "&poiid=" + poi.POI_ID);
		Debug.Log(request.url);
		yield return request;

		JSONObject json = JSONParser.parse(request.text);
		if (CheckResult(json)) _lastOwnPlayerUpdate = -1000;
		lastFarmResult = (string) json["data"];
	}

    /// <summary>
    /// Requests all playerIDs around your position
    /// </summary>
    public void SearchForPlayers()
    {
        if (!LoggedIn) return;
		_lastPlayerQuery = Time.time;
		StartCoroutine(CSearchForPlayers());
    }

    private IEnumerator CSearchForPlayers()
    {
        WWW request = new WWW(GetSessionURL("pquery"));

        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json)){ yield break;}
        JSONObject playersJSON = json["data"];
        PlayersOnMap = new ObjectPos[playersJSON.Count];
        for (int i = 0; i < PlayersOnMap.Length; i++)
        {
            PlayersOnMap[i] = new ObjectPos((string)playersJSON[i][2], (float)playersJSON[i][1], (float)playersJSON[i][0]);
        }
    }

    /// <summary>
    /// Loads player data by passing the playerID.
    /// </summary>
    /// <param name="playerID"></param>
    /// <returns></returns>
    public Player GetPlayer(string playerID)
    {
        if (!LoggedIn) return null;
        Player player;
        if (_playerCache.TryGetValue(playerID, out player))
            return player;
        if (!_playerQueue.Contains(playerID))
            _playerQueue.Add(playerID);
        return null;
    }

    private IEnumerator CGetPlayers()
    {
        _playerQueryActive = true;
        string[] requestedPIDs = _playerQueue.ToArray();
        Debug.Log("Get Players: "+string.Join(",", requestedPIDs));
        WWW request = new WWW(GetSessionURL("pinfo") + "&pinfo=" + string.Join(",", requestedPIDs));

        yield return request;
        _playerQueryActive = false;
        JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json)) {yield break;}
        JSONObject playersJSON = json["data"];
        for (int i = 0; i < playersJSON.Count; i++)
        {
            if (!(bool) playersJSON[i])
            {
                _playerCache[requestedPIDs[i]] = null;
                continue;
            }
            Player player = new Player();
            player.ReadJson(playersJSON[i]);
            _playerCache[requestedPIDs[i]] = player;
            _playerQueue.Remove(requestedPIDs[i]);
        }
        

    }

	public void AddXP( string xp)
	{
		StartCoroutine(CAddXP(xp));
	}

	public IEnumerator CAddXP( string xp)
	{
		
		WWW request = new WWW(GetSessionURL("addxp") + "&xp="+ xp);
		
		yield return request;
		
		JSONObject json = JSONParser.parse(request.text);
		if (!CheckResult(json)){ yield break;}

		GetOwnPlayer();
		
	}

	public void Attack(string enemy )
	{
		StartCoroutine(CAttack(enemy));
	}
	
	public IEnumerator CAttack(string enemy)
	{
		
		WWW request = new WWW(GetSessionURL("attack") + "&enemy="+enemy);
		
		yield return request;
		
		JSONObject json = JSONParser.parse(request.text);
		if (!CheckResult(json)){ yield break;}
		
		GetOwnPlayer();
		
	}

	public void CreatePlayer(string playerid,string playername,string password)
	{
		StartCoroutine(CCreatePlayer(playerid, playername, password));
	}

    public IEnumerator CCreatePlayer(string playerid,string playername,string password)
    {
			WWW request = new WWW(ServerURL+"createplayer"+ "?pid="+playerid + "&name="+playername + "&pass="+password);

			yield return request;
			
			JSONObject json = JSONParser.parse(request.text);
			if (!CheckResult(json)){ yield break;}

		Login(playerid,password, false);
	}

	private IEnumerator GetPois()
	{
		pois_timeQ = 3;
		pois_valid = true;
		Vector2 pos = LocationManager.GetCurrentPosition();
		WWW request = new WWW(GameManager.Singleton.GetSessionURL("getpois") + "&lon=" + pos.x + "&lat=" + pos.y);

		yield return request;

		JSONObject json = JSONParser.parse(request.text);
		if (!CheckResult(json)) yield break;
		JSONObject data = json["data"];
		JSONObject pois = data["POIs"];
		POI[] tmpPOIs = new POI[pois.Count];
		Debug.Log(pois.Count);
		for (int i = 0; i < tmpPOIs.Length; i++)
		{
			tmpPOIs[i] = new POI();
			tmpPOIs[i].ReadJson(pois[i]);
		}

		for (int i = 0; i < POIs.Length; i++)
		{
			bool found = false;
			for (int j = 0; j < tmpPOIs.Length; j++)
			{
				if (POIs[i].POI_ID == tmpPOIs[j].POI_ID)
				{
					tmpPOIs[j] = POIs[i];
					found = true;
					break;
				}
			}
			if (!found && POIs[i].instance != null)
			{
				//Debug.Log("kaputt");
				Destroy(POIs[i].instance);
			}
		}

		POIs = tmpPOIs;
		pois_version++;
		//CreatePOIs();
	}

    /// <summary>
    /// Checks if the request was successful.
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public bool CheckResult(JSONObject json)
    {
        if (!(bool)json["result"])
        {
            Debug.LogError("RPC Fail: " + json["error"]);
            if ((string) json["error"] == "invalid_session")
            {
                SessionID = "";
                CurrentGameMode = GameMode.Login;
            }
            return false;
        }
        return true;
    }

	private void OnApplicationPause(bool paused)
	{
		if (paused)
		{
			Debug.Log("Pause App.");
			Input.location.Stop();
			Input.compass.enabled = false;
		}
		else
		{
			Debug.Log("Resume App.");
			Input.location.Start();
			Input.compass.enabled = true;
		}
	}

    void OnApplicationFocus(bool focusStatus)
    {
        
        if (focusStatus)
        {
            Debug.Log("FOCUS CHANGED => HIDE NAVIGATION BAR!");
#if !UNITY_EDITOR
            (new AndroidJavaClass("com.nerdiacs.nerdgpgplugin.NerdGPG")).CallStatic("HideNavigationBar");
#endif
        }
    }
}
