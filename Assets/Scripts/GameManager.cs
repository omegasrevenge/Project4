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
    public const string ServerURL = "http://172.21.66.4:7774/rpc/";
    private const float OwnUpdateFreq = 60*3;
    private const float PositionUpdateFreq = 60*1;
    private const float PlayerQueryFreq = 60*2;

	private static GameManager _instance;
	
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


	private void Start()
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
            if (Time.time >= PositionUpdateFreq + _lastPositionUpdate)
                SendPosition();
            //Request players around you
            if (Time.time >= PlayerQueryFreq + _lastPlayerQuery)
                SearchForPlayers();
        }

        //Load queued players
        if (!_playerQueryActive && _playerQueue.Count > 0)
            StartCoroutine(CGetPlayers());

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
	public void Login(string playerID, string password)
	{
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
        Vector2 pos = LocationManager.Singleton.GetCurrentPosition();
        WWW request = new WWW(GetSessionURL("setpos")+"&lon="+pos.x+"&lat="+pos.y);

        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json)) yield break;
        Player.Position = pos;
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

		Login(playerid,password);
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



}
