using UnityEngine;
using System.Collections;

public class BattleInit
{
	public GameManager.ResourceElement MonsterAElement = GameManager.ResourceElement.life; //default values. also used as a dirty testing method
	public GameManager.ResourceElement MonsterBElement = GameManager.ResourceElement.fire;	 //you can put in different values, but there HAVE to be default values
	
	public string MonsterAName = "Cthulhu";
	public string MonsterBName = "Ragnaros";
	
	public int MonsterAHealth = 300;
	public int MonsterBHealth = 300;

	public int MonsterAMaxHealth = 300;
	public int MonsterBMaxHealth = 300;
	
	public int MonsterALevel = 20;
	public int MonsterBLevel = 19;

	public FightRoundResult.Player FirstTurnIsPlayer = FightRoundResult.Player.A;
	
	public int BaseMeshA = 0;
	public int BaseMeshB = 0;

}