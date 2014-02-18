using System.Collections.Generic;
using UnityEngine;

public class GUIObjectCrafting : MonoBehaviour 
{
	private const string Prefab = "GUI/panel_crafting";
	private const string CraftingBoxStr = "box_crafting";
	private const string LabelPrefix = "label_";
	private const string SpritePrefix = "sprite_";
	private const string SpriteNamePrefix = "crafting_";
	private const string LabelSufix = "_counter";
	private const string ExitButtonStr = "button_exit";
	private const string NextElemtenStr = "sprite_nextElement";
	private const string CurElementStr = "sprite_currentElement";
	private const string NextElementPrefix = "crafting_change_element_";
	
	private dfButton _exitButton;
	private dfControl _box;
	private dfSprite _curElement;
	private dfSprite _nextElement;

	private enum ResourceLevel { biod, driod_lvl0, driod_lvl1, driod_lvl2, driod_lvl3, driod_lvl4, driod_lvl5 };
	
	private List<dfLabel> counterLabels = new List<dfLabel>();
	private List<dfSprite> recourceSprites = new List<dfSprite>();

	[SerializeField]
	private GameManager.ResourceElement _curResourceElement = GameManager.ResourceElement.energy;
	[SerializeField]
	private ResourceLevel curResourceLevel = ResourceLevel.biod;

	public void UpdateElement()
	{
		for (int i = 0; i < recourceSprites.Count; i++)
		{
			recourceSprites[i].SpriteName = "" + SpriteNamePrefix + (ResourceLevel) i + "_" + _curResourceElement;
		}
		_nextElement.SpriteName = "" + NextElementPrefix + _curResourceElement;
	}

	private void UpdateRecourceCount()
	{
		for (int i = 0; i < counterLabels.Count; i++)
		{
			counterLabels[i].Text = GameManager.Singleton.Player.Resources[i, (int) _curResourceElement].ToString();
		}
	}

	void Awake()
	{
		_exitButton = transform.Find(ExitButtonStr).GetComponent<dfButton>();
		_exitButton.Click +=
				(control, @event) =>
				{
					SoundController.PlaySound(SoundController.SoundClick, SoundController.ChannelSFX);
					Remove();
				};

		Transform rootTransform = transform.Find(CraftingBoxStr);
		_box = rootTransform.GetComponent<dfControl>();
		_nextElement = rootTransform.Find(NextElemtenStr).GetComponent<dfSprite>();
		_curElement = transform.Find(CurElementStr).GetComponent<dfSprite>();

		for (int i = 0; i <= 6; i++)
		{
			counterLabels.Add(rootTransform.Find(LabelPrefix + ((ResourceLevel)i).ToString() + LabelSufix).GetComponent<dfLabel>());
		}

		for (int i = 0; i <= 6; i++)
		{
			dfSprite curSprite = rootTransform.Find(SpritePrefix + ((ResourceLevel) i).ToString()).GetComponent<dfSprite>();
			HandleDrag.AddHandleDrag(curSprite.gameObject, curSprite, true);
			recourceSprites.Add(curSprite);
		}
		HandleDrag.AddHandleDrag(_curElement.gameObject, _curElement, true, true);
	}

	public static GUIObjectCrafting Create(dfControl root)
    {
        dfControl cntrl = root.AddPrefab(Resources.Load<GameObject>(Prefab));
        cntrl.Size = cntrl.Parent.Size;
        cntrl.RelativePosition = Vector2.zero;
		GUIObjectCrafting obj = cntrl.GetComponent<GUIObjectCrafting>();
        return obj;
    }

	private void Remove()
	{
		Debug.Log("remove");
		Destroy(gameObject);
	}

	void Update()
	{
		if (_box.IsVisible)
		{
			UpdateRecourceCount();	
			UpdateElement();
		}
	}
}
