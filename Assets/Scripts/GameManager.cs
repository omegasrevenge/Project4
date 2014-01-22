using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameMode { Login, Map, Fight, Base };
    public enum ExchangeMode { Up, Down, Cricle };

    public static string DontSaveTag = "DontSave";

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

    private static GameManager _instance;
    private ViewController _view;
    private MapGrid _map;

    public bool DummyUI = true;

    private const string Server = "http://pixeltamer.net:7774/rpc/";
    private const string Localhost = "http://localhost:7774/rpc/";
    private const float OwnUpdateFreq = 60 * 3;
    private const float PositionUpdateFreq = 60 * 1;
    private const float PositionUpdateFreqMove = 5;
    private const float PlayerQueryFreq = 60 * 2;
	private const float EnemyTurnFreq = 3f;

    public POI[] POIs = new POI[0];
    public int pois_version = 0;
    public float pois_timeQ;
    public bool pois_valid = false;
    public string lastFarmResult = "";

    private List<Creature> _allOwnCreatures;

    public List<Creature> AllOwnCreatures
    {
        get { return _allOwnCreatures; }
    }

    [SerializeField]
    public Player Player = new Player();
    public string ServerURL = Server;
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
	[SerializeField]
	private float _lastEnemyTurn = -1000;

    public bool LoggedIn { get { return SessionID.Length != 0; } }

    public static GameManager Singleton
    {
        get
        {
            if (_instance != null)
                return _instance;
            return null;
        }
    }


    private void Awake()
    {
        //Check for Singleton
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
        {
            Debug.LogError("Second instance of GameManager.");
            Destroy(gameObject);
            return;
        }
        Init();
    }

    private void Init()
    {
        _view = ViewController.Create();
        _map = (Instantiate(Resources.Load<GameObject>("Map")) as GameObject).GetComponent<MapGrid>();
        _map.name = "Map";
        _allOwnCreatures = null;

#if !UNITY_EDITOR
        Social.Active = new UnityEngine.SocialPlatforms.GPGSocial();
        Social.localUser.Authenticate(OnAuthCB);
#endif
        if (DummyUI)
            InitializeDummyObjects();
    }

    private void InitializeDummyObjects()
    {
        CreateController<GUIMap>("dummy_GUIMap").Init(_view.Camera3D.transform.parent, _map);
        CreateController<GUIBase>("dummy_GUIBase");
    }

    private void Update() //<-------------------------------------------------------------------------------------------------------
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
        else if (CurrentGameMode == GameMode.Base)
        {
            if (_allOwnCreatures == null)
            {
                GetCreatures();
            }
        } 
		else if (CurrentGameMode == GameMode.Fight)
		{
			if(Player.Fighting && Player.CurFight != null && !Player.CurFight.Turn)
			{
                //Debug.Log("F:" + Player.CurFight.Turn);
                if (Time.time >= EnemyTurnFreq + _lastEnemyTurn)
					FightEnemyTurn();
			}
		}

        //Load queued players
        if (!_playerQueryActive && _playerQueue.Count > 0)
            StartCoroutine(CGetPlayers());


        if (!pois_valid && pois_timeQ <= 0)
            StartCoroutine(GetPois());

        pois_timeQ -= Time.deltaTime;
    }

    public static TController CreateController<TController>(string name = "") where TController : MonoBehaviour
    {
        if (name == "")
            name = "controller_" + typeof(TController);
        GameObject obj = new GameObject(name)
        {
            hideFlags = HideFlags.DontSave,
            tag = GameManager.DontSaveTag
        };
        TController instance = obj.AddComponent<TController>();
        return instance;
    }

    public void SwitchGameMode(GameMode newGameMode)
    {
        if (newGameMode == CurrentGameMode) {return;};
        Debug.Log("SwitchGameMode:" + CurrentGameMode + " -> " + newGameMode);
        CurrentGameMode = newGameMode;
        switch (newGameMode)
        {
            case GameMode.Map:
                {
                    break;
                }
            case GameMode.Base:
                {
                    break;
                }
            case GameMode.Login:
                {
                    break;
                }
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


    public void Login(string token, Action<bool> callback = null)
    {
        StartCoroutine(CLogin(token, callback));
    }

    /// <summary>
    /// Gets SessionID by PlayerID (Get from OAuth).
    /// </summary>
    /// <param name="playerID"></param>
    public void Login(string playerID, string password, bool local, Action<bool> callback = null)
    {
        ServerURL = local ? Localhost : Server;
        StartCoroutine(CLogin(playerID, password, callback));
    }

    private IEnumerator CLogin(string token, Action<bool> callback = null)
    {
        WWW request = new WWW(ServerURL + "login_google?token=" + token);
        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json)) yield break;
        SessionID = (string)json["data"]["sid"];
        Player.PlayerID = (string)json["data"]["pid"];
        GetOwnPlayer(callback);
    }

    private IEnumerator CLogin(string playerID, string password, Action<bool> callback = null)
    {
        string url = ServerURL + "login_password?pid=" + playerID + "&pass=" + password;
        Debug.Log("URL: " + url);
        WWW request = new WWW(url);

        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json)) yield break;
        SessionID = (string)json["data"];
        Player.PlayerID = playerID;
        GetOwnPlayer(callback);

    }

    /// <summary>
    /// Loads own Player Data (First login with playerID).
    /// </summary>
    public void GetOwnPlayer(Action<bool> callback = null)
    {
        if (!LoggedIn) return;
        _lastOwnPlayerUpdate = Time.time;
        StartCoroutine(CGetOwnPlayer(callback));
    }

    private IEnumerator CGetOwnPlayer(Action<bool> callback = null)
    {
        WWW request = new WWW(GetSessionURL("pinfo"));

        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json))
        {
            callback(false);
            yield break;
        }
        //Debug.Log(json["data"]);
        Player.ReadJson(json["data"]);
        if (Player.CurCreature != null && _allOwnCreatures != null)
        {
            for (int i = 0; i < _allOwnCreatures.Count; i++)
            {
                if (Player.CurCreature.CreatureID == _allOwnCreatures[i].CreatureID)
                {
                    _allOwnCreatures[i] = Player.CurCreature;
                }
            }
        }
		Player.UpdateBattle();
        if (CurrentGameMode == GameMode.Login)
            SwitchGameMode(GameMode.Map);
		if (Player.Fighting && CurrentGameMode != GameMode.Fight)
			SwitchGameMode(GameMode.Fight);
        if (callback != null)
            callback(true);
    }

    public void GetCreatures()
    {
        if (_allOwnCreatures == null)
        {
            _allOwnCreatures = new List<Creature>();

        }
        StartCoroutine(CGetCreatures());
    }

    private IEnumerator CGetCreatures()
    {
        WWW request = new WWW(GetSessionURL("getcrs"));

        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json))
        {
            _allOwnCreatures = null;
            yield break;
        }

        JSONObject creatureJson = json["data"];


        for (int i = 0; i < creatureJson.Count; i++)
        {
            Creature curCreature = new Creature();
            curCreature.ReadJson(creatureJson[i]);
            _allOwnCreatures.Add(curCreature);
        }
    }

    ///<summary>
    /// Exchange of resources.
    /// </summary>

    public void Exchange(int element, int level, int count, ExchangeMode exchangeMode)
    {
        StartCoroutine(CExchange(element, level, count, (int)exchangeMode));
    }

    private IEnumerator CExchange(int element, int level, int count, int exchangeMode)
    {
        WWW request = new WWW(GetSessionURL("exchange") + "&element=" + element + "&level=" + level + "&count=" + count + "&exchangeMode=" + exchangeMode);
        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json)) yield break;

        GetOwnPlayer();
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
        WWW request = new WWW(GetSessionURL("setpos") + "&lon=" + pos.x + "&lat=" + pos.y);

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
        if (!CheckResult(json)) { yield break; }
        JSONObject playersJSON = json["data"];
        PlayersOnMap = new ObjectPos[playersJSON.Count];
        for (int i = 0; i < PlayersOnMap.Length; i++)
        {
            PlayersOnMap[i] = new ObjectPos((string)playersJSON[i][2], (float)playersJSON[i][1], (float)playersJSON[i][0]);
        }
    }

    /// <summary>
    /// blah
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public void FightPlayerTurn(int s0, int s1, int s2, int s3)
    {
        if (!LoggedIn) return;
        _lastEnemyTurn = Time.time;
        StartCoroutine(CFightPlayerTurn(s0,s1,s2,s3));
    }

    private IEnumerator CFightPlayerTurn(int s0, int s1, int s2, int s3)
    {
        WWW request = new WWW(GetSessionURL("fightplayerturn") + "&s0=" + s0 + "&s1=" + s1 + "&s2=" + s2 + "&s3=" + s3);
        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json)) { yield break; }
        JSONObject turnJSON = json["data"];
        if (!(bool)turnJSON) yield break;
        Player.CurFight.ReadJson(turnJSON);
        Player.UpdateBattle();
    }
	
	/// <summary>
	/// blah
	/// </summary>
	/// <param name=""></param>
	/// <returns></returns>
	public void FightEnemyTurn()
	{
		if (!LoggedIn) return;
		_lastEnemyTurn = Time.time;
		StartCoroutine(CFightEnemyTurn());
	}

	private IEnumerator CFightEnemyTurn()
	{
		WWW request = new WWW(GetSessionURL("fightenemyturn"));
		
		yield return request;

		JSONObject json = JSONParser.parse(request.text);
		if (!CheckResult(json)) { yield break; }
		JSONObject turnJSON = json["data"];
		if(!(bool)turnJSON) yield break;
		Player.CurFight.ReadJson(turnJSON);
		Player.UpdateBattle();
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
        Debug.Log("Get Players: " + string.Join(",", requestedPIDs));
        WWW request = new WWW(GetSessionURL("pinfo") + "&pinfo=" + string.Join(",", requestedPIDs));

        yield return request;
        _playerQueryActive = false;
        JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json)) { yield break; }
        JSONObject playersJSON = json["data"];
        for (int i = 0; i < playersJSON.Count; i++)
        {
            if (!(bool)playersJSON[i])
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

    public void AddXP(string xp)
    {
        StartCoroutine(CAddXP(xp));
    }

    public IEnumerator CAddXP(string xp)
    {

        WWW request = new WWW(GetSessionURL("addxp") + "&xp=" + xp);

        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json)) { yield break; }

        GetOwnPlayer();

    }

    public void Attack(string enemy)
    {
        StartCoroutine(CAttack(enemy));
    }

    public IEnumerator CAttack(string enemy)
    {

        WWW request = new WWW(GetSessionURL("attack") + "&enemy=" + enemy);

        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json)) { yield break; }

        GetOwnPlayer();

    }

    public void SetInitSteps(int steps)
    {

        StartCoroutine(CSetInitSteps(steps));
    }

    public IEnumerator CSetInitSteps(int steps)
    {
        WWW request = new WWW(GetSessionURL("setinitsteps") + "&v=" + steps);
        Player.InitSteps = steps;

        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json)) { yield break; }
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
        if ((bool) json["result"]) return true;
        string sErr = (string) json["error"];
        Debug.LogError("RPC Fail: " + sErr);
        if (sErr == "invalid_session")
        {
            SessionID = "";
            SwitchGameMode(GameMode.Login);
            return false;
        }
        if (sErr == "invalid_fight")
        {
            SwitchGameMode(GameMode.Map);
            return false;
        }

        return false;
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

