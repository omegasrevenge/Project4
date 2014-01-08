using UnityEngine;
using System.Collections;

public class BattleInit : MonoBehaviour 
{
	public enum Element{Fire, Technology, Nature, Water, Wind}
	
	public Element MonsterAElement = Element.Fire; //dirty testing method
	public Element MonsterBElement = Element.Water;
	
	public string MonsterAName;
	public string MonsterBName;
	
	public int MonsterAHealth;
	public int MonsterBHealth;
	
	public int MonsterALevel;
	public int MonsterBLevel;

}
