using UnityEngine;
using System.Collections;

public class BattleInit
{
	public enum Element{Fire, Technology, Nature, Water, Wind}
	
	public Element MonsterAElement = Element.Nature; //default values. also used as a dirty testing method
	public Element MonsterBElement = Element.Fire;	 //you can put in different values, but there HAVE to be default values
	
	public string MonsterAName = "Cthulhu";
	public string MonsterBName = "Ragnaros";
	
	public int MonsterAHealth = 300;
	public int MonsterBHealth = 500;
	
	public int MonsterALevel = 20;
	public int MonsterBLevel = 19;

	public FightRoundResult.Player FirstTurnIsPlayer = FightRoundResult.Player.A;

}