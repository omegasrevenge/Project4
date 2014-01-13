using System.Net.NetworkInformation;
using UnityEngine;
using System.Collections;

public class GUIBase : MonoBehaviour
{
	public bool showWindow = false;

	void OnGUI()
	{
		if (GameManager.Singleton.CurrentGameMode != GameManager.GameMode.Base) return;

		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Resources.Load<Texture>("GUITextures/Background"));

		if (showWindow)
		{
			GUI.color = Color.black;
			GUI.Window(0, new Rect(20, 20, Screen.width - 40, Screen.height - 40), DoWindow, "Crafting");
			return;
		}
		
		if (GUI.Button(new Rect(320, 220, 120, 50), "Leave Base"))
		{
			GameManager.Singleton.SwitchGameMode(GameManager.GameMode.Map);
			Debug.Log("Leaving Base!");
		}

		if (GUI.Button(new Rect(330, 500, 120, 50), "Crafting"))
		{
			showWindow = true;
		}
	}

	void DoWindow(int windowID)
	{
		if (GUI.Button(new Rect(200, 360, (Screen.width/2)-100, 50), "Close Window"))
		{
			showWindow = false;
		}

		GUIStyle curGuiStyle = new GUIStyle { fontSize = 30 };
		curGuiStyle.normal.textColor = Color.white;

		for (int i = 0; i < 7; i++)
		{
			string z = "" + i + ":";
			for (int j = 0; j < 5; j++)
			{
				z += GameManager.Singleton.Player.Resources[i, j] + " ";
			}
			GUI.Label(new Rect(20, 40 + i * 40, 200, 20), z, curGuiStyle);
		}
	}

	void Update () {
		
	}
}
