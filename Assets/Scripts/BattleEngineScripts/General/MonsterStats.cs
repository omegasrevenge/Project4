using UnityEngine;

public class MonsterStats : MonoBehaviour 
{

	public void Init(GameManager.ResourceElement element, bool isWolf, bool isTamed)
	{
	    string model = isWolf ? "Wolf_Texture_" : "Giant_Texture_";
	    string tamed = isTamed ? "_Tamed" : "";
	    string location = "Textures/Battle/";
	    string elementString = "Fire";

		switch(element)
		{
		case GameManager.ResourceElement.fire:
            elementString = "Fire";
			break;
        case GameManager.ResourceElement.energy:
            elementString = "Energy";
			break;
        case GameManager.ResourceElement.life:
            elementString = "Life";
			break;
        case GameManager.ResourceElement.water:
            elementString = "Ice";
			break;
        case GameManager.ResourceElement.storm:
            elementString = "Storm";
			break;
        default:
            Debug.LogError(gameObject.name+" is getting a NONE-MonsterElement input!");
		    break;
		}
        Texture look = Resources.Load<Texture>(location + model + elementString + tamed);
	    for (int index = 0; index < GetComponentsInChildren<SkinnedMeshRenderer>().Length; index++)
	        GetComponentsInChildren<SkinnedMeshRenderer>()[index].material.mainTexture = look;
	}
}
