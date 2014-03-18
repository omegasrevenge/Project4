using System.Collections.Generic;
using UnityEngine;

public class GUIObjectBaseMenue : MonoBehaviour 
{
	private const string Prefab                  = "GUI/panel_basemenue";
	private const string BoxStr                  = "slot_box";
	private const string LabelSpecterLevel       = "label_spectre_level";
	private const string LabelSpecterName        = "label_spectre_name";
	private const string LabelSpecterStatNames   = "label_spectre_stats_names";
	private const string LabelSpecterStatNumbers = "label_spectre_stats_numbers";
	private const string DriodSlotStr            = "driodslot_";
	private const string EquipSlotSpriteStr      = "combat_panel_";
	private const string SlotButtonFocusStr      = "combat_slot_active_";
	private const string CraftingSpriteStr       = "Base_Interface_CraftingButton_";
	private const string CraftingButtonStr       = "button_crafting";
	private const string ExitButtonStr           = "button_exit";
	private const string SwitchSpectre           = "switch_spectre";
	private const string SpectreElementStr       = "sprite_element";
	private const string SpectreElementSprite    = "crafting_element_";
	private const string EquipButtonStr          = "button_equip_spectre";
    private const string SendButtonStr           = "button_send_spectre";

	private GUIObjectEquip _equip;

	private dfButton _craftingButton;
	private dfButton _exitButton;
	private dfButton _equipButton;
    private dfButton _sendButton;
	private dfSprite _spectreElement;
	private dfLabel _spectreLevel;
	private dfLabel _spectreName;
	private dfLabel _spectreStatsNames;
	private dfLabel _spectreStatsNumbers;
	private dfPanel _slotBox;
	private dfPanel _switchSpectre;
	private Player.Faction _curFaction;
	private Creature _curCreature;

	private bool init = false;
	private int creatureIndex = 0;

	private List<dfButton> driodSlots = new List<dfButton>();
	private List<Color32> factionColors = new List<Color32>();
	private int[] creatureIDs;

	[SerializeField]
	private GameManager.ResourceLevel _curResourceLevel = GameManager.ResourceLevel.biod;

	public void UpdateSpectre()
	{
		_spectreLevel.Text = _curCreature.Level.ToString();
		_spectreName.Text = _curCreature.Name;
		_spectreStatsNumbers.Text = "" + _curCreature.HP + "\n" + _curCreature.ExtraDamage + "\n" + _curCreature.Dexterity + "\n" + _curCreature.Defense + "\n" + _curCreature.HPReg;
		_spectreElement.SpriteName = SpectreElementSprite + _curCreature.BaseElement;
		
		if (_curCreature.CreatureID == GameManager.Singleton.Player.CurCreature.CreatureID)
			_equipButton.Hide();
		else
			_equipButton.Show();
        
        if (GameManager.Singleton.Player.CurrentFaction == Player.Faction.NCE)
            _sendButton.Hide();
        else if (GameManager.Singleton.Player.creatureIDs.Length < 2)
            _sendButton.Hide();
        else
            _sendButton.Show();    

		if (init)
		{
			if (GameManager.Singleton.AllOwnCreatures.Count > 0) _curCreature = GameManager.Singleton.AllOwnCreatures[creatureIndex];

			for (int i = 0; i < driodSlots.Count; i++)
			{
				if (i >= _curCreature.slots.Length)
				{
					driodSlots[i].gameObject.SetActive(false);
					continue;
				}
				driodSlots[i].gameObject.SetActive(true);
				GUIObjectSlotHandling eqipSlot = driodSlots[i].GetComponent<GUIObjectSlotHandling>();
				eqipSlot.slot = _curCreature.slots[i];
				eqipSlot.RefreshView();
			}
		}
	}

