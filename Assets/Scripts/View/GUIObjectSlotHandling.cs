using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class GUIObjectSlotHandling : MonoBehaviour
{
	private const string ImprintSpritePrefix = "combat_slot_imprint_";
	private const string ElementSpritePrefix = "combat_driod_";
	private const string LevelSpritePrefix = "combat_driod_level_";
	private const string ImprintSpriteStr = "sprite_imprint_";
	private const string DriodHealthStr = "sprite_droid_health";
	private const string DriodElementStr = "sprite_element";
	private const string DriodLevelStr = "sprite_level";

	private dfSprite _driodHealth;
	private dfSprite _driodElement;
	private dfSprite _driodLevel;
	
	private GameObject root;
	public Creature.Slot slot;
	private List<dfSprite> imprints = new List<dfSprite>();

	private bool init = false;

	private void Init()
	{
		if (init) return;
		init = true;

		for (int i = 1; i <= 10; i++)
		{
			imprints.Add(root.transform.Find(ImprintSpriteStr + i).GetComponent<dfSprite>());
		}

		_driodHealth = root.transform.Find(DriodHealthStr).GetComponent<dfSprite>();
		_driodElement = root.transform.Find(DriodElementStr).GetComponent<dfSprite>();
		_driodLevel = root.transform.Find(DriodLevelStr).GetComponent<dfSprite>();

		RefreshView();
	}

	public void RefreshView()
	{
		_driodLevel.Show();
		_driodHealth.FillAmount = 0.57f + slot.driodenHealth * 0.36f;
		_driodElement.SpriteName = ElementSpritePrefix + slot.driodenElement.ToString();
		if (slot.driodenLevel > 1)
		{
			_driodLevel.SpriteName = LevelSpritePrefix + (slot.driodenLevel - 1);
		}
		else
		{
			_driodLevel.Hide();
		}

		int e = 0; int f = 0; int s = 0; int l = 0; int w = 0;

		for (int i = 0; i < imprints.Count; i++)
		{
			imprints[i].Show();
			if (e < slot.energy)
			{
				imprints[i].SpriteName = ImprintSpritePrefix + GameManager.ResourceElement.energy + (i == 9 ? "" : "0") + (i + 1);
				e++;
			}
			else if (f < slot.fire)
			{
				imprints[i].SpriteName = ImprintSpritePrefix + GameManager.ResourceElement.fire + (i == 9 ? "" : "0") + (i + 1);
				f++;
			}
			else if (s < slot.storm)
			{
				imprints[i].SpriteName = ImprintSpritePrefix + GameManager.ResourceElement.storm + (i == 9 ? "" : "0") + (i + 1);
				s++;
			}
			else if (l < slot.nature)
			{
				imprints[i].SpriteName = ImprintSpritePrefix + GameManager.ResourceElement.life + (i == 9 ? "" : "0") + (i + 1);
				l++;
			}
			else if (w < slot.water)
			{
				imprints[i].SpriteName = ImprintSpritePrefix + GameManager.ResourceElement.water + (i == 9 ? "" : "0") + (i + 1);
				w++;
			}
			else
				imprints[i].Hide();
		}
	}
	public static GUIObjectSlotHandling AddSlotHandling(GameObject root, Creature.Slot slot)
	{
		GUIObjectSlotHandling cntr = root.AddComponent<GUIObjectSlotHandling>();
		cntr.root = root;
		cntr.slot = slot;
		return cntr;
	}

	void Update()
	{
		Init();
	}
}
