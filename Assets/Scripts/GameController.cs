using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	private BattleEngine _battleEngine;
	
	void Start()
	{
		_battleEngine = GetComponent<BattleEngine>();
		_battleEngine.CreateBattle(new BattleInit());
		_battleEngine.ExecuteAttack(_battleEngine.FriendlyCreature, _battleEngine.EnemyCreature);
	}


}
