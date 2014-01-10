using UnityEngine;
using System.Collections;

public class TESTBATTLEENGINE : MonoBehaviour {

	
	void Update()
	{
		
		if(Input.GetKeyDown(KeyCode.A))
		{
			if(BattleEngine.Current == null)
			{
				BattleEngine.CreateBattle(new BattleInit());
				Debug.Log("BattleEngine Created");
			}
			else
			{
				BattleEngine.Current.DestroyBattle();
				Debug.Log("BattleEngine Destroyed");
			}
		}

		if(Input.GetKeyDown(KeyCode.Space))
		{
			if(BattleEngine.Current == null) return;
			BattleEngine.Current.Result = new FightRoundResult();
			BattleEngine.Current.Result.Turn += BattleEngine.Current.Turn;
		}
	}
}
