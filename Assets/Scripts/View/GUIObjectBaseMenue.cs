using UnityEngine;

public class GUIObjectBaseMenue : MonoBehaviour 
{
    private const string Prefab = "GUI/panel_basemenue";
	private const string BoxStr = "layout_box";
	private const string CraftingButtonStr = "button_crafting";
	private const string CreatureButtonStr = "button_creature";
	private const string ExitButtonStr = "button_exit";
	
	private dfButton _craftingButton;
	private dfButton _creatureButton;
	private dfButton _exitButton;

	void Awake()
	{
		Transform rootTransform = transform.Find(BoxStr);
		_craftingButton = rootTransform.Find(CraftingButtonStr).GetComponent<dfButton>();
		_creatureButton = rootTransform.Find(CreatureButtonStr).GetComponent<dfButton>();
		_exitButton = rootTransform.Find(ExitButtonStr).GetComponent<dfButton>();

		_craftingButton.Click +=
				(control, @event) =>
				{
					SoundController.PlaySound(SoundController.SoundClick, SoundController.ChannelSFX);
					transform.parent.GetComponent<GUIObjectBaseUI>().AddCrafting();
				};

		_creatureButton.Click +=
				(control, @event) =>
				{
					SoundController.PlaySound(SoundController.SoundClick, SoundController.ChannelSFX);
					transform.parent.GetComponent<GUIObjectBaseUI>().AddCreatureLab();
				};

		_exitButton.Click +=
				(control, @event) =>
				{
					SoundController.PlaySound(SoundController.SoundClick, SoundController.ChannelSFX);
					GameManager.Singleton.SwitchGameMode(GameManager.GameMode.Map);
				};
	}

	public static GUIObjectBaseMenue Create(dfControl root)
	{
		dfControl cntrl = root.AddPrefab(Resources.Load<GameObject>(Prefab));
        cntrl.Size = cntrl.Parent.Size;
        cntrl.RelativePosition = Vector2.zero;
		GUIObjectBaseMenue obj = cntrl.GetComponent<GUIObjectBaseMenue>();
        return obj;
    }
}
