using UnityEngine;
using System.Collections.Generic;

public class BattleEngine : MonoBehaviour 
{
	//########## public #########
	public FightRoundResult Result;
	public ActorControlls Actor;
	public int Turn = 0;
	//########## public #########
	
	//########## static #########
	public static BattleEngine Current;
	public static GameObject   CurrentGameObject;
	//########## static #########
	
	//########## const #########
	public const string DefaultArena			 = "DefaultArena";
	public const string DefaultFriendlySpawnPos  = "FriendlySpawnPos";
	public const string DefaultEnemySpawnPos	 = "EnemySpawnPos";
	public const string DefaultMonster	 		 = "DefaultMonster";
	//########## const #########

	//########## private #########
	private GameObject _friendlyCreature;
	private GameObject _enemyCreature;
	private GameObject _arena;
	private FightRoundResult.Player _currentPlayer; // whose player's turn is it going to be now
	private int _changeA;
	private int _changeB;
	public GameObject _actor;
	private GameObject _damageIndicator;
	private bool _damageIndicatorInitialized = false;
	//########## private #########
	
	public static void CreateBattle(BattleInit serverInfo) // <----------- this starts the battle
	{
		CurrentGameObject = new GameObject("BattleEngine");
		Current = CurrentGameObject.AddComponent<BattleEngine>();
		Current.Init(serverInfo);
	}

	void Update()
	{
		evaluateResult();
	}

	private void evaluateResult()
	{
		
		if(Result != null && Turn < Result.Turn && (Actor == null || Actor.AnimationFinished))
		{
			// init new turn NOW.
			_currentPlayer = Result.PlayerTurn;
			_damageIndicatorInitialized = false;
			Turn = Result.Turn;
			initSkill();
			_changeA = _friendlyCreature.GetComponent<MonsterController>().Health-Result.PlayerAHealth;
			_changeB = _enemyCreature   .GetComponent<MonsterController>().Health-Result.PlayerBHealth;
		}
		if(Actor != null && !Actor.AnimationFinished)
		{
			executeSkill();
		}
	}

	private void initSkill()
	{
		//actor -> monster
		//Execute casting animation
		//when done do
		//evalute which skill was used by Result.SkillID
		if(Result.SkillID == 1)
		{
			_actor = (GameObject)Instantiate(Resources.Load("Laser"),Vector3.zero,Quaternion.identity);
			Actor = _actor.GetComponent<ActorControlls>();
			Actor.Owner = this;
			switch(_currentPlayer)
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
		if(Actor.CanShowDamage)
		{
			if(_damageIndicator == null && !_damageIndicatorInitialized)
			{
				_damageIndicatorInitialized = true;
				_damageIndicator = (GameObject)Instantiate(Resources.Load("Damage"),Vector3.zero,Quaternion.identity);
				switch(_currentPlayer)
				{
				case FightRoundResult.Player.A:
					_damageIndicator.transform.localPosition = _enemyCreature.transform.position;
					_damageIndicator.transform.rotation = _enemyCreature.transform.rotation;
					break;
				case FightRoundResult.Player.B:
					_damageIndicator.transform.localPosition = _friendlyCreature.transform.position;
					_damageIndicator.transform.rotation = _friendlyCreature.transform.rotation;
					break;
				}
			}
			_friendlyCreature.GetComponent<MonsterController>().Health = Result.PlayerAHealth;
			_enemyCreature.GetComponent<MonsterController>().Health    = Result.PlayerBHealth;
		}
	}

	public void Init(BattleInit serverInfo)
	{
		//////////////////////////////          init Arena          //////////////////////////////////////////////////////////////////////////////////////////
		_arena = Create(DefaultArena, new Vector3(0f,-6.462654f,9.857271f), Quaternion.identity);
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

	private void destroyBattle()
	{
		Destroy(_friendlyCreature);
		Destroy(_enemyCreature);
		Destroy(_arena);
		Destroy(Current);
		Current = null;
	}
	
	public GameObject Create(string value, Vector3 pos, Quaternion rot)
	{
		return (GameObject)Instantiate(Resources.Load(value), pos, rot);
	}
}
