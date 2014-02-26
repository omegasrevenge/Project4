using System.Collections.Generic;
using UnityEngine;

public class GUIObjectBaseMenue : MonoBehaviour 
{
	private const string Prefab = "GUI/panel_basemenue";
	private const string BoxStr = "slot_box";
	private const string LabelSpecterLevel = "label_spectre_level";
	private const string LabelSpecterName = "label_spectre_name";
	private const string LabelSpecterStatNames = "label_spectre_stats_names";
	private const string LabelSpecterStatNumbers = "label_spectre_stats_numbers";
	private const string DriodSlotStr = "driodslot_";
	private const string EquipSlotSpriteStr = "combat_panel_";
	private const string SlotButtonFocusStr = "combat_slot_active_";
	private const string CraftingSpriteStr = "Base_Interface_CraftingButton_";
	private const string CraftingButtonStr = "button_crafting";
	private const string ExitButtonStr = "button_exit";
	
	private dfButton _craftingButton;
	private dfButton _exitButton;
	private dfLabel _spectreLevel;
	private dfLabel _spectreName;
	private dfLabel _spectreStatsNames;
	private dfLabel _spectreStatsNumbers;
	private dfPanel _slotBox;
	private Player.Faction _curFaction;
	private Creature _curCreature;

	private bool init = false;

	private List<dfButton> driodSlots = new List<dfButton>();
	private List<Color32> factionColors = new List<Color32>();

	[SerializeField]
	private GameManager.ResourceLevel _curResourceLevel = GameManager.ResourceLevel.biod;

	private void UpdateSpecter()
	{
		_spectreLevel.Text = _curCreature.Level.ToString();
		_spectreName.Text = _curCreature.Name;
		_spectreStatsNumbers.Text = "" + _curCreature.HP + "\n" + _curCreature.ExtraDamage + "\n" + _curCreature.Dexterity + "\n" + _curCreature.Defense + "\n" + _curCreature.HPReg;
	}

	void Awake()
	{
		factionColors.Add(GameManager.Withe);
		factionColors.Add(GameManager.Black);

		_spectreLevel = transform.Find(LabelSpecterLevel).GetComponent<dfLabel>();
		_spectreName = transform.Find(LabelSpecterName).GetComponent<dfLabel>();
		_spectreStatsNames = transform.Find(LabelSpecterStatNames).GetComponent<dfLabel>();
		_spectreStatsNumbers = transform.Find(LabelSpecterStatNumbers).GetComponent<dfLabel>();

		SetStatNames();
	}

	private void SetStatNames()
	{
		string life = Localization.GetText("spectre_life_text");
		string exdmg = Localization.GetText("spectre_extradamage_text");
		string dex = Localization.GetText("spectre_dexterity_text");
		string def = Localization.GetText("spectre_defence_text");
		string reg = Localization.GetText("spectre_regeneration_text");

		_spectreStatsNames.Text = life + "\n" + exdmg + "\n" + dex + "\n" + def + "\n" + reg;
	}

	private void Init()
	{
		if(init) return;
		init = true;

		Transform slotTransform = transform.Find(BoxStr);
		_slotBox = slotTransform.GetComponent<dfPanel>();
		_craftingButton = slotTransform.Find(CraftingButtonStr).GetComponent<dfButton>();
		for (int i = 1; i <= 4; i++)
		{
			dfButton curButton = slotTransform.Find(DriodSlotStr + i).GetComponent<dfButton>();

			if (i > _curCreature.slots.Length)
			{
				curButton.gameObject.SetActive(false);
				continue;
			}
			GUIObjectSlotHandling.AddSlotHandling(curButton.gameObject, _curCreature.slots[i - 1]);
			curButton.FocusSprite = SlotButtonFocusStr + _curFaction.ToString().ToLower();
			curButton.Click +=
				(control, @event) =>
				{
					SoundController.PlaySound(SoundController.SoundClick, SoundController.ChannelSFX);
					transform.parent.GetComponent<GUIObjectBaseUI>().AddEquip(_curCreature, _curCreature.slots[driodSlots.IndexOf((dfButton)control)]);
				};


			driodSlots.Add(curButton);
		}
		_exitButton = transform.Find(ExitButtonStr).GetComponent<dfButton>();

		_craftingButton.Click +=
				(control, @event) =>
				{
					SoundController.PlaySound(SoundController.SoundClick, SoundController.ChannelSFX);
					transform.parent.GetComponent<GUIObjectBaseUI>().AddCrafting();
				};
		_exitButton.Click +=
				(control, @event) =>
				{
					SoundController.PlaySound(SoundController.SoundClick, SoundController.ChannelSFX);
					GameManager.Singleton.SwitchGameMode(GameManager.GameMode.Map);
				};


		SetColor(gameObject.GetComponent<dfControl>());
	}

	private void SetColor(dfControl guiComponent)
	{
		guiComponent.Color = factionColors[(int) _curFaction];
		_craftingButton.BackgroundSprite = CraftingSpriteStr + _curFaction.ToString().ToLower();
		_slotBox.BackgroundSprite = EquipSlotSpriteStr + _curFaction.ToString().ToLower();
		foreach (dfButton driodSlot in driodSlots)
		{
			driodSlot.FocusSprite = SlotButtonFocusStr + _curFaction.ToString().ToLower();
		}
	}

	public static GUIObjectBaseMenue Create(dfControl root)
	{
		dfControl cntrl = root.AddPrefab(Resources.Load<GameObject>(Prefab));
        cntrl.Size = cntrl.Parent.Size;
        cntrl.RelativePosition = Vector2.zero;
		GUIObjectBaseMenue obj = cntrl.GetComponent<GUIObjectBaseMenue>();
		obj._curFaction = GameManager.Singleton.Player.CurrentFaction;
		obj._curCreature = GameManager.Singleton.Player.CurCreature;
		return obj;
    }

	void Update()
	{
		Init();
		UpdateSpecter();
	}
}