#if !UNITY_EDITOR
    void OnApplicationFocus(bool focusStatus)
    {
        if (focusStatus)
            (new AndroidJavaClass("com.nerdiacs.nerdgpgplugin.NerdGPG")).CallStatic("HideNavigationBar");
    }
#endif


    void OnDisable()
    {
        if (Application.isEditor)
        {
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(DontSaveTag);
            foreach (GameObject o in gameObjects)
            {
                // Object.Destroy is delayed, and never completes in the editor, so use DestroyImmediate instead.
                DestroyImmediate(o);
            }

        }
    }

    void OnAuthCB(bool result)
    {
        if (!result)
        {
            //@To-do: Do something!
        }

        string token = NerdGPG.Instance().GetToken();
        if (string.IsNullOrEmpty(token))
        {
            //@To-do: Do something!
            return;
        }
        Debug.Log("GPG: Got Login Response: " + result);
        Debug.Log("Token: " + token);
        Login(token, OnPlayerLoaded);

    }

    private void OnPlayerLoaded(bool result)
    {
        if (!result)
        {
            Debug.LogError("Couldn't load Player Data!");
            return;
        }

        if (DummyUI)
        {
            Debug.LogWarning("Be aware, that you disabled the real view \nby enabling DummyUI in the GameManager.\nIf you have further Questions, please contact Anton.");
            return;
        }
        _view.AddIrisPopup("iris_01_text", "test").AddIrisPopup("iris_02_01_text", "test");
    }

    //####################################  Editor Login ###################################
#if UNITY_EDITOR
    public string PlayerID = "PlayerID";
    public string Password = "Password";

    void OnGUI()
    {


        if (!LoggedIn)
        {
            PlayerID = GUI.TextField(new Rect(10, 10, 200, 20), PlayerID, 100);
            Password = GUI.TextField(new Rect(10, 40, 200, 20), Password, 100);
            if (GUI.Button(new Rect(10, 70, 100, 20), "Login"))
            {
                Login(PlayerID, Password, false, OnPlayerLoaded);
            }

            if (GUI.Button(new Rect(210, 70, 100, 20), " >>> Local <<<"))
            {
                Login(PlayerID, Password, true, OnPlayerLoaded);
            }
        }

    }
#endif
}