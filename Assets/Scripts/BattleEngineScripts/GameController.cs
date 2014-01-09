using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	
	void Update()
	{
		
		if(Input.GetKeyDown(KeyCode.A))
		{
			BattleEngine.CreateBattle(new BattleInit());
		}

		if(Input.GetKeyDown(KeyCode.Space))
		{
			if(BattleEngine.Current == null) return;
			BattleEngine.Current.Result = new FightRoundResult();
			BattleEngine.Current.Result.Turn += BattleEngine.Current.Turn;
		}
	}
}
