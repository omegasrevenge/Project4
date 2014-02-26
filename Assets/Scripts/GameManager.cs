using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GooglePlayGames;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameMode { Login, Map, Fight, Base };
    public enum ExchangeMode { Up, Down, Cricle };
    public enum ResourceElement { None = -1, energy, fire, storm, life, water };
	public enum ResourceLevel { biod, driod_lvl0, driod_lvl1, driod_lvl2, driod_lvl3, driod_lvl4, driod_lvl5 };

	public static readonly Color32 Black = new Color32(0, 0, 0, 255);
	public static readonly Color32 Withe = new Color32(255, 255, 255, 255);
    public static readonly Color32 NCERed  = new Color32(120, 40, 40, 255);
    public static readonly Color32 NCEGrey = new Color32(50, 50, 50, 255);

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
    private ViewController  _view;
    private Map             _map;
    private PlayerBase      _base;
    private BattleEngine    _fight;
    public string lastPlayerRequest = "none";

    public bool DummyUI = true;

    private const string Server = "http://pixeltamer.net:7774/rpc/";
    private const string Localhost = "http://localhost:7774/rpc/";
    private const float OwnUpdateFreq = 5;
    private const float PositionUpdateFreq = 30;
    private const float PositionUpdateFreqMove = 5;
    private const float PlayerQueryFreq = 10;
    private const float EnemyTurnFreq = 2;

    public List<POI> POIs = new List<POI>();
    public float pois_timeQ;
    public bool pois_valid = false;
    public bool BattleEngineSkipTurn = false;
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
    public JSONObject Techtree;

    private ObjectPos[] PlayerPositionsInRange;
    public List<Player> PlayersOnMap = new List<Player>();
    private bool _playersUpdated = false;
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

    public bool LoggedIn
    {
        get
        {
            bool socialActive = true;
            
#if !UNITY_EDITOR
            socialActive = (Social.Active is PlayGamesPlatform) && ((PlayGamesPlatform) Social.Active).IsAuthenticated();
#endif
            return SessionID.Length != 0 && socialActive;
        }
    }

    private GUIObjectPopup challengePopup;

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
        Application.targetFrameRate = 25;


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
        SoundController.Create().Init();
        //_map = (Instantiate(Resources.Load<GameObject>("Map")) as GameObject).GetComponent<MapGrid>();
        //_map.name = "Map";
        _allOwnCreatures = null;

        StartOAuth();
#if UNITY_EDITOR
		PlayerID=PlayerPrefs.GetString("PlayerID");
		Password=PlayerPrefs.GetString("Password");
