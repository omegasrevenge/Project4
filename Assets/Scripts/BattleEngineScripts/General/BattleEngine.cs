using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class BattleEngine : SceneRoot3D
{
    //########## public #########
    public enum TurnState { Wait, Execute, Hit }
    public enum ResourceElement { None = -1, Energy, Fire, Storm, Nature, Water };


    public ActorControlls Actor;
    public FightRoundResult.Player CurrentPlayer;
    public BattleInit ServerInfo;


    public int Turn = 0;


    public GameObject FriendlyCreature;
    public GameObject EnemyCreature;
    //########## public #########

    //########## static #########
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
    private float _counter;
    private GameObject _gg;
    private bool _initialized = false;
    //########## private #########

    //########## getter #########

	public bool Initialized
	{
		get
		{
			return (FriendlyCreature != null && EnemyCreature != null && _results != null);
		}
	}

    public GUIObjectBattleEngine View
    {
        get
        {
            return _gui as GUIObjectBattleEngine;
        }
    }

    public bool Fighting
    {
        get
        {
            if (GameManager.Singleton.Player.CurFight == null)
                return false;
            return !GameManager.Singleton.Player.CurFight.Finished;
        }
    }

    public static BattleEngine Current
    {
        get 
        {
            if (CurrentGameObject == null) Debug.LogError("BattleEngine GameObject null, although in use!");
            return CurrentGameObject.GetComponent<BattleEngine>();
        }
    }

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
            if (value == null) return;
            if ((_results.Count > 1 && _results[_results.Count - 2].Turn >= _results[_results.Count - 1].Turn)) return;
            if (value.Turn <= Turn) return;
            _results.Add(value);
        }
    }

    public TurnState GetTurnState
    {
        get
        {
            if (_results.Count == 0 && (Actor == null || Actor.AnimationFinished)) return TurnState.Wait;
            if (Actor != null && Actor.CanShowDamage) return TurnState.Hit;
            return TurnState.Execute;
        }
    }
    //########## getter ##################################################################

    public static BattleEngine Create(BattleInit serverInfo) // <----------- this starts the battle
    {
        CurrentGameObject = CreateObject(null, DefaultArena, new Vector3(0f, 1000f, 1000f), Quaternion.identity);
        return CurrentGameObject.GetComponent<BattleEngine>();
    }

    public void StartFight(BattleInit serverInfo)
    {
        Turn = GameManager.Singleton.Player.CurFight.Round;
		_results = new List<FightRoundResult>();
		ServerInfo = serverInfo;
        if (!View.Initialized) 
            View.Init();
        InitCreatures(serverInfo);
        CurrentPlayer = serverInfo.FirstTurnIsPlayer;
        if (RenderSettings.fog) RenderSettings.fog = false;
    }

    void Update()
    {
		if(!Initialized) return;
        if (!Fighting)
            enforceEnd();

        if (GetTurnState == _currentStatus) return;
        _currentStatus = GetTurnState;
        switch (GetTurnState)
        {
            case TurnState.Wait:
                //
                break;
            case TurnState.Execute:
                turnInit();
                break;
            case TurnState.Hit:
                executeSkill();
                break;
        }
    }

    private void enforceEnd()
    {
        _counter += Time.deltaTime;

        if (_counter >= 3f)
            Camera.transform.FindChild("GGScreen").gameObject.SetActive(true);

        if (_counter >= 8f)
        {
            EndBattle();
            GameManager.Singleton.SwitchGameMode(GameManager.GameMode.Map);
        }
    }

    private void turnInit()
    {
        Turn = Result.Turn;

        //actor -> monster
        //Execute casting animation

        if (Resources.Load("Battle/" + Result.SkillName) == null)
        {
            Debug.LogError("The skill >" + Result.SkillName + "< does not exist. Casting Laser instead.");
            createSkillVisuals("Laser");
            return;
        }
        createSkillVisuals(Result.SkillName);
    }

    private void createSkillVisuals(string name)
    {
        _actor = CreateObject(transform, name, Vector3.zero, Quaternion.identity);
        Actor = _actor.GetComponent<ActorControlls>();
        Actor.Owner = this;
        switch (CurrentPlayer)
        {
            case FightRoundResult.Player.A:
                _actor.transform.position = FriendlyCreature.transform.position;
                _actor.transform.LookAt(EnemyCreature.transform);
                break;
            case FightRoundResult.Player.B:
                _actor.transform.position = EnemyCreature.transform.position;
                _actor.transform.LookAt(FriendlyCreature.transform);
                break;
        }
    }

    private void executeSkill()
    {
        if (CurrentPlayer == FightRoundResult.Player.A)
        {
            View.CreateDamageIndicator(EnemyCreature, Result.Damage, Result.DoT);
            if (Result.HoT > 0)
                View.CreateDamageIndicator(FriendlyCreature, Result.HoT, 0, true);
        }
        else
        {
            View.CreateDamageIndicator(FriendlyCreature, Result.Damage, Result.DoT);
            if (Result.HoT > 0)
                View.CreateDamageIndicator(EnemyCreature, Result.HoT, 0, true);
        }

        CurrentPlayer = Result.PlayerTurn;
        _results.Remove(Result);
    }

    public void InitCreatures(BattleInit serverInfo)
    {
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
        FriendlyCreature = CreateObject(transform, prefabName,
                                  transform.FindChild(DefaultFriendlySpawnPos).position,
                                  transform.FindChild(DefaultFriendlySpawnPos).rotation);

        FriendlyCreature.GetComponent<MonsterController>().StartPosition = transform.FindChild(DefaultFriendlySpawnPos).position;
        FriendlyCreature.GetComponent<MonsterController>().Owner = this;
        FriendlyCreature.GetComponent<MonsterStats>().Init(serverInfo.MonsterAElement);
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
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
        EnemyCreature = CreateObject(transform, prefabName,
                                transform.FindChild(DefaultEnemySpawnPos).position,
                                transform.FindChild(DefaultEnemySpawnPos).rotation);

        EnemyCreature.GetComponent<MonsterController>().StartPosition = transform.FindChild(DefaultEnemySpawnPos).position;
        EnemyCreature.GetComponent<MonsterController>().Owner = this;
        EnemyCreature.GetComponent<MonsterStats>().Init(serverInfo.MonsterBElement);
    }

    public void EndBattle()
    {
        _results.Clear();
        Camera.transform.FindChild("GGScreen").gameObject.SetActive(false);
        if (!RenderSettings.fog)
            RenderSettings.fog = true;
        Destroy(FriendlyCreature);
        Destroy(EnemyCreature);
    }

    public static GameObject CreateObject(Transform root, string name, Vector3 pos, Quaternion rot)
    {
        GameObject obj = (GameObject)Instantiate(Resources.Load("Battle/" + name), pos, rot);
        if (root)
            obj.transform.parent = root;
        return obj;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (FriendlyCreature != null)
            EndBattle();
    }
}