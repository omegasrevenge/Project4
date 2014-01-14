using System;
using System.Net.NetworkInformation;
using UnityEngine;
using System.Collections;

public class GUIBase : MonoBehaviour
{
	public enum ResourceLevel { Bioden, DriodenLvl0, DriodenLvl1, DriodenLvl2, DriodenLvl3, DriodenLvl4, DriodenLvl5 };
	public ResourceLevel curResourceLevel = ResourceLevel.Bioden;

	public enum ResourceElement { Fire, Tech, Nature, Water, Storm };
	public ResourceElement CuResourceElement;

	public bool showWindow = false;
	private Rect windowRect;
	private GUIStyle textGuiStyle;
	private string curInput = "1";	

	void Awake()
	{
		textGuiStyle = new GUIStyle { fontSize = 30 };
		textGuiStyle.normal.textColor = Color.white;
	}
	
	void OnGUI()
	{
		if (GameManager.Singleton.CurrentGameMode != GameManager.GameMode.Base) return;

		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Resources.Load<Texture>("GUITextures/Background"));

		if (showWindow)
		{
			windowRect = new Rect(20, 20, Screen.width - 40, Screen.height - 40);
			GUI.color = Color.black;
			GUI.Window(0, windowRect, DoWindow, "Crafting");
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
		GUI.Label(new Rect(70, 30, 200, 50), curResourceLevel.ToString(), textGuiStyle);

		GUI.Label(new Rect((windowRect.width-220), 30, 200, 50), CuResourceElement.ToString(), textGuiStyle);
		
		if (GUI.Button(new Rect(20,30,40,40),Resources.Load<Texture>("GUITextures/Previous")))
		{
			curResourceLevel = (ResourceLevel)(((int)curResourceLevel - 1) == -1 ? 6 : ((int)curResourceLevel - 1));
		}
		if (GUI.Button(new Rect(250, 30, 40, 40), Resources.Load<Texture>("GUITextures/Next")))
		{
			curResourceLevel = (ResourceLevel)(((int)curResourceLevel + 1) % 7);
		}

		ResourcesExchange();

		ShowChart();

		if (GUI.Button(new Rect((windowRect.width / 2) - 100, windowRect.height - 70, 200, 50), "Close Window"))
		{
			showWindow = false;
		}
	}

	private void ResourcesExchange()
	{
		curInput = GUI.TextField(new Rect((windowRect.width/2) - 50, 270, 100, 50), curInput);

		if (GUI.Button(new Rect((windowRect.width/2) - 50, 120, 100, 50), ResourceElement.Fire.ToString()))
		{
			CuResourceElement = ResourceElement.Fire;
		}
		if (GUI.Button(new Rect((windowRect.width / 2) + 120, 250, 100, 50), ResourceElement.Tech.ToString()))
		{
			CuResourceElement = ResourceElement.Tech;
		}
		if (GUI.Button(new Rect((windowRect.width / 2) + 50, 400, 100, 50), ResourceElement.Nature.ToString()))
		{
			CuResourceElement = ResourceElement.Nature;
		}
		if (GUI.Button(new Rect((windowRect.width / 2) - 150, 400, 100, 50), ResourceElement.Water.ToString()))
		{
			CuResourceElement = ResourceElement.Water;
		}
		if (GUI.Button(new Rect((windowRect.width / 2) - 220, 250, 100, 50), ResourceElement.Storm.ToString()))
		{
			CuResourceElement = ResourceElement.Storm;
		}

		if (GUI.Button(new Rect((windowRect.width / 2) - 50, 500, 100, 50), "Circle"))
		{
			int input = -1;
			if (Int32.TryParse(curInput, out input))
			{
				GameManager.Singleton.Exchange((int)CuResourceElement, (int)curResourceLevel, input*2, GameManager.ExchangeMode.Cricle);
			}
		}
		if (GUI.Button(new Rect((windowRect.width / 2) - 190, 500, 100, 50), "Up"))
		{
			int input = -1;
			if (Int32.TryParse(curInput, out input))
			{
				GameManager.Singleton.Exchange((int)CuResourceElement, (int)curResourceLevel, input*(curResourceLevel == ResourceLevel.Bioden?10:3), GameManager.ExchangeMode.Up);
			}
		}
		if (GUI.Button(new Rect((windowRect.width / 2) + 90, 500, 100, 50), "Down"))
		{
			int input = -1;
			if (Int32.TryParse(curInput, out input))
			{
				GameManager.Singleton.Exchange((int)CuResourceElement, (int)curResourceLevel, input, GameManager.ExchangeMode.Down);
			}
		}
	}

	private void ShowChart()
	{

		for (int i = 0; i < 7; i++)
		{
			string z = "" + i + ": ";
			for (int j = 0; j < 5; j++)
			{
				z += GameManager.Singleton.Player.Resources[i, j] + " ";
			}
			GUI.Label(new Rect((windowRect.width / 2) - 100, (windowRect.height - 100 - 40 * 7) + i * 40, 200, 20), z, textGuiStyle);
		}
	}

	void Update () {
		
	}
}
