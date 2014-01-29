using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using System.Collections;

public class GUIBase : MonoBehaviour
{
	public enum ResourceLevel { Bioden, DriodenLvl0, DriodenLvl1, DriodenLvl2, DriodenLvl3, DriodenLvl4, DriodenLvl5 };
	public ResourceLevel curResourceLevel = ResourceLevel.Bioden;

    public BattleEngine.ResourceElement CuResourceElement;

	private enum Windows { Crafting, Creature };
	private Windows curWindow;

	public bool showWindow = false;
	private Rect windowRect;
	private GUIStyle textGuiStyle;
	private string curInput = "1";
	private int curInputAsInt = -1;

	private Creature equiptCreature;
	private List<Creature> allOwnCreatures;
	private int creatureID = 1;
	private int creatureIndex = 0;

	void Awake()
	{
		textGuiStyle = new GUIStyle { fontSize = 30 };
		textGuiStyle.normal.textColor = Color.white;
		textGuiStyle.alignment = TextAnchor.MiddleCenter;
	}
	
	void OnGUI()
	{
		if (GameManager.Singleton.CurrentGameMode != GameManager.GameMode.Base) return;

		equiptCreature = GameManager.Singleton.Player.CurCreature;
		allOwnCreatures = GameManager.Singleton.AllOwnCreatures;

		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Resources.Load<Texture>("GUITextures/Background"));

		if (showWindow)
		{
			ShowWindow(curWindow);
			return;
		}

		if (GUI.Button(new Rect(320, 220, 120, 50), "<color=white><size=20>Leave Base</size></color>"))
		{
			GameManager.Singleton.SwitchGameMode(GameManager.GameMode.Map);
		}

		if (GUI.Button(new Rect(30, 380, 120, 50), "<color=white><size=20>" + "Crafting" + "</size></color>"))
		{
			ShowWindow(Windows.Crafting);
		}

