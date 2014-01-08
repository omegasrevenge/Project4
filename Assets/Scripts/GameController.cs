using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	
	void Start()
	{
		BattleEngine.CreateBattle(new BattleInit());
		//BattleEngine.Current.ExecuteAttack(BattleEngine.Current.FriendlyCreature, 
		//                                   BattleEngine.Current.EnemyCreature);
	}


}
