using UnityEngine;
using System.Collections;

public class MonsterStats : MonoBehaviour 
{

	public void Init(GameManager.ResourceElement element)
	{
		Color color = Color.grey;
		switch(element)
		{
		case GameManager.ResourceElement.Fire:
			color = Color.red;
			break;
		case GameManager.ResourceElement.Energy:
			color = Color.black;
			break;
		case GameManager.ResourceElement.Nature:
			color = Color.green;
			break;
		case GameManager.ResourceElement.Water:
			color = Color.blue;
			break;
		case GameManager.ResourceElement.Storm:
			color = Color.white;
			break;
		}
		transform.FindChild("model").GetComponent<MeshRenderer>().material.color = color;
	}
}
