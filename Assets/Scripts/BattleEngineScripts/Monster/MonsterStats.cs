using UnityEngine;
using System.Collections;

public class MonsterStats : MonoBehaviour 
{

    public void Init(BattleEngine.ResourceElement element)
	{
		Color color = Color.grey;
		switch(element)
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
