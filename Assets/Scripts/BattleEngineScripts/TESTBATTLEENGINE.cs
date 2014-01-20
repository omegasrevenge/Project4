using UnityEngine;
using System.Collections;

public class TESTBATTLEENGINE : MonoBehaviour {

	private int _counter = 0;
	private int _AHP;
	private int _BHP;
	
	
	void Update()
	{
		
		if(Input.GetKeyDown(KeyCode.A))
		{
			Init();
		}
		
		if(Input.GetKeyDown(KeyCode.Space))
		{
			CreateNewSubstituteForResult();
		}
	}

	public void Init()
	{
		if(BattleEngine.Current == null)
		{
			_AHP = new BattleInit().MonsterAHealth;
			_BHP = new BattleInit().MonsterBHealth;
			BattleEngine.CreateBattle(new BattleInit());
		}
		else
		{
			BattleEngine.Current.DestroyBattle();
		}
	}

	public void CreateNewSubstituteForResult()
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
		BattleEngine.Current.Results[BattleEngine.Current.Results.Count-1].Turn = _counter;
		switch(BattleEngine.Current.Results[BattleEngine.Current.Results.Count-1].PlayerTurn)
		{
		case FightRoundResult.Player.A:
			_AHP -= 50;
			BattleEngine.Current.Results[BattleEngine.Current.Results.Count-1].PlayerAHealth =  _AHP;
			BattleEngine.Current.Results[BattleEngine.Current.Results.Count-1].PlayerBHealth = _BHP;
			break;
		case FightRoundResult.Player.B:
			_BHP -= 50;
			BattleEngine.Current.Results[BattleEngine.Current.Results.Count-1].PlayerAHealth =  _AHP;
			BattleEngine.Current.Results[BattleEngine.Current.Results.Count-1].PlayerBHealth = _BHP;
			break;
		}
	}
}
