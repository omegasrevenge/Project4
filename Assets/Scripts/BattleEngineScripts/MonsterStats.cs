using UnityEngine;
using System.Collections;

public class MonsterStats : MonoBehaviour 
{

	public BattleInit.Element Element;
	public string Name;
	public int Level;
	public int HP;

	public void Init(BattleInit.Element element, string name, int level, int hp)
	{
		Element = element;
		Name = name;
		Level = level;
		HP = hp;

		Color color = Color.grey;
		switch(Element)
		{
		case BattleInit.Element.Fire:
			color = Color.red;
			break;
		case BattleInit.Element.Technology:
			color = Color.black;
			break;
		case BattleInit.Element.Nature:
			color = Color.green;
			break;
		case BattleInit.Element.Water:
			color = Color.blue;
			break;
		case BattleInit.Element.Wind:
			color = Color.white;
			break;
		}
		transform.FindChild("model").GetComponent<MeshRenderer>().material.color = color;


	}
}
