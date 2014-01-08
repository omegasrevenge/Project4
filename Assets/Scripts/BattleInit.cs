using UnityEngine;
using System.Collections;

public class BattleInit : MonoBehaviour 
{
	public enum Element{Fire, Technology, Nature, Water, Wind}
	
	public Element MonsterAElement = Element.Nature; //dirty testing method
	public Element MonsterBElement = Element.Fire;
	
	public string MonsterAName = "Cthulhu";
	public string MonsterBName = "Ragnaros";
	
	public int MonsterAHealth = 300;
	public int MonsterBHealth = 500;
	
	public int MonsterALevel = 20;
	public int MonsterBLevel = 19;

}