using UnityEngine;
using System.Collections;

public class BattleEngine : MonoBehaviour 
{
	private GameObject _friendlyCreature;
	private GameObject _enemyCreature;
	private GameObject _arena;


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

	public void CreateArena(string arenaName = "DefaultArena", string friendlyCreature = "DefaultFriendly", string enemyCreature = "DefaultEnemy")
	{
		_arena = Create(arenaName, Vector3.zero, Quaternion.identity);
		_friendlyCreature = Create(friendlyCreature, 
		                           _arena.transform.FindChild("FriendlySpawnPos").position, 
		                           _arena.transform.FindChild("FriendlySpawnPos").rotation);

		_friendlyCreature.GetComponent<CreatureControlls>().StartPosition = _arena.transform.FindChild("FriendlySpawnPos");
		_friendlyCreature.GetComponent<CreatureControlls>().Owner = this;

		_enemyCreature = Create(enemyCreature, 
		                        _arena.transform.FindChild("EnemySpawnPos").position, 
		                        _arena.transform.FindChild("EnemySpawnPos").rotation);
		_enemyCreature.GetComponent<CreatureControlls>().StartPosition = _arena.transform.FindChild("EnemySpawnPos");
	}

	private GameObject Create(string value, Vector3 pos, Quaternion rot)
	{
		return (GameObject)Instantiate(Resources.Load(value), pos, rot);
	}

	public void ExecuteAttack(GameObject source, GameObject target)
	{
		source.GetComponent<CreatureControlls>().Attack(target.transform);
	}
}
