using UnityEngine;
using System.Collections;

public class MonsterStats : MonoBehaviour 
{

	public GUIBase.ResourceElement Element;
	public string Name;
	public int Level;
	public int HP;

	public void Init(GUIBase.ResourceElement element, string name, int level, int hp)
	{
		Element = element;
		Name = name;
		Level = level;
		HP = hp;

		Color color = Color.grey;
		switch(Element)
		{
		case GUIBase.ResourceElement.Fire:
			color = Color.red;
			break;
		case GUIBase.ResourceElement.Tech:
			color = Color.black;
			break;
		case GUIBase.ResourceElement.Nature:
			color = Color.green;
			break;
		case GUIBase.ResourceElement.Water:
			color = Color.blue;
			break;
		case GUIBase.ResourceElement.Storm:
			color = Color.white;
			break;
		}
		transform.FindChild("model").GetComponent<MeshRenderer>().material.color = color;


	}
}
