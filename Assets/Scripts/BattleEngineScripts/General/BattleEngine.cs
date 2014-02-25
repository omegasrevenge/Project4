using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BattleEngine : SceneRoot3D
{
    //########## public #########
    public enum TurnState { None, Wait, Execute, Hit }

    public string TestAnimation = "";
    public string TestSkillName = "";

    public bool UseTestEntries = false;

    public ActorControlls Actor;
    public FightRoundResult.Player CurrentPlayer;
    public BattleInit ServerInfo;
    [HideInInspector]
    public bool CatchInProcess = false;
    public FightRoundResult LastResult;
    public TurnState CurrentStatus = TurnState.None;

    public int Turn = 0;
    [HideInInspector]
    public bool SkipOneTurn = false;
    [HideInInspector]
    public bool FightIsOverOnce = false;

    public bool CanShowDamage = false;

    public GameObject FriendlyCreature;
    public GameObject EnemyCreature;

    public AudioSource BackgroundMusic;
    //########## public #########

    //########## static #########
    public static GameObject CurrentGameObject;
    //########## static #########

    //########## const #########
    public const string DefaultSkillOrigin = "CastFromMouthPos";
    public const string DefaultSkillTarget = "MiddleOfBody";
    public const string ArenaName = "BattleArena";
    public const string DefaultFriendlySpawnPos = "FriendlySpawnPos";
    public const string DefaultEnemySpawnPos = "EnemySpawnPos";
    public const string WolfMonster = "WolfMonster";
    public const string GiantMonster = "GiantMonster";
    //########## const #########

    //########## private #########
    [SerializeField]
    private List<string> _wolfAttackSounds;
    [SerializeField]
    private List<string> _giantAttackSounds;
    [SerializeField]
    private List<FightRoundResult> _results;
    private GameObject _actor;
    private float _counter;
    private GameObject _gg;
    private bool _initialized = false;
    private float _delay = 0f;
    private int lvl;
    //########## private #########

    //########## getter #########

	public bool Initialized
	{
		get { return (FriendlyCreature != null && EnemyCreature != null && _results != null); }
	}

    public GUIObjectBattleEngine View
    {
        get { return _gui as GUIObjectBattleEngine; }
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
        get { return _results; }
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
            if (_results.Count > 0 && _results[_results.Count - 1].Turn >= value.Turn) return;
            if (value.Turn <= Turn) return;
            _results.Add(value);
        }
    }

    public TurnState GetTurnState
    {
        get
        {
            if (_results.Count == 0 && Actor == null) return TurnState.Wait;
            if ((Actor != null && Actor.CanShowDamage) || CanShowDamage) return TurnState.Hit;
            return TurnState.Execute;
        }
    }

    public GameObject CurTarget
    {
        get { return CurrentPlayer == FightRoundResult.Player.A ? EnemyCreature : FriendlyCreature; }
    }

    public GameObject CurCaster
    {
        get { return CurrentPlayer == FightRoundResult.Player.A ? FriendlyCreature : EnemyCreature; }
    }

    //########## getter ##################################################################

    public static BattleEngine Create(BattleInit serverInfo)
    {
        CurrentGameObject = CreateObject(null, ArenaName, new Vector3(0f, 1000f, 1000f), Quaternion.identity);
        CurrentGameObject.transform.FindChild("FriendlySpawnPos").LookAt(CurrentGameObject.transform.FindChild("EnemySpawnPos"));
        CurrentGameObject.transform.FindChild("EnemySpawnPos").LookAt(CurrentGameObject.transform.FindChild("FriendlySpawnPos"));
        return CurrentGameObject.GetComponent<BattleEngine>();
    }

    public void StartFight(BattleInit serverInfo)
    {
        if (_wolfAttackSounds == null)
            _wolfAttackSounds = new List<string> { BattleSounds.WolfAttack1, BattleSounds.WolfAttack2, BattleSounds.WolfAttack3, BattleSounds.WolfAttack4 };
        if (_giantAttackSounds == null)
            _giantAttackSounds = new List<string> { BattleSounds.GiantAttack1, BattleSounds.GiantAttack2, BattleSounds.GiantAttack3, BattleSounds.GiantAttack4 };
        _delay = 5.3f;
        BackgroundMusic = SoundController.PlaySound(
            GameManager.Singleton.Player.CurrentFaction == Player.Faction.VENGEA ? 
            BattleSounds.BackgroundVengea : BattleSounds.BackgroundNce, BattleSounds.BattleSoundChannel);
        BackgroundMusic.loop = true;
        FightIsOverOnce = false;
        _counter = 0f;
        Turn = GameManager.Singleton.Player.CurFight.Round;
		_results = new List<FightRoundResult>();
		ServerInfo = serverInfo;
        View.Init();
        InitCreatures(serverInfo);
        CurrentPlayer = serverInfo.FirstTurnIsPlayer;
        if (RenderSettings.fog) RenderSettings.fog = false;
        lvl = GameManager.Singleton.Player.CurCreature.Level;
    }

    void Update()
    {
		if(!Initialized) return;

        if (!Fighting && !View.IndsArePlaying && GetTurnState == TurnState.Wait)
            enforceEnd();

        if (_delay > 0f)
        {
            _delay -= Time.deltaTime;
            return;
        }

        if (CatchInProcess) return;
        updateIdleAnim();
        if (GetTurnState == CurrentStatus
            || View.IndsArePlaying) 
            return;

        CurrentStatus = GetTurnState; 
        switch (GetTurnState)
        {
            case TurnState.Wait:
                //
                break;
            case TurnState.Execute:
                if (SkipOneTurn)
                {
                    SkipOneTurn = false;
                    Turn = Result.Turn;
                    CurrentPlayer = Result.PlayerTurn;
                    _results.Remove(Result);
                    CurrentStatus = TurnState.Wait;
                } else turnInit();
                break;
            case TurnState.Hit:
                executeSkill();
                break;
        }
    }

    private void updateIdleAnim()
    {
		float ahp = 0f;
		float bhp = 0f;
		float ahpmax = ServerInfo.MonsterAMaxHealth;
		float bhpmax = ServerInfo.MonsterBMaxHealth;
		if (LastResult == null) 
		{
			ahp = ServerInfo.MonsterAHealth;
			bhp = ServerInfo.MonsterBHealth;
		} 
		else 
		{
            ahp = LastResult.MonsterAHP;
            bhp = LastResult.MonsterBHP;
        }

		if (ahp / ahpmax < 0.33f)
            FriendlyCreature.GetComponent<MonsterAnimationController>().SetState("Hurt");
        else
            FriendlyCreature.GetComponent<MonsterAnimationController>().SetState("Hurt", false);

		if (bhp / bhpmax < 0.33f)
            EnemyCreature.GetComponent<MonsterAnimationController>().SetState("Hurt");
        else
            EnemyCreature.GetComponent<MonsterAnimationController>().SetState("Hurt", false);
    }

    private void enforceEnd()
    {
        if (!FightIsOverOnce)
        {
            if (GameManager.Singleton.Player.CurFight == null ?
                (LastResult == null ? ServerInfo.MonsterAHealth : LastResult.MonsterAHP) > 0 
                : 
                GameManager.Singleton.Player.CurCreature.HP > 0)
            {
                FriendlyCreature.GetComponent<MonsterAnimationController>().DoAnim("Victory");
                EnemyCreature.GetComponent<MonsterAnimationController>().DoAnim("Defeat");
            }
            else
            {
                FriendlyCreature.GetComponent<MonsterAnimationController>().DoAnim("Defeat");
                EnemyCreature.GetComponent<MonsterAnimationController>().DoAnim("Victory");
            }
        }

        FightIsOverOnce = true;
        _counter += Time.deltaTime;

        if (_counter >= 5f)
            View.GGContainer.Show();
    }

    private void monsterPlayAttackSound(int index)
    {
        SoundController.PlaySound(
            CurCaster.name.Contains("Wolf") ?
            _wolfAttackSounds[index] :
            _giantAttackSounds[index]
            ,
            CurCaster == FriendlyCreature ?
            BattleSounds.FriendlySoundChannel :
            BattleSounds.EnemySoundChannel);
    }

    private void turnInit()
    {
        LastResult = Result;
        Turn = Result.Turn;

        if (UseTestEntries)
        {
            int a = Convert.ToInt32(TestAnimation[TestAnimation.Length - 1]);
            if(!(a == 0 || a == 1 || a == 2 || a == 3))
            {
                Debug.LogWarning("Using Test Entries: Cannot use information provided "+TestAnimation+", "+TestSkillName+". Skipping.");
                CanShowDamage = true;
                return;
            }
            MonsterDoFullAction(CurCaster, TestAnimation, Convert.ToInt32(TestAnimation[TestAnimation.Length - 1]) - 1, TestSkillName);
            return;
        }

        if (Result.SkillName.Equals("Default"))
        {
            string element = "";
// ReSharper disable once ConditionalTernaryEqualBranch
            string model = CurCaster.name.Contains("Wolf") ? "_Scratch_Skill_Wolf" : "_Scratch_Skill_Wolf"; //<<<<<////////////////////////////////////////////////////////////
            int anim = 1;
            switch (Result.DefaultAttackElement2)
            {
                case GameManager.ResourceElement.energy:
                    anim = 2;
                    break;
                case GameManager.ResourceElement.fire:
                    anim = 3;
                    break;
                case GameManager.ResourceElement.life:
                    anim = 3;
                    break;
                case GameManager.ResourceElement.storm:
                    anim = 2;
                    break;
                case GameManager.ResourceElement.water:
                    anim = 2;
                    break;
            }
            switch (Result.DefaultAttackElement1)
            {
                case GameManager.ResourceElement.energy:
                    element = "Energy";
                    break;
                case GameManager.ResourceElement.fire:
                    element = "Fire";
                    break;
                case GameManager.ResourceElement.life:
                    element = "Life";
                    break;
                case GameManager.ResourceElement.storm:
                    element = "Storm";
                    break;
                case GameManager.ResourceElement.water:
                    element = "Water";
                    break;
            }
            MonsterDoFullAction(CurCaster, "atk_var_" + anim, anim - 1, element + model);
            return;
        }
        string animContent = Extract(Result.SkillName, "Animation");
        CurCaster.GetComponent<MonsterAnimationController>()
            .DoAnim(animContent);
        monsterPlayAttackSound(Convert.ToInt32(animContent[animContent.Length - 1]) - 1);

        if (GameManager.Singleton.Techtree[Result.SkillName] == null || 
            Resources.Load("Battle/Skill/" + Extract(Result.SkillName, "Asset_Name")) == null)
        {
            Debug.LogWarning("Skill Name: " + Result.SkillName + " <- doesn't seem to be initialized. Using Fire_Scratch_Skill_Wolf instead.");
            MonsterDoFullAction(CurCaster, "atk_var_1", 0, "Fire_Scratch_Skill_Wolf");
            return;
        }

        createSkillVisuals(Extract(Result.SkillName, "Asset_Name"));
    }

    public void MonsterDoFullAction(GameObject target, string animationName, int attackSoundIndex, string skillName)
    {
        target.GetComponent<MonsterAnimationController>().DoAnim(animationName);
        monsterPlayAttackSound(attackSoundIndex);
        createSkillVisuals(skillName);
    }

    public string Extract(string skillName, string extractionMode)
    { return (string) GameManager.Singleton.Techtree[skillName][extractionMode]; }

    private void createSkillVisuals(string objName)
    {
        _actor = CreateObject(transform, "Skill/"+objName, Vector3.zero, Quaternion.identity);
        Actor = _actor.GetComponent<ActorControlls>();
        bool conjuration = _actor.name.Contains("Conjuration");
        switch (CurrentPlayer)
        {
            case FightRoundResult.Player.A:
                if (!conjuration)
                {
                    _actor.transform.position = FriendlyCreature.transform.FindChild(DefaultSkillOrigin).position;
                    _actor.transform.LookAt(EnemyCreature.transform.FindChild(DefaultSkillTarget));
                }
                else
                {
                    _actor.transform.rotation = EnemyCreature.transform.rotation;
                    _actor.transform.position = EnemyCreature.transform.position;
                }
                break;
            case FightRoundResult.Player.B:
                if (!conjuration)
                {
                    _actor.transform.position = EnemyCreature.transform.FindChild(DefaultSkillOrigin).position;
                    _actor.transform.LookAt(FriendlyCreature.transform.FindChild(DefaultSkillTarget));
                }
                else
                {
                    _actor.transform.rotation = FriendlyCreature.transform.rotation;
                    _actor.transform.position = FriendlyCreature.transform.position;
                }
                break;
        }
    }

    private void executeSkill()
    {
        CanShowDamage = false;
        if (Result.Damage > 0 && Fighting && !Result.EVDA && !Result.EVDB)
            CurTarget.GetComponent<MonsterAnimationController>().DoAnim("Hit");

        if (!Fighting && Results.Count == 1)
        {
            FriendlyCreature.GetComponent<MonsterAnimationController>().SetState("GameOver");
            EnemyCreature.GetComponent<MonsterAnimationController>().SetState("GameOver");
        }
        if (Result.EVDA)
            FriendlyCreature.GetComponent<MonsterAnimationController>().DoAnim(Random.Range(0, 2) > 0 ? "evd_var1" : "evd_var2");
        if (Result.EVDB)
            EnemyCreature.GetComponent<MonsterAnimationController>().DoAnim(Random.Range(0, 2) > 0 ? "evd_var1" : "evd_var2");


        var info = new List<GUIObjectBattleEngine.IndicatorContent>
        { new GUIObjectBattleEngine.IndicatorContent(CurCaster, Localization.GetText(Result.SkillName), 0) };

        if (Result.Damage > 0)
            info.Add(new GUIObjectBattleEngine.IndicatorContent(CurTarget, Localization.GetText("DMG"), Result.Damage));
        if (Result.DotA && !View.MonsterADot.IsVisible)
            info.Add(new GUIObjectBattleEngine.IndicatorContent(FriendlyCreature, Localization.GetText("Set_On_Fire"), 0));
        if (Result.DotB && !View.MonsterBDot.IsVisible)
            info.Add(new GUIObjectBattleEngine.IndicatorContent(EnemyCreature, Localization.GetText("Set_On_Fire"), 0));
        if (Result.DoT > 0)
            info.Add(new GUIObjectBattleEngine.IndicatorContent(CurTarget, Localization.GetText("Burn_DMG"), Result.DoT));
        if (Result.HotA && !View.MonsterAHot.IsVisible)
            info.Add(new GUIObjectBattleEngine.IndicatorContent(FriendlyCreature, Localization.GetText("Regen"), 0));
        if (Result.HotB && !View.MonsterBHot.IsVisible)
            info.Add(new GUIObjectBattleEngine.IndicatorContent(EnemyCreature, Localization.GetText("Regen"), 0));
        if (Result.HoT > 0)
            info.Add(new GUIObjectBattleEngine.IndicatorContent(CurCaster, Localization.GetText("Heal"), Result.HoT));
        if (Result.ConA && !View.MonsterACon.IsVisible)
            info.Add(new GUIObjectBattleEngine.IndicatorContent(FriendlyCreature, Localization.GetText("Confusion"), 0));
        if (Result.ConB && !View.MonsterBCon.IsVisible)
            info.Add(new GUIObjectBattleEngine.IndicatorContent(EnemyCreature, Localization.GetText("Confusion"), 0));
        if (Result.BuffA && !View.MonsterABuff.IsVisible)
            info.Add(new GUIObjectBattleEngine.IndicatorContent(FriendlyCreature, Localization.GetText("Buff"), 0));
        if (Result.BuffB && !View.MonsterBBuff.IsVisible)
            info.Add(new GUIObjectBattleEngine.IndicatorContent(EnemyCreature, Localization.GetText("Buff"), 0));

        View.ShowDamageIndicators(info);

        View.UpdateVisualsOnSkillHit();
        CurrentPlayer = Result.PlayerTurn;
        _results.Remove(Result);
    }

    public void InitCreatures(BattleInit serverInfo)
    {
        ////////////////////////////// init Friendly Creature //////////////////////////////////////////////////////////////////////////////////////////
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

        FriendlyCreature.GetComponent<MonsterStats>().Init(serverInfo.MonsterAElement);
        
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

        EnemyCreature.GetComponent<MonsterStats>().Init(serverInfo.MonsterBElement);
    }

    public void EndBattle()
    {
        BackgroundMusic.Stop();
        if (lvl != GameManager.Singleton.Player.CurCreature.Level)
            GameManager.Singleton.LevelUp();
        SoundController.RemoveChannel(BattleSounds.BattleSoundChannel);
        SoundController.RemoveChannel(BattleSounds.EnemySoundChannel);
        SoundController.RemoveChannel(BattleSounds.FriendlySoundChannel);
        Destroy(ServerInfo.gameObject);
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