#endif
    }

    public void StartOAuth()
    {
#if !UNITY_EDITOR
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        Social.localUser.Authenticate(OnAuthCB);
#endif
    }


    private void Update() //<-------------------------------------------------------------------------------------------------------
    {
        if (!LoggedIn) return;

        //Update own player information
		if (Time.time >= OwnUpdateFreq + _lastOwnPlayerUpdate && CurrentGameMode != GameMode.Fight)
            GetOwnPlayer(null,true);

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
            CheckStartFight();
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
            if (Player.Fighting && Player.CurFight != null /*&& !Player.CurFight.Turn*/)
            {
                //Debug.Log("F:" + Player.CurFight.Turn);
                if (Time.time >= EnemyTurnFreq + _lastEnemyTurn)
                    FightEnemyTurn();
            }
        }

        //Load queued players
        if (!_playerQueryActive && _playerQueue.Count > 0)
            StartCoroutine(CGetPlayers());

        if (_playersUpdated)
        {
            UpdatePlayersOnMap();
        }

        if (!pois_valid && pois_timeQ <= 0)
            StartCoroutine(GetPois());

        pois_timeQ -= Time.deltaTime;
    }



    private void UpdatePlayersOnMap()
    {
        if (PlayerPositionsInRange == null) return;
        List<Player> tempPlayers = new List<Player>();
        foreach (ObjectPos pos in PlayerPositionsInRange)
        {
            Player player = GetPlayer(pos.ID);
			if (player == null) continue;
			player.Position.x=pos.Lon;
			player.Position.y=pos.Lat;
            tempPlayers.Add(player);
        }
        PlayersOnMap = tempPlayers;
        _playersUpdated = false;
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
        if (newGameMode == CurrentGameMode) { return; };

        Debug.Log("SwitchGameMode:" + CurrentGameMode + " -> " + newGameMode);

        if (CurrentGameMode == GameMode.Fight)
        {
            FightDelete();
        }

        CurrentGameMode = newGameMode;
        
        switch (newGameMode)
        {
            case GameMode.Login:
                _view.Switch3DSceneRoot(null);
                break;
            case GameMode.Map:
                if (!_map)
                {
                    _map = Map.Create();
                    _map.AttachGUI(_view.AddMapUI());
                    _map.SetCreatureInfo(Player.CurCreature);
                }
                _view.Switch3DSceneRoot(_map);
                break;
            case GameMode.Base:
                if (!_base)
                {
                    _base = PlayerBase.Create();
                    _base.AttachGUI(_view.AddBaseUI());
                }
                _view.Switch3DSceneRoot(_base);
                playBaseBackgroundMusic();
                break;
            case GameMode.Fight:
                if(BattleEngine.CurrentGameObject == null)
				{
                    _fight = BattleEngine.Create(Player.GetBattleInit());
					_fight.AttachGUI(_view.AddBattleUI());
				}
				_view.Switch3DSceneRoot(_fight);
				_fight.StartFight(Player.GetBattleInit());
                break;
        }
    }

    private void playBaseBackgroundMusic()
    {
        SoundController.PlaySound(Player.CurrentFaction == Player.Faction.VENGEA
            ? BaseSounds.BackgroundVengea
            : BaseSounds.BackgroundNce, BaseSounds.BackgroundChannel).loop = true;
    }

    /// <summary>
    /// Returns a valid URL for the current session to use RPC functions.
    /// </summary>
    /// <param name="function"></param>
    /// <returns></returns>
    public string GetSessionURL(string function)
    {
		long tcnow = (long)((DateTime.UtcNow-new DateTime (1970,1,1)).TotalMilliseconds);
		return ServerURL + function + "?pid=" + Player.PlayerID + "&sid=" + SessionID+"&tc="+tcnow;
    }

    public void Login(string token, Action<bool> callback = null)
    {
        StartCoroutine(CLogin(token, callback));
    }

    public void Logout()
    {
        SessionID = "";
        Player = new Player();
#if !UNITY_EDITOR
        ((PlayGamesPlatform) Social.Active).SignOut();
#endif
        _view.AddMaxScreen(GUIObjectLoginScreen.Create());
        _view.HideLoadingScreen();
        SwitchGameMode(GameMode.Login);
        if(_map)
            _map.HideMenu();
        else
        {
            Debug.Log("NoMap");
        }
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
		NTPEntries.Clear();
        WWW request = new WWW(ServerURL + "login_google?token=" + token);
        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json,request.url)) yield break;
        SessionID = (string)json["data"]["sid"];
        Player.PlayerID = (string)json["data"]["pid"];
        GetOwnPlayer(callback);
    }

    private IEnumerator CLogin(string playerID, string password, Action<bool> callback = null)
    {
		NTPEntries.Clear();
        string url = ServerURL + "login_password?pid=" + playerID + "&pass=" + password;
        Debug.Log("URL: " + url);
        WWW request = new WWW(url);
        yield return request;
        Debug.Log("Login: " + request.text);
        JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json, request.url))
        {
            Logout();
            yield break;
        }
        SessionID = (string)json["data"];
        Player.PlayerID = playerID;
        GetOwnPlayer(callback);
       

    }

    /// <summary>
    /// Loads own Player Data (First login with playerID).
    /// </summary>
    public void GetOwnPlayer(Action<bool> callback = null,bool version=false)
    {
        if (!LoggedIn) return;
        _lastOwnPlayerUpdate = Time.time;
		StartCoroutine(CGetOwnPlayer(callback,version));
        if (Techtree == null)
            StartCoroutine(CRequestTechtree());
    }

	private IEnumerator CGetOwnPlayer(Action<bool> callback = null,bool version=false)
    {
		string sSessUrl=GetSessionURL("pinfo");
		if(version) {
			sSessUrl+="&v=" + Player.Version;
            if (Player.CurFight != null)
            {
                sSessUrl += "&fv=" + Player.CurFight.Version;
            }
			if (Player.CurCreature != null)
			{
				sSessUrl += "&cv=" + Player.CurCreature.Version;
			}
		}
        WWW request = new WWW(sSessUrl);

        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json,request.url))
        {
            if (callback != null)            callback(false);
            yield break;
		};
		JSONObject data=json["data"];
		if((string)data=="nochange")
		{
            Debug.Log("nochange");
			yield break;
		};
        Debug.Log(json["data"]);
		ReadPlayerJSON(data);
	    CheckStartFight();
        if (callback != null)
            callback(true);
    }



    public void ReadPlayerJSON(JSONObject jsonPlayer)
    {
        Player.ReadJson(jsonPlayer);
        if (Player.CurCreature != null  && _map)
        {
            _map.SetCreatureInfo(Player.CurCreature);
        }

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

        if (!_fight) return;
        if (BattleEngineSkipTurn) BattleEngine.Current.SkipOneTurn = true;
        BattleEngineSkipTurn = false;
        _fight.Result = Player.GetResult();
    }

    public void CheckStartFight()
    {
        Fight fight = Player.CurFight;
        if (fight == null || fight.Finished)
        {
            if(_map)
                _map.HideFightInvation();
            return;
        }
        if (!fight.Started)
        {
            Player FighterA=GetPlayer(fight.FighterA.PId);
            Player FighterB=GetPlayer(fight.FighterB.PId);
            if (FighterA == null || FighterB == null) return; 
            if (FighterB.PlayerID == lastPlayerRequest) return;
            // not for challenger !!!!!
            if (!fight.FighterA.challenger)
            {
                lastPlayerRequest = FighterB.PlayerID;
                if (_map)
                    _map.ShowFightInvation();
            }                
            return;
        }

        if (Player.Fighting && CurrentGameMode != GameMode.Fight)
            SwitchGameMode(GameMode.Fight);
    }

	public void CreateCreature(int element, Action<bool, string> callback)
	{
		StartCoroutine(CCreateCreature(element, callback));
	}

	private IEnumerator CCreateCreature(int element, Action<bool, string> callback)
	{
		WWW request = new WWW(GetSessionURL("crcr") + "&element=" + element);

		yield return request;

		JSONObject json = JSONParser.parse(request.text);
		if (!CheckResult(json,request.url))
		{
			if (callback != null)
				callback(false, (string)json["error"]);
			yield break;
		}
		if (callback != null)
			callback(true, "");

		GetOwnPlayer();
	}

	public void NameCreature(int creatureID, string name, Action<bool, string> callback)
	{
		StartCoroutine(CNameCreature(creatureID, name, callback));
	}

	private IEnumerator CNameCreature(int creatureID, string name, Action<bool, string> callback)
	{
		WWW request = new WWW(GetSessionURL("namecr") + "&cid=" + creatureID + "&name=" + name);

		yield return request;

		JSONObject json = JSONParser.parse(request.text);
		if (!CheckResult(json,request.url))
		{
			if (callback != null)
				callback(false, (string)json["error"]);
			yield break;
		}
		if (callback != null)
			callback(true, "");

		GetOwnPlayer();
	}

    public void AddCreatureEQSlot(int creatureID)
    {
        StartCoroutine(CAddCreatureEQSlot(creatureID));
    }

    private IEnumerator CAddCreatureEQSlot(int creatureID)
    {
        WWW request = new WWW(GetSessionURL("addcrsl") + "&cid=" + creatureID);
        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json,request.url)) yield break;

        UpdateAllOwnCreatures(json["data"]);

        GetOwnPlayer();
    }

    public void EquipCreatureSlot(int creatureID, int slotId, int driodenElement, int driodenLevel)
    {
        StartCoroutine(CEquipCreatureSlot(creatureID, slotId, driodenElement, driodenLevel));
    }

    private IEnumerator CEquipCreatureSlot(int creatureID, int slotId, int driodenElement, int driodenLevel)
    {
        WWW request = new WWW(GetSessionURL("equipcrsl") + "&cid=" + creatureID + "&slotid=" + slotId + "&element=" + driodenElement + "&level=" + driodenLevel);
        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        Debug.Log(json);
        if (!CheckResult(json,request.url)) yield break;

        UpdateAllOwnCreatures(json["data"]);

        GetOwnPlayer();
    }

    public void UpgradeCreatureSlot(int creatureID, int slotId, int driodenElement)
    {
        StartCoroutine(CUpgradeCreatureSlot(creatureID, slotId, driodenElement));
    }

    private IEnumerator CUpgradeCreatureSlot(int creatureID, int slotId, int driodenElement)
    {
        WWW request = new WWW(GetSessionURL("upgradecrsl") + "&cid=" + creatureID + "&slotid=" + slotId + "&element=" + driodenElement);
        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        Debug.Log(json);
        if (!CheckResult(json,request.url)) yield break;

        UpdateAllOwnCreatures(json["data"]);

        GetOwnPlayer();
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
        if (!CheckResult(json,request.url))
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

	public void SwitchCurrentCreature(int creatureID)
	{
		StartCoroutine(CSwitchCurrentCreature(creatureID));
	}

	private IEnumerator CSwitchCurrentCreature(int creatureID)
	{
		WWW request = new WWW(GetSessionURL("switchcurcr") + "&cid=" + creatureID);
		yield return request;

		JSONObject json = JSONParser.parse(request.text);
		Debug.Log(json);
		if (!CheckResult(json,request.url)) yield break;

		GetOwnPlayer();
	}

    ///<summary>
    /// Exchange of resources.
    /// </summary>

	public void Exchange(int element, int level, int count, ExchangeMode exchangeMode, int lvldiff)
    {
		StartCoroutine(CExchange(element, level, count, (int)exchangeMode, lvldiff));
    }

    private IEnumerator CExchange(int element, int level, int count, int exchangeMode, int lvldiff)
    {
		WWW request = new WWW(GetSessionURL("exchange") + "&element=" + element + "&level=" + level + "&count=" + count + "&exchangeMode=" + exchangeMode + "&lvldiff=" + lvldiff);
        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json,request.url)) yield break;

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
        if (!CheckResult(json,request.url)) yield break;
        Player.Position = pos;
    }

    private IEnumerator CRequestTechtree()
    {
        WWW request = new WWW(GetSessionURL("gettechtree"));
        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json,request.url)) yield break;
        Techtree = json["data"];
    }

    public void SendBasePosition()
    {
        if (!LoggedIn) return;

        Player.BaseTime = GetServerTime();
        StartCoroutine(CSendBasePosition());
    }

    private IEnumerator CSendBasePosition()
    {
        Vector2 pos = LocationManager.GetCurrentPosition();
        WWW request = new WWW(GetSessionURL("setbasepos") + "&lon=" + pos.x + "&lat=" + pos.y);

        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json,request.url)) yield break;
        Player.BasePosition = pos;
        BaseOnMap.Singleton.SetProjPos();
    }

    public void PoiFarm(POI poi)
    {
        if (!LoggedIn) return;
        StartCoroutine(CPoiFarm(poi));
    }

    private IEnumerator CPoiFarm(POI poi)
    {
        WWW request = new WWW(GetSessionURL("poifarm") + "&mappos=" + poi.MapPos + "&poiid=" + poi.POI_ID);
        //Debug.Log(request.url);
        yield return request;

        JSONObject json = JSONParser.parse(request.text);
		//Debug.Log(json);
		JSONObject data = json["data"];
		bool succeeded = data["Success"];
		if (data.HasField("NextFarm")) {
			DateTime nextFarm = (DateTime)data["NextFarm"];
			poi.NextFarm=nextFarm;
		}
        if (!CheckResult(json,request.url)) yield break;
        _lastOwnPlayerUpdate = -1000;
        GUIShowFarmResult(succeeded, data["Result"]);
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
        if (!CheckResult(json,request.url)) { yield break; }
        JSONObject playersJSON = json["data"];
        PlayerPositionsInRange = new ObjectPos[playersJSON.Count];
        for (int i = 0; i < PlayerPositionsInRange.Length; i++)
        {
            PlayerPositionsInRange[i] = new ObjectPos((string)playersJSON[i][2], (float)playersJSON[i][1], (float)playersJSON[i][0]);
        }
        UpdatePlayersOnMap();
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
        StartCoroutine(CFightPlayerTurn(s0, s1, s2, s3));
    }

    private IEnumerator CFightPlayerTurn(int s0, int s1, int s2, int s3)
    {
        WWW request = new WWW(GetSessionURL("fightplayerturn") + "&s0=" + s0 + "&s1=" + s1 + "&s2=" + s2 + "&s3=" + s3);
        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        if (json["error"] != null && BattleEngine.CurrentGameObject != null && ((string)json["error"]).Equals("invalid_fight")) //<------------------------
            Player.CurFight.Finished = true;
        if (!CheckResult(json,request.url)) { yield break; }
        JSONObject turnJSON = json["data"];
        if (!turnJSON) yield break;
        ReadPlayerJSON(turnJSON);
        Debug.Log("!!!!!!!!!!!!!!a " + request.text);
    }

    public void CatchAttempt(int driodLevel) { StartCoroutine(CCatchAttempt(driodLevel)); }

    private IEnumerator CCatchAttempt(int driodLevel)
    {
        WWW request = new WWW(GetSessionURL("catchcr") + "&level=" + driodLevel); //<---------------------------------
        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        string message;
        if ((bool) json["result"])
        {
            SoundController.PlaySound(BattleSounds.TameSuccess, BattleSounds.MiscSoundChannel);
            BattleEngine.Current.EnemyCreature.GetComponent<MonsterAnimationController>().DoAnim("tame_success");
            message = "Success!";
        }
        else
        {
            SoundController.PlaySound(BattleSounds.TameFail, BattleSounds.MiscSoundChannel);
            BattleEngine.Current.EnemyCreature.GetComponent<MonsterAnimationController>().DoAnim("tame_fail");
            message = "Fail!";
        }
        BattleEngine.Current.View.ShowDamageIndicators(new List<GUIObjectBattleEngine.IndicatorContent>
        { new GUIObjectBattleEngine.IndicatorContent(BattleEngine.Current.EnemyCreature, message, 0, 1.5f, 3f) });
        BattleEngine.Current.View.TxtIndicators[0].GetComponent<IndicatorController>().CatchResult = true;
        //if (!CheckResult(json,request.url)) { yield break; }
        BattleEngineSkipTurn = true;
        GetOwnPlayer();
    }

    public void EscapeAttempt() { StartCoroutine(CEscapeAttempt()); }

    private IEnumerator CEscapeAttempt()
    {
        WWW request = new WWW(GetSessionURL("escape"));
        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        if (!(bool)json["result"] && ((string)json["error"]).Equals("same_round")) { yield break; }
        string message;
        float delay;
        if ((bool) json["result"])
            message = "Success!";
        else
            message = "Fail!";
        BattleEngine.Current.View.ShowDamageIndicators(new List<GUIObjectBattleEngine.IndicatorContent>
        { new GUIObjectBattleEngine.IndicatorContent(BattleEngine.Current.FriendlyCreature, message, 0, 2f) });
		Debug.LogError (json);
        //if (!CheckResult(json,request.url)) { yield break; }
        BattleEngineSkipTurn = true;
        GetOwnPlayer();
    }

    /// <summary>
    /// blah
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public void FightEnemyTurn()
    {
        if (!LoggedIn) return;
        if (Player.CurFight == null) return;
        if (Player.CurFight.Finished) return;

        _lastEnemyTurn = Time.time;
        StartCoroutine(CFightEnemyTurn());
    }

    private IEnumerator CFightEnemyTurn()
    {
        WWW request = new WWW(GetSessionURL("fightenemyturn"));

        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        if (json["error"] != null && BattleEngine.CurrentGameObject != null && ((string)json["error"]).Equals("invalid_fight")) //<------------------------
            Player.CurFight.Finished = true;
        if (!CheckResult(json,request.url)) { yield break; }
        JSONObject turnJSON = json["data"];
        if (!turnJSON) yield break;
        ReadPlayerJSON(turnJSON);
        Debug.Log("!!!!!!!!!!!!!!b " + request.text);
    }

    public void FightDelete()
    {
        if (!LoggedIn) return;
        StartCoroutine(CFightDelete());
    }
    private IEnumerator CFightDelete()
    {
        WWW request = new WWW(GetSessionURL("fightdelete"));
        yield return request;
        JSONObject json = JSONParser.parse(request.text);
        CheckResult(json,request.url);
    }

    /// <summary>
    /// Loads player data by passing the playerID.
    /// </summary>
    /// <param name="playerID"></param>
    /// <returns></returns>
    public Player GetPlayer(string playerID)
    {
        if (!LoggedIn) return null;
        if (playerID == null || playerID.Length != 21)
        {
            Debug.Log("!!!!!!!! Invalid PId" + playerID);
            return null;
        }
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
        //Debug.Log("Get Players: " + string.Join(",", requestedPIDs));
        WWW request = new WWW(GetSessionURL("pinfo") + "&pinfo=" + string.Join(",", requestedPIDs));

        yield return request;
        _playerQueryActive = false;
        JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json,request.url)) { yield break; }
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

        _playersUpdated = true;
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
        if (!CheckResult(json,request.url)) { yield break; }

        GetOwnPlayer();
    }

    public void Attack(string enemy)
    {
        StartCoroutine(CAttack(enemy));
    }

    public IEnumerator CAttack(string enemy)
    {
        WWW request = new WWW(GetSessionURL("fightcreate") + "&enemy=" + enemy);

        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json,request.url)) { yield break; }

        GetOwnPlayer();
    }

    public void Accept()
    {
        StartCoroutine(CAccept());
    }

    public IEnumerator CAccept()
    {
        WWW request = new WWW(GetSessionURL("accepted"));

        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json,request.url)) { yield break; }

        GetOwnPlayer();
    }

    public void Decline()
    {
        StartCoroutine(CDecline());
    }

    public IEnumerator CDecline()
    {
        WWW request = new WWW(GetSessionURL("declined"));

        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json,request.url)) { yield break; }

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
        if (!CheckResult(json,request.url)) { yield break; }
    }

    public void SetFirewall(bool firewall)
    {
        if (firewall == Player.Firewall) return;
        StartCoroutine(CSetFirewall(firewall));
    }

    public IEnumerator CSetFirewall(bool firewall)
    {
        WWW request = new WWW(GetSessionURL("setFirewall") + "&fiwa=" + firewall);
        Player.Firewall = firewall;

        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json, request.url)) { yield break; }
    }

    public void SetFaction(Player.Faction faction)
    {
        if (faction == Player.CurrentFaction) return;
        StartCoroutine(CSetFaction(faction));
    }

    public IEnumerator CSetFaction(Player.Faction faction)
    {
        WWW request = new WWW(GetSessionURL("setfaction") + "&fac=" + (int)faction);
        //Player.CurrentFaction = faction;

        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json, request.url)) { yield break; }
        gameObject.AddComponent<GUIObjectReboot>();
    }

    public void SubmitPlayerName(string name, Action<bool, string> callback)
    {
        StartCoroutine(CSubmitPlayerName(name, callback));
    }

    public IEnumerator CSubmitPlayerName(string name, Action<bool, string> callback)
    {
        WWW request = new WWW(GetSessionURL("nameplayer") + "&name=" + name);
        Player.Name = name;

        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json,request.url))
        {
            if (callback != null)
                callback(false, (string)json["error"]);
            yield break;
        }
        if (callback != null)
            callback(true, "");
    }

    private IEnumerator GetPois()
    {
        pois_timeQ = 3;
        pois_valid = true;
        Vector2 pos = LocationManager.GetCurrentPosition();
        WWW request = new WWW(Singleton.GetSessionURL("getpois") + "&lon=" + pos.x + "&lat=" + pos.y);

        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json,request.url)) yield break;

        JSONObject data = json["data"];
        JSONObject pois = data["POIs"];
        List<POI> tmpPOIs = new List<POI>();
        for (int i = 0; i < pois.Count; i++)
        {
            POI tempPOI = POI.ReadJson(pois[i]);

            POI poi = POIs.FirstOrDefault(p => p.POI_ID == tempPOI.POI_ID);
            if (Equals(poi, default(POI)))
                poi = tempPOI;
            tmpPOIs.Add(poi);
        }

        POIs = tmpPOIs;
    }

    /// <summary>
    /// Checks if the request was successful.
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public bool CheckResult(JSONObject json,string url)
    {
		long tcnow = (long)((DateTime.UtcNow-new DateTime (1970,1,1)).TotalMilliseconds);
		long tcold=(long)json["tc"];
		long ts=(long)json["ts"];
		if(ts!=0&&tcold!=0)
		{
			long dur=tcnow-tcold;
			long tc=(tcold+tcnow)/2;
			long diff=tc-ts;
			NTPPush(dur,diff);
		};

        if ((bool)json["result"]) return true;
        string sErr = (string)json["error"];
        Debug.LogError("RPC Fail: " + sErr + "@ "+url);
        if (sErr == "invalid_session")
        {
            SessionID = "";
            SwitchGameMode(GameMode.Login);
            return false;
        }
        return false;
    }

	struct NTPEntry
	{
		public long dur;
		public long diff;
	}
	private List<NTPEntry> NTPEntries=new List<NTPEntry>();
	private long ServerTimeDiff=0;

	private void NTPPush(long dur,long diff)
	{
		//Debug.Log("dur:"+dur+" diff:"+diff);
		if(dur>1000&&NTPEntries.Count<=0) {return;}; //bad timing, ignore
		NTPEntry e;e.dur=dur;e.diff=diff;
		NTPEntries.Add(e);
		if(NTPEntries.Count>32) {NTPEntries.RemoveAt(0);}; //remove old

		long dSum=0;
		long dWgh=0;
		for(int i=0;i<NTPEntries.Count;i++)
		{
			diff=NTPEntries[i].diff;
			dur=NTPEntries[i].dur;
			long weight=1000-dur;
			if(weight<1) weight=1; 
			weight*=weight;
			dWgh+=weight;
			dSum+=weight*diff;
		};
		ServerTimeDiff=dSum/dWgh;
		//Debug.Log("Avarage Diff:"+ServerTimeDiff+" c:"+NTPEntries.Count+" s:"+dSum+" w:"+dWgh);
		//GetServerTime();
	}

	//returns synchronized server time in UTC
	public DateTime GetServerTime()
	{
		long tcnow = (long)((DateTime.UtcNow-new DateTime (1970,1,1)).TotalMilliseconds);
		tcnow-=ServerTimeDiff;
		DateTime UnixStartDate = new DateTime(1970, 1, 1);
		UnixStartDate=UnixStartDate.AddMilliseconds(tcnow);
		//Debug.Log(UnixStartDate.ToLocalTime().ToString());
		return UnixStartDate;
	}


    void OnApplicationFocus(bool focusStatus)
    {
        //(new AndroidJavaClass("com.nerdiacs.nerdgpgplugin.NerdGPG")).CallStatic("HideNavigationBar");
    }



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
            Logout();
        StartCoroutine(RequestToken());
    }

    private IEnumerator RequestToken()
    {
        string error = null;
        while (string.IsNullOrEmpty(error))
        {
            string token = ((PlayGamesPlatform)Social.Active).GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                Login(token, OnPlayerLoaded);
                yield break;
            }
            yield return new WaitForEndOfFrame();
            error = ((PlayGamesPlatform) Social.Active).GetRequestError();
        }    
        Debug.Log("Cannot get Token: "+error);


    }

    private void OnPlayerLoaded(bool result)
    {
        if (!result)
        {
            Debug.LogError("Couldn't load Player Data!");
            return;
        }

        if (!Player.Fighting||CurrentGameMode==GameMode.Login)
        {
            switch (Player.InitSteps)
            {
                case (0):
                    GUIStartIRISinstructions();
                    break;
				case(1):
					GUIShowSpectreChoice();
					break;
				case (2):
					GUIShowSpectreName();
					break;
                default:
                    SwitchGameMode(GameMode.Map);
                    break;
            }
        }
        _view.HideLoadingScreen();
    }

	private void UpdateAllOwnCreatures(JSONObject json)
	{
		int CId=(int)json["CId"];
		if (Player.CurCreature.CreatureID==CId) {Player.CurCreature.ReadJson(json);};
		
		foreach (Creature curCreature in AllOwnCreatures)
		{
			if (curCreature.CreatureID == CId) curCreature.ReadJson(json);
		}
	}

    #region GUI methods

    public void GUIStartIRISinstructions()
    {
        _view.AddMaxScreen(GUIObjectNameInput.Create("screen_entername_title", "screen_entername_text", "continue", "default_name", GUISubmitName));
        var irisPopUp = _view.AddIrisPopup("iris_01_text", "iris_01");
        irisPopUp.StartCallback = delegate { irisPopUp.AddIrisPopup("iris_02_01_text", "iris_02_01"); };
    }

    public void GUISubmitName(string username)
    {
        //_view.ShowLoadingScreen(Localization.GetText("loadingscreen_submitName"));
        SubmitPlayerName(username, GUINameSubmitted);
        //Endless Loop
    }

    public void GUINameSubmitted(bool result, string error)
    {
        //_view.HideLoadingScreen();
        if (result)
        {
            _view.AddIrisPopup("iris_02_02_text", "iris_02_02").Callback = () =>
            {
                _view.RemoveMaxScreen();
                SpectresIntro intro = SpectresIntro.Create(GUIShowSpectreChoice);
                intro.AttachGUI(_view.AddSpectresIntro("iris_03_text"));
                _view.Switch3DSceneRoot(intro);
            };
            return;
        }
        Debug.Log(error);
    }

    public void GUIShowSpectreChoice()
    {
		_view.AddMaxScreen(GUIObjectChooseElement.Create("screen_chooseelement_title", "screen_chooseelement_text", GUISubmitElement));
		var irisPopUp = _view.AddIrisPopup("iris_04_01_text", "iris_04_01");
		irisPopUp.StartCallback = delegate { irisPopUp.AddIrisPopup("iris_04_02_text", "iris_04_02"); };
    }

	public void GUISubmitElement(int element)
	{
		CreateCreature(element, GUIElementSubmitted);
	}

	public void GUIElementSubmitted(bool result, string error)
	{
		if (result)
		{
			SetInitSteps(2);
			_view.AddIrisPopup("iris_04_03_text", "iris_04_03").Callback = GUIShowSpectreName;
			return;
		}
		Debug.Log(error);
	}

	public void GUIShowSpectreName()
	{
		_view.AddMaxScreen(GUIObjectMonsterNameInput.Create(
					"screen_monsterentername_title",
					"screen_monsterentername_text",
					"continue",
					Player.CurCreature,
					GUISubmitSpectreName));
	}

	public void GUISubmitSpectreName(string monsterName)
	{
		NameCreature(Player.CurCreature.CreatureID, monsterName, GUISpectreNameSubmitted);
		
	}

	public void GUISpectreNameSubmitted(bool result, string error)
	{
		if (result)
		{
			_view.AddIrisPopup("iris_04_04_text", "iris_04_04").Callback = () =>
			{
				SetInitSteps(3);
				_view.RemoveMaxScreen();
				SwitchGameMode(GameMode.Map);
			};
			return;
		}
		Debug.Log(error);
	}

    public void GUIShowFarmResult(bool succeeded, JSONObject result)
    {
        if (result.ToString() != "\"fight\"" && result.ToString() != "\"heal\"")
        {
            string[] results = result.ToString().Replace("\"", "").Split(';');
            FarmResult resultObj = new FarmResult();
            for (int i = 0; i < results.Length; i++)
            {
                string[] rsc = results[i].Split(' ');
                int element = Convert.ToInt32(rsc[0]);
                int level = Convert.ToInt32(rsc[1]);
                resultObj.AddResult(element, level, 1);
                //element[i] = Resource.ResourceTypes[eIndex + 1].ToLower();
   
            }
            _view.ShowPopup(GUIObjectResourceResult.Create("farm_rsc_text", "ok", resultObj));
        }
    }

    public void GUIHideLoginScreen()
    {
        _view.ShowLoadingScreen(Localization.GetText("loadingscreen_login"));
        _view.RemoveMaxScreen();
        StartOAuth();
    }

    public void GUIShowLoadingScreen(string textKey)
    {
        _view.ShowLoadingScreen(Localization.GetText(textKey));
    }

    public void HideLoadingScreen(string textKey)
    {
        _view.HideLoadingScreen();
    }


    public void GUIClickMessage()
    {
        Fight fight = Player.CurFight;
        if (fight == null || fight.Finished) return;
        if (!fight.Started)
        {
            if (challengePopup == null || challengePopup.gameObject == null)
            {
                Player FighterA = GetPlayer(fight.FighterA.PId);
                Player FighterB = GetPlayer(fight.FighterB.PId);
                if (FighterA == null || FighterB == null) return;
                _view.ShowPopup(GUIObjectChallenge.Create("challenge_text", "challenge_title", FighterB.Name, "decline", "accept"));
            }
        }
    }

    public void GUILevelUp()
    {
        _view.ShowPopup(GUIObjectLevelUp.Create());
    }

    public void GUIFirewallWarning()
    {
        _view.ShowPopup(GUIObjectIconPopupContent.Create("popup_warning_firewall_text", "popup_warning_firewall_title", "accept", "symbol_reboot"));
    }

    public void GUIEnableMenu()
    {
        if (_map)
            _map.MenuControlsEnabled = true;
    }

    public void GUIDisableMenu()
    {
        if (_map)
            _map.MenuControlsEnabled = false;
    }

	public void GUIUpdateSpectre(Creature creature)
	{
		if (_base)
			_base.UpdateCreature(creature);
	}

    #endregion

    #region Unity Editor
    //####################################  Editor Login ###################################
