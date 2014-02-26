using System.Collections.Generic;
using UnityEngine;

public class GUIObjectEquip : MonoBehaviour 
{
	private const string Prefab = "GUI/panel_equip";
	private const string BoxStr = "box_equip";
	private const string ExitButtonStr = "button_exit";
	private const string LabelPrefix = "label_";
	private const string LabelSufix = "_counter";
	private const string SpriteNamePrefix = "crafting_";
	private const string ButtonPrefix = "button_";
	private const float ToLessResources = 0.2f;
	
	private dfButton _exitButton;
	private dfPanel _equipBox;
	private Player.Faction _curFaction;
	private Creature _curCreature;
	private Creature.Slot _slot;

	private List<dfLabel> counterLabels = new List<dfLabel>();
	private List<dfButton> elementButtons = new List<dfButton>();
	private List<dfButton> driodButtons = new List<dfButton>(); 
	private List<Color32> factionColors = new List<Color32>();

	[SerializeField]
	private GameManager.ResourceLevel _curResourceLevel = GameManager.ResourceLevel.biod;
	[SerializeField]
	private GameManager.ResourceElement _curResourceElement = GameManager.ResourceElement.energy;

	private void UpdateRecourceCount()
	{
		for (int i = 0; i < counterLabels.Count; i++)
		{
			int curCount = GameManager.Singleton.Player.Resources[i+1, (int)_curResourceElement];
			counterLabels[i].Text = curCount.ToString();
			
			if (curCount <= 0)
			{
				driodButtons[i].IsInteractive = false;
				driodButtons[i].Opacity = ToLessResources;
				continue;
			}

			driodButtons[i].IsInteractive = true;
			driodButtons[i].Opacity = 1;
		} 
	}

	public void UpdateElement()
	{
		for (int i = 0; i < driodButtons.Count; i++)
		{
			driodButtons[i].BackgroundSprite = "" + SpriteNamePrefix + ((GameManager.ResourceLevel)i+1) + "_" + _curResourceElement;
		}
	}

	void Awake()
	{
		factionColors.Add(GameManager.Withe);
		factionColors.Add(GameManager.Black);

		Transform equipTransform = transform.Find(BoxStr);
		_equipBox = equipTransform.GetComponent<dfPanel>();
		_exitButton = equipTransform.Find(ExitButtonStr).GetComponent<dfButton>(); 
		
		_exitButton.Click +=
				 (control, @event) =>
				 {
					 SoundController.PlaySound(SoundController.SoundClick, SoundController.ChannelSFX);
					 Remove();
				 };

		for (int i = 1; i <= 6; i++)
		{
			counterLabels.Add(equipTransform.Find(LabelPrefix + ((GameManager.ResourceLevel)i).ToString() + LabelSufix).GetComponent<dfLabel>());
		}

		for (int i = 1; i <= 6; i++)
		{
			dfButton curButton = equipTransform.Find(ButtonPrefix + ((GameManager.ResourceLevel)i).ToString()).GetComponent<dfButton>();
			curButton.Click +=
				 (control, @event) =>
				 {
					 SoundController.PlaySound(SoundController.SoundClick, SoundController.ChannelSFX);
					 _curResourceLevel = (GameManager.ResourceLevel)(driodButtons.IndexOf((dfButton)control) + 1);
					 GameManager.Singleton.EquipCreatureSlot(_curCreature.CreatureID, _slot.slotId, (int)_curResourceElement, (int)_curResourceLevel);
					 Remove();
				 };
			driodButtons.Add(curButton);
		}

		for (int i = 0; i < 5; i++)
		{
			dfButton curButton = equipTransform.Find(ButtonPrefix + (GameManager.ResourceElement) i).GetComponent<dfButton>();
			curButton.Click +=
				 (control, @event) =>
				 {
					 SoundController.PlaySound(SoundController.SoundClick, SoundController.ChannelSFX);
					 _curResourceElement = (GameManager.ResourceElement)elementButtons.IndexOf((dfButton) control);
				 };
			elementButtons.Add(curButton);
		}
	}

	private void SetColor()
	{
		_equipBox.BackgroundColor = factionColors[(int)_curFaction];
	}

	public static GUIObjectEquip Create(dfControl root, Creature curCreature, Creature.Slot slot)
	{
		dfControl cntrl = root.AddPrefab(Resources.Load<GameObject>(Prefab));
        cntrl.Size = cntrl.Parent.Size;
        cntrl.RelativePosition = Vector2.zero;
		GUIObjectEquip obj = cntrl.GetComponent<GUIObjectEquip>();
		obj._curFaction = GameManager.Singleton.Player.CurrentFaction;
		obj.SetColor();
		obj._curCreature = curCreature;
		obj._slot = slot;
		return obj;
    }

	void Update()
	{
		UpdateRecourceCount();
		UpdateElement();
	}

	private void Remove()
	{
		Debug.Log("remove");
		Destroy(gameObject);
	}
}
