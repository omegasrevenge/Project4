using UnityEngine;
using System.Collections.Generic;

public class BattleEngine : SceneRoot3D
{
    //########## public #########
    public enum TurnState { Wait, Execute, Hit }


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
    private bool _enactEndScreen = false;
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

    public GameObject CurTarget
    {
        get
        {
            return CurrentPlayer == FightRoundResult.Player.A ? EnemyCreature : FriendlyCreature;
        }
    }

    public GameObject CurCaster
    {
        get
        {
            return CurrentPlayer == FightRoundResult.Player.A ? FriendlyCreature : EnemyCreature;
        }
    }

    //########## getter ##################################################################

    public static BattleEngine Create(BattleInit serverInfo)
    {
        CurrentGameObject = CreateObject(null, DefaultArena, new Vector3(0f, 1000f, 1000f), Quaternion.identity);
        CurrentGameObject.transform.FindChild("FriendlySpawnPos").LookAt(CurrentGameObject.transform.FindChild("EnemySpawnPos"));
        CurrentGameObject.transform.FindChild("EnemySpawnPos").LookAt(CurrentGameObject.transform.FindChild("FriendlySpawnPos"));
        return CurrentGameObject.GetComponent<BattleEngine>();
    }

    public void StartFight(BattleInit serverInfo)
    {
        _counter = 0f;
        Turn = GameManager.Singleton.Player.CurFight.Round;
		_results = new List<FightRoundResult>();
		ServerInfo = serverInfo;
        View.Init();
        InitCreatures(serverInfo);
        CurrentPlayer = serverInfo.FirstTurnIsPlayer;
        if (RenderSettings.fog) RenderSettings.fog = false;
    }

    void Update()
    {
		if(!Initialized) return;
        if (!Fighting && !View.IndsArePlaying && (Actor == null || Actor.AnimationFinished))
            enforceEnd();

        if (GetTurnState == _currentStatus 
            || _enactEndScreen 
            || View.IndsArePlaying) 
            return;

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
                if (!Fighting)
                    _enactEndScreen = true;
                break;
        }
    }

    private void enforceEnd()
    {
        _counter += Time.deltaTime;

        if (_counter >= 2f)
            View.GGContainer.Show();
    }

    private void turnInit()
    {
        Turn = Result.Turn;

        CurCaster.GetComponent<MonsterAnimationController>()
            .DoAnim(Extract(Result.SkillName, "Animation"));

        if (Resources.Load("Battle/" + Extract(Result.SkillName, "Asset_Name")) == null)
        {
            Debug.LogError("The skill >" + Result.SkillName + "< does not exist in Techtree. Casting Laser instead.");
            createSkillVisuals("Laser");
            return;
        }
        createSkillVisuals(Extract(Result.SkillName, "Asset_Name"));
    }

    public string Extract(string skillName, string info)
    {
		if (GameManager.Singleton.Techtree [skillName] == null)
		    return info.Equals("Animation") ? "atk_var_1" : "Laser";
        return (string)GameManager.Singleton.Techtree[skillName][info];
    }

    private void createSkillVisuals(string name)
    {
        _actor = CreateObject(transform, name, Vector3.zero, Quaternion.identity);
        Actor = _actor.GetComponent<ActorControlls>();
        Actor.Owner = this;

        switch (CurrentPlayer)
        {
            case FightRoundResult.Player.A:
                _actor.transform.position = FriendlyCreature.transform.FindChild("CastFromMouthPos").position;
                _actor.transform.rotation = FriendlyCreature.transform.FindChild("CastFromMouthPos").rotation;
                break;
            case FightRoundResult.Player.B:
                _actor.transform.position = EnemyCreature.transform.FindChild("CastFromMouthPos").position;
                _actor.transform.rotation = EnemyCreature.transform.FindChild("CastFromMouthPos").rotation;
                break;
        }
    }

    private void executeSkill()
    {
        if (Result.Damage > 0)
            CurTarget.GetComponent<MonsterAnimationController>().DoAnim("Hit");

        var info = new List<GUIObjectBattleEngine.IndicatorContent>
        {
            new GUIObjectBattleEngine.IndicatorContent(CurCaster, Result.SkillName, 0)
        };

        if (Result.Damage > 0)
            info.Add(new GUIObjectBattleEngine.IndicatorContent(CurTarget, "DMG", Result.Damage));
        if (Result.DotA && !View.MonsterADot.IsVisible)
            info.Add(new GUIObjectBattleEngine.IndicatorContent(FriendlyCreature, "Set on fire!", 0));
        if (Result.DotB && !View.MonsterBDot.IsVisible)
            info.Add(new GUIObjectBattleEngine.IndicatorContent(EnemyCreature, "Set on fire!", 0));
        if (Result.DoT > 0)
            info.Add(new GUIObjectBattleEngine.IndicatorContent(CurTarget, "Burn DMG", Result.DoT));
        if (Result.HotA && !View.MonsterAHot.IsVisible)
            info.Add(new GUIObjectBattleEngine.IndicatorContent(FriendlyCreature, "Regenerating!", 0));
        if (Result.HotB && !View.MonsterBHot.IsVisible)
            info.Add(new GUIObjectBattleEngine.IndicatorContent(EnemyCreature, "Regenerating!", 0));
        if (Result.HoT > 0)
            info.Add(new GUIObjectBattleEngine.IndicatorContent(CurCaster, "Heal", Result.HoT));
        if (Result.ConA && !View.MonsterACon.IsVisible)
            info.Add(new GUIObjectBattleEngine.IndicatorContent(FriendlyCreature, "Confusion!", 0));
        if (Result.ConB && !View.MonsterBCon.IsVisible)
            info.Add(new GUIObjectBattleEngine.IndicatorContent(EnemyCreature, "Confusion!", 0));
        if (Result.BuffA && !View.MonsterABuff.IsVisible)
            info.Add(new GUIObjectBattleEngine.IndicatorContent(FriendlyCreature, "Buffed!", 0));
        if (Result.BuffB && !View.MonsterBBuff.IsVisible)
            info.Add(new GUIObjectBattleEngine.IndicatorContent(EnemyCreature, "Buffed!", 0));

        View.ShowDamageIndicators(info);

        View.UpdateVisualsOnSkillHit();
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
        _enactEndScreen = false;
        _results.Clear();
        View.GGContainer.Hide();
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
}