#if UNITY_EDITOR
    public string PlayerID = "PlayerID";
    public string Password = "Password";

    private string _crElement = "Element (int)";
    private string _crType =  "Type (int)";
    private bool _cr;
    private bool _keyUp = true;

    void OnGUI()
    {
        if (!LoggedIn)
        {
            PlayerID = GUI.TextField(new Rect(10, 10, 200, 20), PlayerID, 100);
            Password = GUI.TextField(new Rect(10, 40, 200, 20), Password, 100);
            if (GUI.Button(new Rect(10, 70, 200, 20), "Login"))
            {
                Login(PlayerID, Password, false, OnPlayerLoaded);
				PlayerPrefs.SetString("PlayerID",PlayerID);
				PlayerPrefs.SetString("Password",Password);
            }

            if (GUI.Button(new Rect(10, 100, 200, 20), "Local Login"))
            {
                Login(PlayerID, Password, true, OnPlayerLoaded);
				PlayerPrefs.SetString("PlayerID",PlayerID);
				PlayerPrefs.SetString("Password",Password);
			}

            if (GUI.Button(new Rect(10, 130, 200, 20), "Login and Reset"))
            {
                Login(PlayerID, Password, false, EditorResetPlayer);
                PlayerPrefs.SetString("PlayerID", PlayerID);
                PlayerPrefs.SetString("Password", Password);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && _keyUp)
        {
            Debug.Log("Expand");
            _keyUp = false;
            _cr = !_cr;
        }

        if (Input.GetKeyUp(KeyCode.Escape) && !_keyUp)
        {
            Debug.Log("Expand");
            _keyUp = true;
        }
        //Debug.Log(_cr);
        if (_cr && LoggedIn)
        {

            _crElement = GUI.TextField(new Rect(10, 10, 200, 20), _crElement, 100);
            _crType = GUI.TextField(new Rect(10, 40, 200, 20), _crType, 100);
            if (GUI.Button(new Rect(10, 70, 200, 20), "StartFight"))
            {
                _cr = false;
                StartSpecialFight(Convert.ToInt32(_crElement), Convert.ToInt32(_crType));
            }
        }

    }

    public void StartSpecialFight(int elemnt, int type)
    {
        if (!LoggedIn) return;

        StartCoroutine(CStartSpecialFight(elemnt,type));
    }

    private IEnumerator CStartSpecialFight(int element, int type)
    {
        WWW request = new WWW(GetSessionURL("startspecialfight") + "&xp=" + Player.CurCreature.XP + "&element=" + element + "&mid"+type);
        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        JSONObject data = json["data"];
        if (!CheckResult(json, request.url)) yield break;
        _lastOwnPlayerUpdate = -1000;
    }

    void EditorResetPlayer(bool result)
    {
        if (result)
        {
            SetInitSteps(0);
            OnPlayerLoaded(true);
        }
        else
        {
            OnPlayerLoaded(false);
        }
    }
#endif
    #endregion


}