		if (GUI.Button(new Rect(330, 500, 140, 50), "<color=white><size=20>" + "Creature Lab" + "</size></color>"))
		{
			creatureID = equiptCreature.CreatureID;
			ShowWindow(Windows.Creature);
		}
	}

	private void ShowWindow(Windows windowName)
	{
		showWindow = true;
		curWindow = windowName;
		windowRect = new Rect(20, 20, Screen.width - 40, Screen.height - 40);
		GUI.color = Color.black;
		switch (windowName)
		{
			case Windows.Crafting:
			{
				GUI.Window(0, windowRect, CraftingWindow, windowName.ToString());
				break;
			}
			case Windows.Creature:
			{
				GUI.Window(1, windowRect, CreatureWindow, windowName.ToString());
				break;
			}
		}
	}

	private void CreatureWindow(int windowID)
	{
		int[] creatureIDs = GameManager.Singleton.Player.creatureIDs;

		if (GUI.Button(new Rect((windowRect.width / 2) - 150, 30, 40, 40), Resources.Load<Texture>("GUITextures/Previous")))
		{
			creatureIndex = creatureIndex <= 0 ? (creatureIDs.Length-1) : creatureIndex- 1;
			creatureID = creatureIDs[creatureIndex];
		}
		if (GUI.Button(new Rect((windowRect.width / 2) + 100, 30, 40, 40), Resources.Load<Texture>("GUITextures/Next")))
		{
			creatureIndex = creatureIndex >= (creatureIDs.Length-1) ? 0 : creatureIndex + 1;
			creatureID = creatureIDs[creatureIndex];
		}

		ShowCreature(creatureID);

		if (GUI.Button(new Rect((windowRect.width / 2) - 100, windowRect.height - 70, 200, 50), "<color=white><size=20>Close Window</size></color>"))
		{
			showWindow = false;
		}
	}

	private void ShowCreature(int creatureID)
	{
		Creature curCreature = null;

		foreach (Creature ownCreature in allOwnCreatures)
		{
			if (ownCreature.CreatureID == creatureID)
			{
				curCreature = ownCreature;
			}
		}

		if (curCreature == null)
		{
			Debug.LogError("No Creature Found!");
			return;
		}

		GUI.Label(new Rect((windowRect.width / 2) - 100, 30, 200, 50), curCreature.Name, textGuiStyle);

		GUIStyle curStyle = new GUIStyle(textGuiStyle);
		curStyle.fontSize = 20;

		if (curCreature.CreatureID == equiptCreature.CreatureID)
		{
			GUI.Label(new Rect((windowRect.width / 2) - 100, 80, 200, 50), "Equipped", curStyle);
		}
		
		curStyle.alignment = TextAnchor.MiddleLeft;

		GUI.Label(new Rect(50, 100, 200, 50), "Level:  " + curCreature.Level, curStyle);
		GUI.Label(new Rect(50, 125, 200, 50), "XP:  " + curCreature.XP, curStyle);
		GUI.Label(new Rect(50, 150, 200, 50), "HP:  " + curCreature.HP + " / " + curCreature.HPMax, curStyle);
		GUI.Label(new Rect(50, 175, 200, 50), "Damage:  " + curCreature.Damage, curStyle);
		GUI.Label(new Rect(50, 200, 200, 50), "Defense:  " + curCreature.Defense, curStyle);
		GUI.Label(new Rect(50, 225, 200, 50), "Dexterity:  " + curCreature.Dexterity, curStyle);
		GUI.Label(new Rect(50, 250, 200, 50), "Skillpoints:  " + curCreature.Skillpoints, curStyle);
		GUI.Label(new Rect(50, 275, 200, 50), "Base Element:  " + curCreature.BaseElement, curStyle);

		DriodenSlots(curCreature);
		ShowChart();
	}

	private void DriodenSlots(Creature curCreature)
	{
		for (int i = 0; i < 4; i++)
		{
			Rect curRect = new Rect(120 + i * 110, 350, 80, 80);

			if (i < curCreature.slots.Length)
			{
				Creature.Slot curSlot = curCreature.slots[i];

				if ((int)curSlot.driodenElement == -1 && curSlot.driodenLevel == -1)
				{
					if (GUI.Button(curRect, Resources.Load<Texture>("GUITextures/lock_open")))
					{
						GameManager.Singleton.EquipCreatureSlot(curCreature.CreatureID, curCreature.slots[i].slotId, (int)CuResourceElement, (int)curResourceLevel);
					}
					continue;
				}

				if (GUI.Button(curRect, "<color=white><size=20>" + ((BattleEngine.ResourceElement)curSlot.driodenElement) + "\n" + (curSlot.driodenLevel - 1) + "</size></color>"))
				{
					GameManager.Singleton.EquipCreatureSlot(curCreature.CreatureID, curCreature.slots[i].slotId, (int)CuResourceElement, (int)curResourceLevel);
				}
			}
			else
			{
				if (GUI.Button(curRect, Resources.Load<Texture>("GUITextures/lock")))
				{
					GameManager.Singleton.AddCreatureEQSlot(curCreature.CreatureID);
				}
			}
		}
		if (curResourceLevel == ResourceLevel.Bioden) curResourceLevel = ResourceLevel.DriodenLvl0;

		GUI.Label(new Rect(60, 450, 200, 50), curResourceLevel.ToString(), textGuiStyle);

		GUI.Label(new Rect((windowRect.width - 270), 450, 200, 50), CuResourceElement.ToString(), textGuiStyle);

		if (GUI.Button(new Rect(20, 450, 40, 40), Resources.Load<Texture>("GUITextures/Previous")))
		{
			curResourceLevel = (ResourceLevel)(((int)curResourceLevel - 1) == 0 ? 6 : ((int)curResourceLevel - 1));
		}
		if (GUI.Button(new Rect(250, 450, 40, 40), Resources.Load<Texture>("GUITextures/Next")))
		{
			curResourceLevel = (ResourceLevel)(((int)curResourceLevel + 1) % 7);
		}

		if (GUI.Button(new Rect(80, 550, 80, 80), "<color=white><size=20>" + BattleEngine.ResourceElement.Energy.ToString() + "</size></color>"))
		{
			CuResourceElement = BattleEngine.ResourceElement.Energy;
		}
		if (GUI.Button(new Rect(170, 550, 80, 80), "<color=white><size=20>" + BattleEngine.ResourceElement.Fire.ToString() + "</size></color>"))
		{
			CuResourceElement = BattleEngine.ResourceElement.Fire;
		}
		if (GUI.Button(new Rect(260, 550, 80, 80), "<color=white><size=20>" + BattleEngine.ResourceElement.Storm.ToString() + "</size></color>"))
		{
			CuResourceElement = BattleEngine.ResourceElement.Storm;
		}
		if (GUI.Button(new Rect(350, 550, 80, 80), "<color=white><size=20>" + BattleEngine.ResourceElement.Nature.ToString() + "</size></color>"))
		{
			CuResourceElement = BattleEngine.ResourceElement.Nature;
		}
		if (GUI.Button(new Rect(440, 550, 80, 80), "<color=white><size=20>" + BattleEngine.ResourceElement.Water.ToString() + "</size></color>"))
		{
			CuResourceElement = BattleEngine.ResourceElement.Water;
		}
	}

	#region Crafting

	private void CraftingWindow(int windowID)
	{
		GUI.Label(new Rect(60, 30, 200, 50), curResourceLevel.ToString(), textGuiStyle);

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

		if (GUI.Button(new Rect((windowRect.width / 2) - 100, windowRect.height - 70, 200, 50), "<color=white><size=20>Close Window</size></color>"))
		{
			showWindow = false;
		}
	}

	private void ResourcesExchange()
	{
		GUIStyle textFieldStyle = textGuiStyle;

		curInput = GUI.TextField(new Rect((windowRect.width / 2) - 20, 270, 50, 50), curInput, 3, textFieldStyle);

		if (Int32.TryParse(curInput, out curInputAsInt))
		{
			if (GUI.Button(new Rect((windowRect.width / 2) - 70, 270, 40, 40), "<color=white><size=30>-</size></color>"))
			{
				curInputAsInt = curInputAsInt == 1 ? 1 : curInputAsInt - 1;
			}
			if (GUI.Button(new Rect((windowRect.width / 2) + 40, 270, 40, 40), "<color=white><size=30>+</size></color>"))
			{
				curInputAsInt++;
			}

			curInput = curInputAsInt.ToString();
		}

        if (GUI.Button(new Rect((windowRect.width / 2) - 50, 120, 100, 50), "<color=white><size=20>" + BattleEngine.ResourceElement.Energy.ToString() + "</size></color>"))
		{
            CuResourceElement = BattleEngine.ResourceElement.Energy;
		}
        if (GUI.Button(new Rect((windowRect.width / 2) + 120, 250, 100, 50), "<color=white><size=20>" + BattleEngine.ResourceElement.Fire.ToString() + "</size></color>"))
		{
            CuResourceElement = BattleEngine.ResourceElement.Fire;
		}
        if (GUI.Button(new Rect((windowRect.width / 2) + 50, 400, 100, 50), "<color=white><size=20>" + BattleEngine.ResourceElement.Storm.ToString() + "</size></color>"))
		{
            CuResourceElement = BattleEngine.ResourceElement.Storm;
		}
        if (GUI.Button(new Rect((windowRect.width / 2) - 150, 400, 100, 50), "<color=white><size=20>" + BattleEngine.ResourceElement.Nature.ToString() + "</size></color>"))
		{
            CuResourceElement = BattleEngine.ResourceElement.Nature;
		}
        if (GUI.Button(new Rect((windowRect.width / 2) - 220, 250, 100, 50), "<color=white><size=20>" + BattleEngine.ResourceElement.Water.ToString() + "</size></color>"))
		{
            CuResourceElement = BattleEngine.ResourceElement.Water;
		}

		if (GUI.Button(new Rect((windowRect.width / 2) - 50, 500, 100, 50), "<color=white><size=20>Circle</size></color>"))
		{
			GameManager.Singleton.Exchange((int)CuResourceElement, (int)curResourceLevel, curInputAsInt * 2, GameManager.ExchangeMode.Cricle);
		}
		if (GUI.Button(new Rect((windowRect.width / 2) - 190, 500, 100, 50), "<color=white><size=20>Up</size></color>"))
		{
			GameManager.Singleton.Exchange((int)CuResourceElement, (int)curResourceLevel, curInputAsInt * (curResourceLevel == ResourceLevel.Bioden ? 10 : 3), GameManager.ExchangeMode.Up);
		}
		if (GUI.Button(new Rect((windowRect.width / 2) + 90, 500, 100, 50), "<color=white><size=20>Down</size></color>"))
		{
			GameManager.Singleton.Exchange((int)CuResourceElement, (int)curResourceLevel, curInputAsInt, GameManager.ExchangeMode.Down);
		}
	}
	#endregion

	private void ShowChart()
	{

		for (int i = 0; i < 7; i++)
		{
			string z = "" + i + ": ";
			for (int j = 0; j < 5; j++)
			{
				z += GameManager.Singleton.Player.Resources[i, j] + "| ";
			}
			GUI.Label(new Rect((windowRect.width / 2) - 100, (windowRect.height - 70 - 40 * 7) + i * 40, 200, 20), z, textGuiStyle);
		}
	}

	void Update () {
		
	}
}