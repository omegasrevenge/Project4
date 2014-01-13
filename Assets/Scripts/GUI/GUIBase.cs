using System.Net.NetworkInformation;
using UnityEngine;
using System.Collections;

public class GUIBase : MonoBehaviour
{

	void OnGUI()
	{
		if (GameManager.Singleton.CurrentGameMode != GameManager.GameMode.Base) return;

		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Resources.Load<Texture>("GUITextures/Background"));

		if (GUI.Button(new Rect(320, 330, 120, 50), "Leave Base"))
		{
			GameManager.Singleton.SwitchGameMode(GameManager.GameMode.Map);
			Debug.Log("Leaving Base!");
		}
	}

	void Update () {
		
	}
}
