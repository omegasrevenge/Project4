using UnityEngine;
using System.Collections.Generic;

public class BattleEngine : MonoBehaviour 
{
	//########## public #########
	public enum TurnState{Wait, Rotate, Execute, Hit}
	public GameObject BattleCam;
	public GameObject MainCam;
	public ActorControlls Actor;
	public int Turn = 0;
	public FightRoundResult.Player CurrentPlayer; // whose player's turn is it going to be now
	public BattleInit ServerInfo;
	public List<string> TEST;
	//########## public #########
	
	//########## static #########
	public static BattleEngine Current;
	public static GameObject   CurrentGameObject;
	//########## static #########
	
	//########## const #########
	public const string DefaultArena			 = "Battle/DefaultArena";
	public const string DefaultFriendlySpawnPos  = "FriendlySpawnPos";
	public const string DefaultEnemySpawnPos	 = "EnemySpawnPos";
	public const string DefaultMonster	 		 = "Battle/DefaultMonster";
	//########## const #########

	//########## private #########
	private TurnState _currentStatus = TurnState.Wait;
	private List<FightRoundResult> _results;
	private GameObject _friendlyCreature;
	private GameObject _enemyCreature;
	private GameObject _arena;
	private int _changeA;
	private int _changeB;
	private GameObject _actor;
	private GameObject _damageIndicator;
	private float _counter = 0f;
	private GameObject _gg;
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
			if(_results.Count < 1) return null;
			return _results[0];
		}
		set
		{
			_results.Add(value);
		}
	}
	
	public GameObject Arena
	{
		get
		{
			return _arena;
		}
	}
	
	public GameObject Friendly
	{
		get
		{
			return _friendlyCreature;
		}
	}
	
	public GameObject Enemy
	{
		get
		{
			return _enemyCreature;
		}
	}

	public bool TurnIsDone
	{
		get
		{
			return (Actor == null || Actor.AnimationFinished); 
		}
	}

	public bool IsResultForNextTurnThere
	{
		get
		{
			if(Result == null) return false;
			return Turn < Result.Turn;
		}
	}

	public bool IsItTheOtherPlayersTurnNow
	{
		get
		{
			if(Result == null) return false;
			return CurrentPlayer != Result.PlayerTurn;
		}
	}

	public bool IsBattleCamRotating
	{
		get
		{
			return BattleCam.transform.parent.GetComponent<RotateBattleCam>().DoRotation;
		}
	}

	public TurnState GetTurnState
	{
		get
		{
			if(TurnIsDone && !BattleCamInPosition(CurrentPlayer)) return TurnState.Rotate;
			if(_results.Count == 0 && (Actor == null || Actor.AnimationFinished)) return TurnState.Wait;
			if(Actor != null && Actor.CanShowDamage) return TurnState.Hit;
			return TurnState.Execute;
		}
	}

	public bool BattleCamInPosition(FightRoundResult.Player value)
	{
		if(value == FightRoundResult.Player.A && BattleCam.transform.parent.GetComponent<RotateBattleCam>().CameraInAPosition) return true;
		if(value == FightRoundResult.Player.B && BattleCam.transform.parent.GetComponent<RotateBattleCam>().CameraInBPosition) return true;
		return false;
	}
	//########## getter ##################################################################

	void Awake()
	{
		_results = new List<FightRoundResult>();
		TEST = new List<string>();
	}

	public static void CreateBattle(BattleInit serverInfo) // <----------- this starts the battle
	{
		CurrentGameObject = new GameObject("BattleEngine");
		Current = CurrentGameObject.AddComponent<BattleEngine>();
		Current.Init(serverInfo);
		Current.BattleCam = Current.Arena.transform.FindChild("CameraPivot").FindChild("BattleCamera").gameObject;
		Current.BattleCam.SetActive(true);
		Current.MainCam = GameObject.Find("Main Camera");
		Current.MainCam.SetActive(false);
		Destroy(Current.MainCam.GetComponent<AudioListener>());
		Current.CurrentPlayer = serverInfo.FirstTurnIsPlayer;
		Current.ServerInfo = serverInfo;
	}

	void Update()
	{
		if(Friendly.GetComponent<MonsterController>().Health <= 0 || Enemy.GetComponent<MonsterController>().Health <= 0)
		{
			if(_gg == null) _gg = Create("Battle/GGScreen", GameObject.Find("GGScreenPos").transform.position, GameObject.Find("GGScreenPos").transform.rotation);
			_counter += Time.deltaTime;
			if(_counter >= 3f) { Destroy(_gg); DestroyBattle(); }
			return;
		}

		if(GetTurnState != _currentStatus)
		{
			_currentStatus = GetTurnState;
			switch(GetTurnState)
			{
			case TurnState.Wait:
				break;
			case TurnState.Rotate:
				rotateBattleCam();
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

	private void rotateBattleCam()
	{
		BattleCam.transform.parent.GetComponent<RotateBattleCam>().DoRotation = true;
	}

	private void turnInit()
	{
		Turn = Result.Turn;
		initSkill();
		_changeA = _friendlyCreature.GetComponent<MonsterController>().Health-Result.PlayerAHealth;
		_changeB = _enemyCreature   .GetComponent<MonsterController>().Health-Result.PlayerBHealth;
	}

	private void initSkill()
	{
		//actor -> monster
		//Execute casting animation
		//when done do
		//evalute which skill was used by Result.SkillID
		if(Result.SkillID == 1)
		{
			_actor = (GameObject)Instantiate(Resources.Load("Battle/Laser"),Vector3.zero,Quaternion.identity);
			Actor = _actor.GetComponent<ActorControlls>();
			Actor.Owner = this;
			switch(CurrentPlayer)
			{
			case FightRoundResult.Player.A:
				_actor.transform.localPosition = _friendlyCreature.transform.position;
				_actor.transform.LookAt(_enemyCreature.transform);
				break;
			case FightRoundResult.Player.B:
				_actor.transform.localPosition = _enemyCreature.transform.position;
				_actor.transform.LookAt(_friendlyCreature.transform);
				break;
			}
		}
	}

	private void executeSkill()
	{
		_damageIndicator = (GameObject)Instantiate(Resources.Load("Battle/Damage"),Vector3.zero,Quaternion.identity);
		switch(CurrentPlayer)
		{
		case FightRoundResult.Player.A:
			_damageIndicator.transform.localPosition = _enemyCreature.transform.position;
			break;
		case FightRoundResult.Player.B:
			_damageIndicator.transform.localPosition = _friendlyCreature.transform.position;
			break;
		}
		_friendlyCreature.GetComponent<MonsterController>().Health = Result.PlayerAHealth;
		_enemyCreature.GetComponent<MonsterController>().Health    = Result.PlayerBHealth;
		CurrentPlayer = Result.PlayerTurn;
		_results.Remove(Result);
	}

	public void Init(BattleInit serverInfo)
	{
		//////////////////////////////          init Arena          //////////////////////////////////////////////////////////////////////////////////////////
		_arena = Create(DefaultArena, new Vector3(0f,1000f,1000f), Quaternion.identity);
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/// 
		////////////////////////////// init Friendly Creature ////////////////////////////////////////////////////////////
		_friendlyCreature = Create(DefaultMonster, 
		                           _arena.transform.FindChild(DefaultFriendlySpawnPos).position, 
		                           _arena.transform.FindChild(DefaultFriendlySpawnPos).rotation);
		
		_friendlyCreature.GetComponent<MonsterController>().StartPosition = _arena.transform.FindChild(DefaultFriendlySpawnPos).position;
		_friendlyCreature.GetComponent<MonsterController>().Owner = this;
		_friendlyCreature.GetComponent<MonsterController>().Health = serverInfo.MonsterAHealth;
		_friendlyCreature.GetComponent<MonsterStats>().Init(serverInfo.MonsterAElement, serverInfo.MonsterAName, serverInfo.MonsterALevel, serverInfo.MonsterAHealth);
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/// 
		////////////////////////////// init Enemy Creature //////////////////////////////////////////////////////////////////////////////////////////
		_enemyCreature = Create(DefaultMonster, 
		                        _arena.transform.FindChild(DefaultEnemySpawnPos).position, 
		                        _arena.transform.FindChild(DefaultEnemySpawnPos).rotation);
		
		_enemyCreature.GetComponent<MonsterController>().StartPosition = _arena.transform.FindChild(DefaultEnemySpawnPos).position;
		_enemyCreature.GetComponent<MonsterController>().Owner = this;
		_enemyCreature.GetComponent<MonsterController>().Health = serverInfo.MonsterBHealth;
		_enemyCreature.GetComponent<MonsterStats>().Init(serverInfo.MonsterBElement, serverInfo.MonsterBName, serverInfo.MonsterBLevel, serverInfo.MonsterBHealth);
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	}

	public void DestroyBattle()
	{
		MainCam.SetActive(true);
		BattleCam.SetActive(false);
		MainCam.AddComponent<AudioListener>();
		Destroy(_friendlyCreature);
		Destroy(_enemyCreature);
		Destroy(_arena);
		Destroy(Current);
		Destroy(CurrentGameObject);
		Current = null;
		CurrentGameObject = null;
	}
	
	public GameObject Create(string name, Vector3 pos, Quaternion rot)
	{
		return (GameObject)Instantiate(Resources.Load(name), pos, rot);
	}
}
