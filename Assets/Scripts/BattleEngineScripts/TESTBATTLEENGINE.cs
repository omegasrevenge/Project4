using UnityEngine;
using System.Collections;

public class TESTBATTLEENGINE : MonoBehaviour {

	private int _counter = 0;
	
	void Update()
	{
		
		if(Input.GetKeyDown(KeyCode.A))
		{
			if(BattleEngine.Current == null)
			{
				BattleEngine.CreateBattle(new BattleInit());
			}
			else
			{
				BattleEngine.Current.DestroyBattle();
			}
		}

		if(Input.GetKeyDown(KeyCode.Space))
		{
			if(BattleEngine.Current == null) return;
			BattleEngine.Current.Result = new FightRoundResult();
			if(BattleEngine.Current.Results.Count > 1) 
			{
				if(BattleEngine.Current.Results[BattleEngine.Current.Results.Count-2].PlayerTurn == FightRoundResult.Player.A)
				{
					BattleEngine.Current.Results[BattleEngine.Current.Results.Count-1].PlayerTurn = FightRoundResult.Player.B;
				}
				else
				{
					BattleEngine.Current.Results[BattleEngine.Current.Results.Count-1].PlayerTurn = FightRoundResult.Player.A;
				}
			}
			else
			{
				if(BattleEngine.Current.CurrentPlayer == FightRoundResult.Player.A)
				{
					BattleEngine.Current.Result.PlayerTurn = FightRoundResult.Player.B;
				}
				else
				{
					BattleEngine.Current.Result.PlayerTurn = FightRoundResult.Player.A;
				}
			}
			_counter++;
			BattleEngine.Current.Result.Turn = _counter;
		}
	}
}
