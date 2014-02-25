using System.Collections.Generic;
using UnityEngine;

public class GUIObjectBaseMenue : MonoBehaviour 
{
	private const string Prefab = "GUI/panel_basemenue";
	private const string BoxStr = "slot_box";
	private const string DriodSlotStr = "driodslot_";
	private const string EquipSlotSpriteStr = "combat_panel_";
	private const string SlotButtonFocusStr = "combat_slot_active_";
	private const string CraftingSpriteStr = "Base_Interface_CraftingButton_";
	private const string CraftingButtonStr = "button_crafting";
	private const string ExitButtonStr = "button_exit";
	
	private dfButton _craftingButton;
	private dfButton _exitButton;
	private dfPanel _slotBox;
	private Player.Faction _curFaction;

	private List<dfButton> driodSlots = new List<dfButton>();
	private List<Color32> factionColors = new List<Color32>();

	[SerializeField]
	private GameManager.ResourceLevel _curResourceLevel = GameManager.ResourceLevel.biod;

	void Awake()
	{
		factionColors.Add(GameManager.Withe);
		factionColors.Add(GameManager.Black);

		Transform slotTransform = transform.Find(BoxStr);
		_slotBox = slotTransform.GetComponent<dfPanel>();
		_craftingButton = slotTransform.Find(CraftingButtonStr).GetComponent<dfButton>();
		for (int i = 1; i <= 4; i++)
		{
			dfButton curButton = slotTransform.Find(DriodSlotStr + i).GetComponent<dfButton>();

			if (i > GameManager.Singleton.Player.CurCreature.slots.Length)
			{
				curButton.gameObject.SetActive(false);
				continue;
			}

			GUIObjectSlotHandling curSlotHandling = GUIObjectSlotHandling.AddSlotHandling(curButton.gameObject, GameManager.Singleton.Player.CurCreature.slots[i - 1]);

			curButton.FocusSprite = SlotButtonFocusStr + _curFaction.ToString().ToLower();
			curButton.Click +=
				 (control, @event) =>
				 {
					 SoundController.PlaySound(SoundController.SoundClick, SoundController.ChannelSFX);
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
	}

	private void SetColor(dfControl guiComponent)
	{
		guiComponent.Color = factionColors[(int) _curFaction];
		_craftingButton.BackgroundSprite = CraftingSpriteStr + _curFaction.ToString().ToLower();
		_slotBox.BackgroundSprite = EquipSlotSpriteStr + _curFaction.ToString().ToLower();
	}

	public static GUIObjectBaseMenue Create(dfControl root)
	{
		dfControl cntrl = root.AddPrefab(Resources.Load<GameObject>(Prefab));
        cntrl.Size = cntrl.Parent.Size;
        cntrl.RelativePosition = Vector2.zero;
		GUIObjectBaseMenue obj = cntrl.GetComponent<GUIObjectBaseMenue>();
		obj._curFaction = GameManager.Singleton.Player.CurrentFaction;
		obj.SetColor(cntrl);
		return obj;
    }
}
