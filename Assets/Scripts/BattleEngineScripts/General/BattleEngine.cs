using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BattleEngine : MonoBehaviour
{
    //########## public #########
    public enum TurnState { Wait, Rotate, Execute, Hit }
    public enum ResourceElement { None = -1, Energy, Fire, Storm, Nature, Water };


    public ActorControlls Actor;
    public FightRoundResult.Player CurrentPlayer;
    public BattleInit ServerInfo;


    public List<int> InputText;
    public int Turn = 0;
    public bool Fighting = true;


    public GameObject BattleCam;
    public GameObject MainCam;
    public GameObject FriendlyCreature;
    public GameObject EnemyCreature;
    public GameObject Arena;
    //########## public #########

    //########## static #########
    public static BattleEngine Current;
    public static GameObject CurrentGameObject;
    //########## static #########

    //########## const #########
    public const string DefaultArena = "DefaultArena";
    public const string DefaultFriendlySpawnPos = "FriendlySpawnPos";
    public const string DefaultEnemySpawnPos = "EnemySpawnPos";
    public const string WolfMonster = "WolfMonster";
    public const string GiantMonster = "GiantMonster";
    //########## const #########

    //########## private #########
    private TurnState _currentStatus = TurnState.Wait;
    private List<FightRoundResult> _results;
    private GameObject _actor;
    private float _counter = 0f;
    private GameObject _gg;
    private GameObject _monsterAName;
    private GameObject _monsterBName;
    private GameObject _monsterAHealth;
    private GameObject _monsterBHealth;
    private List<GameObject> _damageGUI;
    //########## private #########

    //########## getter #########
    public List<FightRoundResult> Results
    {
        get
        {
            return _results;
        }
    }

    public FightRoundResult Result
    {
        get
        {
            if (_results.Count < 1) return null;
            return _results[0];
        }
        set
        {
            if (!typeof(FightRoundResult).IsInstanceOfType(value)) return;
            if ((_results.Count > 1 && _results[_results.Count - 2].Turn >= _results[_results.Count - 1].Turn)) return;
            if (value.Turn <= Turn) return;
            _results.Add(value);
        }
    }

    public TurnState GetTurnState
    {
        get
        {
            if ((Actor == null || Actor.AnimationFinished) && !BattleCamInPosition(CurrentPlayer)) return TurnState.Rotate;
            if (_results.Count == 0 && (Actor == null || Actor.AnimationFinished)) return TurnState.Wait;
            if (Actor != null && Actor.CanShowDamage) return TurnState.Hit;
            return TurnState.Execute;
        }
    }

    public bool BattleCamInPosition(FightRoundResult.Player value)
    {
        if (value == FightRoundResult.Player.A && BattleCam.transform.parent.GetComponent<RotateBattleCam>().CameraInAPosition) return true;
        if (value == FightRoundResult.Player.B && BattleCam.transform.parent.GetComponent<RotateBattleCam>().CameraInBPosition) return true;
        return false;
    }
    //########## getter ##################################################################

    void Awake()
    {
        _results = new List<FightRoundResult>();
        _damageGUI = new List<GameObject>();
        InputText = new List<int>();
    }

    public static void CreateBattle(BattleInit serverInfo) // <----------- this starts the battle
    {
        CurrentGameObject = new GameObject("BattleEngine");
        Current = CurrentGameObject.AddComponent<BattleEngine>();
        Current.Init(serverInfo);
        Current.BattleCam = Current.Arena.transform.FindChild("CameraPivot").FindChild("BattleCamera").gameObject;
        Current.BattleCam.SetActive(true);
        Current.MainCam = Camera.main.gameObject;
        Current.MainCam.SetActive(false);
        Current.CurrentPlayer = serverInfo.FirstTurnIsPlayer;
        Current.ServerInfo = serverInfo;
        if (RenderSettings.fog)
            RenderSettings.fog = false;
    }

    void Update()
    {
        updateGUI();

        if (!Fighting && Results.Count == 0)
            enforceEnd();

        if (GetTurnState != _currentStatus)
        {
            _currentStatus = GetTurnState;
            switch (GetTurnState)
            {
                case TurnState.Wait:
                    //
                    break;
                case TurnState.Rotate:
                    BattleCam.transform.parent.GetComponent<RotateBattleCam>().DoRotation = true;
                    break;
                case TurnState.Execute:
                    turnInit();
                    break;
                case TurnState.Hit:
                    executeSkill();
                    break;
            }
        }
    }

    void OnGUI()
    {
        var current = GameManager.Singleton.Player.CurCreature.slots;
        for (int i = 0; i < current.Length; i++)
        {
            string element = "";
            switch (current[i].driodenElement)
            {
                case BattleEngine.ResourceElement.Fire:
                    element = "Fire";
                    break;

                case BattleEngine.ResourceElement.Nature:
                    element = "Nature";
                    break;

                case BattleEngine.ResourceElement.Storm:
                    element = "Storm";
                    break;

                case BattleEngine.ResourceElement.Energy:
                    element = "Tech";
                    break;

                case BattleEngine.ResourceElement.Water:
                    element = "Water";
                    break;
            }
            if (GUI.Button(new Rect(0, Screen.height - 100 * (i + 1), 250, 100), (i + 1).ToString() + ". Driode. Element: " + element + ". HP: " + current[i].driodenHealth.ToString() + "%."))
            {
                InputText.Add(i);
            }
        }

        if (GUI.Button(new Rect(Screen.width - 200, 0, 200, 200), "Execute"))
        {
            switch (InputText.Count)
            {
                case 0:
                    GameManager.Singleton.FightPlayerTurn(-1, -1, -1, -1);
                    break;
                case 1:
                    GameManager.Singleton.FightPlayerTurn(InputText[0], -1, -1, -1);
                    break;
                case 2:
                    GameManager.Singleton.FightPlayerTurn(InputText[0], InputText[1], -1, -1);
                    break;
                case 3:
                    GameManager.Singleton.FightPlayerTurn(InputText[0], InputText[1], InputText[2], -1);
                    break;
                case 4:
                    GameManager.Singleton.FightPlayerTurn(InputText[0], InputText[1], InputText[2], InputText[3]);
                    break;
            }
            InputText.Clear();
        }

        string inpTxt = "";
        foreach (int number in InputText)
            inpTxt += number.ToString();

        GUI.TextArea(new Rect(Screen.width - 100, 200, 100, 100), inpTxt);

        if (GUI.Button(new Rect(Screen.width - 200, 200, 100, 100), "Delete Selection"))
            InputText.Clear();

        if (GameManager.Singleton.Player.CurFight.confused)
            GUI.Button(new Rect(250, Screen.height - 100, 100, 100), "CONFUSION!");
        if (GameManager.Singleton.Player.CurFight.defBoosted)
            GUI.Button(new Rect(250, Screen.height - 200, 100, 100), "DEF BOOSTED!");

        if (GUI.Button(new Rect(Screen.width - 200, Screen.height - 200, 200, 200), "Fliehen versuchen!"))
        {
            GameManager.Singleton.EscapeAttempt();
        }
    }

    private void updateGUI()
    {
        _damageGUI.RemoveAll(item => item == null);
        _monsterAName.transform.position = BattleCam.GetComponent<Camera>().WorldToViewportPoint(FriendlyCreature.transform.FindChild("NamePos").transform.position);
        _monsterBName.transform.position = BattleCam.GetComponent<Camera>().WorldToViewportPoint(EnemyCreature.transform.FindChild("NamePos").transform.position);
        _monsterAHealth.GetComponent<GUIText>().text = FriendlyCreature.GetComponent<MonsterController>().Health.ToString() + "/" + FriendlyCreature.GetComponent<MonsterStats>().HP;
        _monsterBHealth.GetComponent<GUIText>().text = EnemyCreature.GetComponent<MonsterController>().Health.ToString() + "/" + EnemyCreature.GetComponent<MonsterStats>().HP;
        _monsterAHealth.transform.position = BattleCam.GetComponent<Camera>().WorldToViewportPoint(FriendlyCreature.GetComponent<MonsterController>().BgHealthbar.position);
        _monsterBHealth.transform.position = BattleCam.GetComponent<Camera>().WorldToViewportPoint(EnemyCreature.GetComponent<MonsterController>().BgHealthbar.position);
    }

    private void enforceEnd()
    {
        _counter += Time.deltaTime;

        if (_counter >= 3f)
            Arena.transform.FindChild("CameraPivot").FindChild("BattleCamera").FindChild("GGScreen").gameObject.SetActive(true);

        if (_counter >= 8f)
            DestroyBattle();
    }

    private void turnInit()
    {
        Turn = Result.Turn;

        //actor -> monster
        //Execute casting animation

        if (Resources.Load("Battle/" + Result.SkillName) == null)
        {
            Debug.Log("The skill " + Result.SkillName + " does not exist. Casting Laser instead.");
            createSkillVisuals("Laser");
        }
        createSkillVisuals(Result.SkillName);
    }

    private void createSkillVisuals(string name)
    {
        _actor = Create(name, Vector3.zero, Quaternion.identity);
        Actor = _actor.GetComponent<ActorControlls>();
        Actor.Owner = this;
        switch (CurrentPlayer)
        {
            case FightRoundResult.Player.A:
                _actor.transform.localPosition = FriendlyCreature.transform.position;
                _actor.transform.LookAt(EnemyCreature.transform);
                break;
            case FightRoundResult.Player.B:
                _actor.transform.localPosition = EnemyCreature.transform.position;
                _actor.transform.LookAt(FriendlyCreature.transform);
                break;
        }
    }

    private void executeSkill()
    {
        if (CurrentPlayer == FightRoundResult.Player.A)
        {
            createDamageIndicator(EnemyCreature, Result.Damage, Result.DoT);
            if (Result.HoT > 0)
                createDamageIndicator(FriendlyCreature, Result.HoT, 0, true);

            FriendlyCreature.GetComponent<MonsterController>().Health += Result.HoT;
            EnemyCreature.GetComponent<MonsterController>().Health += -Result.Damage - Result.DoT;
            if (EnemyCreature.GetComponent<MonsterController>().Health < 0)
                EnemyCreature.GetComponent<MonsterController>().Health = 0;

        }
        else
        {
            createDamageIndicator(FriendlyCreature, Result.Damage, Result.DoT);
            if (Result.HoT > 0)
                createDamageIndicator(EnemyCreature, Result.HoT, 0, true);

            FriendlyCreature.GetComponent<MonsterController>().Health += -Result.Damage - Result.DoT;
            EnemyCreature.GetComponent<MonsterController>().Health += Result.HoT;
            if (FriendlyCreature.GetComponent<MonsterController>().Health < 0)
                FriendlyCreature.GetComponent<MonsterController>().Health = 0;
        }

        CurrentPlayer = Result.PlayerTurn;
        _results.Remove(Result);
    }

    private void createDamageIndicator(GameObject target, int damage, int dot, bool heal = false)
    {
        _damageGUI.Add(new GameObject("GUI Damage Indicator"));
        _damageGUI[_damageGUI.Count - 1].transform.localPosition = target.transform.position;
        _damageGUI[_damageGUI.Count - 1].AddComponent<DamageIndicator>();
        _damageGUI[_damageGUI.Count - 1].AddComponent<SelfDestruct>();

        if (!heal)
            _damageGUI[_damageGUI.Count - 1].AddComponent<GUIText>().text = damage.ToString() + "DMG " + Result.SkillName + " & " + dot.ToString() + "DoT ->" + Result.SkillName + "<-";
        else
            _damageGUI[_damageGUI.Count - 1].AddComponent<GUIText>().text = damage.ToString() + "HoT";

    }

    public void Init(BattleInit serverInfo)
    {
        //////////////////////////////          init Arena          //////////////////////////////////////////////////////////////////////////////////////////
        Arena = Create(DefaultArena, new Vector3(0f, 1000f, 1000f), Quaternion.identity);
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// 
        ////////////////////////////// init Friendly Creature ////////////////////////////////////////////////////////////
        string prefabName = "";
        switch (serverInfo.BaseMeshA)
        {
            case 0:
                prefabName = WolfMonster;
                break;
            case 1:
                prefabName = GiantMonster;
                break;
        }
        FriendlyCreature = Create(prefabName,
                                  Arena.transform.FindChild(DefaultFriendlySpawnPos).position,
                                  Arena.transform.FindChild(DefaultFriendlySpawnPos).rotation);

        FriendlyCreature.GetComponent<MonsterController>().StartPosition = Arena.transform.FindChild(DefaultFriendlySpawnPos).position;
        FriendlyCreature.GetComponent<MonsterController>().Owner = this;
        FriendlyCreature.GetComponent<MonsterController>().Health = serverInfo.MonsterAHealth;
        FriendlyCreature.GetComponent<MonsterStats>().Init(serverInfo.MonsterAElement, serverInfo.MonsterAName, serverInfo.MonsterALevel, serverInfo.MonsterAMaxHealth);
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// 
        ////////////////////////////// init Enemy Creature //////////////////////////////////////////////////////////////////////////////////////////
        prefabName = "";
        switch (serverInfo.BaseMeshB)
        {
            case 0:
                prefabName = WolfMonster;
                break;
            case 1:
                prefabName = GiantMonster;
                break;
        }
        EnemyCreature = Create(prefabName,
                                Arena.transform.FindChild(DefaultEnemySpawnPos).position,
                                Arena.transform.FindChild(DefaultEnemySpawnPos).rotation);

        EnemyCreature.GetComponent<MonsterController>().StartPosition = Arena.transform.FindChild(DefaultEnemySpawnPos).position;
        EnemyCreature.GetComponent<MonsterController>().Owner = this;
        EnemyCreature.GetComponent<MonsterController>().Health = serverInfo.MonsterBHealth;
        EnemyCreature.GetComponent<MonsterStats>().Init(serverInfo.MonsterBElement, serverInfo.MonsterBName, serverInfo.MonsterBLevel, serverInfo.MonsterBMaxHealth);
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// 
        /// 
        _monsterAName = new GameObject("Monster A Name");
        _monsterBName = new GameObject("Monster B Name");
        _monsterAName.AddComponent<GUIText>().text = "Lv." + serverInfo.MonsterALevel + " " + serverInfo.MonsterAName;
        _monsterBName.AddComponent<GUIText>().text = "Lv." + serverInfo.MonsterBLevel + " " + serverInfo.MonsterBName;
        _monsterAHealth = new GameObject("Monster A Health");
        _monsterBHealth = new GameObject("Monster B Health");
        _monsterAHealth.AddComponent<GUIText>();
        _monsterBHealth.AddComponent<GUIText>();
    }

    public void DestroyBattle()
    {
        if (!RenderSettings.fog)
            RenderSettings.fog = true;
        MainCam.SetActive(true);
        BattleCam.SetActive(false);
        Destroy(FriendlyCreature);
        Destroy(EnemyCreature);
        Destroy(Arena);
        Destroy(_monsterAName);
        Destroy(_monsterBName);
        Destroy(_monsterAHealth);
        Destroy(_monsterBHealth);
        Destroy(Current);
        Destroy(CurrentGameObject);
        GameManager.Singleton.SwitchGameMode(GameManager.GameMode.Map);
        Current = null;
        CurrentGameObject = null;
    }

    public GameObject Create(string name, Vector3 pos, Quaternion rot)
    {
        return (GameObject)Instantiate(Resources.Load("Battle/" + name), pos, rot);
    }
}