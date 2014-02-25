using UnityEngine;
using System.Collections;

public class MonsterStats : MonoBehaviour 
{

	public void Init(GameManager.ResourceElement element)
	{
		Color color = Color.grey;
		switch(element)
		{
		case GameManager.ResourceElement.fire:
			color = Color.red;
			break;
		case GameManager.ResourceElement.energy:
			color = Color.yellow;
			break;
		case GameManager.ResourceElement.life:
			color = Color.green;
			break;
		case GameManager.ResourceElement.water:
			color = Color.blue;
			break;
		case GameManager.ResourceElement.storm:
			color = Color.gray;
			break;
		}
		for(int index = 0; index < GetComponentsInChildren<SkinnedMeshRenderer>().Length; index++)
			GetComponentsInChildren<SkinnedMeshRenderer>()[index].material.color = color;
	}
}
