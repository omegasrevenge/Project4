using UnityEngine;
using System.Collections.Generic;

public class BattleEngine : MonoBehaviour 
{

	public FightRoundResult Result;
	public List<ActorControlls> Actors;
	public int Turn = 0;

	private GameObject _friendlyCreature;
	private GameObject _enemyCreature;
	private GameObject _arena;
	private FightRoundResult.Player _nextPlayer;
	private bool _resultEvaluated = false;
	
	public const string DefaultArena			 = "DefaultArena";
	public const string DefaultFriendly 		 = "DefaultFriendly";
	public const string DefaultEnemy 			 = "DefaultEnemy";
	public const string DefaultFriendlySpawnPos  = "FriendlySpawnPos";
	public const string DefaultEnemySpawnPos	 = "EnemySpawnPos";

	void Awake()
	{
		Actors = new List<ActorControlls>();
	}

	void Update()
	{
		if(Result != null)
		{
			if(!_resultEvaluated)
			{
				_nextPlayer = Result.PlayerTurn;

				int changeA = FriendlyCreature.GetComponent<ActorControlls>().Health-Result.PlayerAHealth;
				int changeB = EnemyCreature   .GetComponent<ActorControlls>().Health-Result.PlayerBHealth;
				
				FriendlyCreature.GetComponent<ActorControlls>().Health = Result.PlayerAHealth; // has to be changed in the future, your creature doesnt have to belong to playerA
				EnemyCreature.GetComponent<ActorControlls>().Health    = Result.PlayerBHealth;
				_resultEvaluated = true;
			}

			bool allDone = true;
			foreach(ActorControlls actor in Actors)
			{
				if(actor.CurrentPhase != ActorControlls.Phase.None)
				{
					allDone = false;
					break;
				}
			}
			if(allDone)
			{
				Actors.Clear();
				if(Turn != Result.Turn)
				{ 
					Debug.Log("Turn not synchronized. BattleEngine.Turn = "+Turn+", Result.Turn = "+Result.Turn); 
					Turn = Result.Turn;
				}
				Turn++;
				Destroy(Result);
				_resultEvaluated = false;
			}
		}
	}

	public GameObject FriendlyCreature
	{
		get
		{
			if(_friendlyCreature == null) 
			{
				Debug.Log("FriendlyCreature request, return null.");
				_friendlyCreature = Create("DefaultFriendly", Vector3.zero, Quaternion.identity);
			}
			return _friendlyCreature;
		}
		set
		{
			if(_friendlyCreature != null) Destroy(_friendlyCreature);
			_friendlyCreature = value;
		}
	}
	
	public GameObject EnemyCreature
	{
		get
		{
			if(_enemyCreature == null) 
			{
				Debug.Log("EnemyCreature request, return null.");
				_enemyCreature = Create("DefaultEnemy", Vector3.zero, Quaternion.identity);
			}
			return _enemyCreature;
		}
		set
		{
			if(_enemyCreature != null) Destroy(_enemyCreature);
			_enemyCreature = value;
		}
	}

	public GameObject Arena
	{
		get
		{
			if(_arena == null) 
			{
				Debug.Log("Arena request, return null.");
				CreateArena();
			}
			return _arena;
		}
		set
		{
			if(_arena != null) Destroy(_arena);
			_arena = value;
		}
	}

	public void CreateArena(string arenaName = DefaultArena, string friendlyCreature = DefaultFriendly, string enemyCreature = DefaultEnemy)
	{
		//////////////////////////////
		_arena = Create(arenaName, Vector3.zero, Quaternion.identity);
		//////////////////////////////
		_friendlyCreature = Create(friendlyCreature, 
		                           _arena.transform.FindChild(DefaultFriendlySpawnPos).position, 
		                           _arena.transform.FindChild(DefaultFriendlySpawnPos).rotation);

		_friendlyCreature.GetComponent<ActorControlls>().StartPosition = _arena.transform.FindChild(DefaultFriendlySpawnPos).position;
		_friendlyCreature.GetComponent<ActorControlls>().Owner = this;
		//////////////////////////////
		_enemyCreature = Create(enemyCreature, 
		                        _arena.transform.FindChild(DefaultEnemySpawnPos).position, 
		                        _arena.transform.FindChild(DefaultEnemySpawnPos).rotation);

		_enemyCreature.GetComponent<ActorControlls>().StartPosition = _arena.transform.FindChild(DefaultEnemySpawnPos).position;
		_enemyCreature.GetComponent<ActorControlls>().Owner = this;
		//////////////////////////////
	}

	private GameObject Create(string value, Vector3 pos, Quaternion rot)
	{
		return (GameObject)Instantiate(Resources.Load(value), pos, rot);
	}

	public void ExecuteAttack(GameObject source, GameObject target)
	{
		Actors.Add(source.GetComponent<ActorControlls>());
		source.GetComponent<ActorControlls>().Attack(target.transform.position);
	}
}
