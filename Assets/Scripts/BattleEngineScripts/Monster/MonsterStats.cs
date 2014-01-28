using UnityEngine;
using System.Collections;

public class MonsterStats : MonoBehaviour 
{

    public BattleEngine.ResourceElement Element;
	public string Name;
	public int Level;
	public int HP;

    public void Init(BattleEngine.ResourceElement element, string name, int level, int hp)
	{
		Element = element;
		Name = name;
		Level = level;
		HP = hp;

		Color color = Color.grey;
		switch(Element)
		{
            case BattleEngine.ResourceElement.Fire:
			color = Color.red;
			break;
            case BattleEngine.ResourceElement.Energy:
			color = Color.black;
			break;
            case BattleEngine.ResourceElement.Nature:
			color = Color.green;
			break;
            case BattleEngine.ResourceElement.Water:
			color = Color.blue;
			break;
            case BattleEngine.ResourceElement.Storm:
			color = Color.white;
			break;
		}
		transform.FindChild("model").GetComponent<MeshRenderer>().material.color = color;


	}
}
