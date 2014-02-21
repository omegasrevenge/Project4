using UnityEngine;

public class GUIObjectBaseMenue : MonoBehaviour 
{
	private const string Prefab = "GUI/panel_basemenue";
	private const string BoxStr = "slot_box";
	private const string CraftingButtonStr = "button_crafting";
	private const string ExitButtonStr = "button_exit";
	
	private dfButton _craftingButton;
	private dfButton _exitButton;

	void Awake()
	{
		Transform slotTransform = transform.Find(BoxStr);
		_craftingButton = slotTransform.Find(CraftingButtonStr).GetComponent<dfButton>();
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

	public static GUIObjectBaseMenue Create(dfControl root)
	{
		dfControl cntrl = root.AddPrefab(Resources.Load<GameObject>(Prefab));
        cntrl.Size = cntrl.Parent.Size;
        cntrl.RelativePosition = Vector2.zero;
		GUIObjectBaseMenue obj = cntrl.GetComponent<GUIObjectBaseMenue>();
        return obj;
    }
}
