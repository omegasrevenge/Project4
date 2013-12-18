using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	private BattleEngine _battleEngine;
	
	void Start()
	{
		_battleEngine = GetComponent<BattleEngine>();
		_battleEngine.CreateArena();
		_battleEngine.ExecuteAttack(_battleEngine.FriendlyCreature, _battleEngine.EnemyCreature);
	}


}