	void Awake()
	{
		factionColors.Add(GameManager.Withe);
		factionColors.Add(GameManager.Black);

		_spectreLevel = transform.Find(LabelSpecterLevel).GetComponent<dfLabel>();
		_spectreName = transform.Find(LabelSpecterName).GetComponent<dfLabel>();
		_spectreStatsNames = transform.Find(LabelSpecterStatNames).GetComponent<dfLabel>();
		_spectreStatsNumbers = transform.Find(LabelSpecterStatNumbers).GetComponent<dfLabel>();
		_spectreElement = transform.Find(SpectreElementStr).GetComponent<dfSprite>();
		_equipButton = transform.Find(EquipButtonStr).GetComponent<dfButton>();
	    _sendButton = transform.FindChild(SendButtonStr).GetComponent<dfButton>();

		SetStatNames();
	}

	private void Init()
	{
		if(init) return;

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
                    SoundController.PlaySound(SoundController.SFXlocation + SoundController.Faction + SoundController.SoundFacClick, SoundController.ChannelSFX);
					transform.parent.GetComponent<GUIObjectBaseUI>().AddEquip(_curCreature, _curCreature.slots[driodSlots.IndexOf((dfButton)control)]);
				};

			driodSlots.Add(curButton);
		}
		_exitButton = transform.Find(ExitButtonStr).GetComponent<dfButton>();

		_craftingButton.Click +=
				(control, @event) =>
				{
                    SoundController.PlaySound(SoundController.SFXlocation + SoundController.Faction + SoundController.SoundFacClick, SoundController.ChannelSFX);
					transform.parent.GetComponent<GUIObjectBaseUI>().AddCrafting();
				};

		_exitButton.Click +=
				(control, @event) =>
				{
					SoundController.RemoveChannel(BaseSounds.BackgroundChannel);
                    SoundController.PlaySound(SoundController.SFXlocation + SoundController.Faction + SoundController.SoundFacClick, SoundController.ChannelSFX);
					GameManager.Singleton.SwitchGameMode(GameManager.GameMode.Map);
                    //tutorial
                    if (GameManager.Singleton.Player.InitSteps < 8)
                        GameManager.Singleton.GUILeaveBase();
				};

		_equipButton.Click +=
				(control, @event) =>
				{
                    SoundController.PlaySound(SoundController.SFXlocation + SoundController.Faction + SoundController.SoundFacClick, SoundController.ChannelSFX);
					GameManager.Singleton.SwitchCurrentCreature(_curCreature.CreatureID);
				};

        _sendButton.Click +=
                (control, @event) =>
                {                   
                    SoundController.PlaySound(SoundController.SFXlocation + SoundController.Faction + SoundController.SoundFacClick, SoundController.ChannelSFX);
                    GameManager.Singleton.GUISendCreatureWarning(_curCreature);
                    NextSpectre();
                };

		_switchSpectre = transform.Find(SwitchSpectre).GetComponent<dfPanel>();
		HandleDrag.AddHandleDrag(this, _switchSpectre.gameObject, _switchSpectre);

		SetColor(gameObject.GetComponent<dfControl>());
		init = true;
	}

	public void NextSpectre()
	{
		creatureIndex = creatureIndex >= (creatureIDs.Length - 1) ? 0 : creatureIndex + 1;
		_curCreature = GameManager.Singleton.AllOwnCreatures[creatureIndex];
		GameManager.Singleton.GUIUpdateSpectre(_curCreature);
	}

	public void PreviousSpectre()
	{
		creatureIndex = creatureIndex <= 0 ? (creatureIDs.Length - 1) : creatureIndex - 1;
		_curCreature = GameManager.Singleton.AllOwnCreatures[creatureIndex];
		GameManager.Singleton.GUIUpdateSpectre(_curCreature);
	}

	private void SetStatNames()
	{
		string life = Localization.GetText("spectre_life_text");
		string exdmg = Localization.GetText("spectre_extradamage_text");
		string dex = Localization.GetText("spectre_dexterity_text");
		string def = Localization.GetText("spectre_defence_text");
		string reg = Localization.GetText("spectre_regeneration_text");

		_spectreStatsNames.Text = life + "\n" + exdmg + "\n" + dex + "\n" + def + "\n" + reg;

		_equipButton.Text = Localization.GetText("equip_button_text");
        _sendButton.Text = Localization.GetText("send_button_text");
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
		obj.creatureIDs = GameManager.Singleton.Player.creatureIDs;
		return obj;
    }

	void Update()
	{
		Init();
		UpdateSpectre();
	}
}
