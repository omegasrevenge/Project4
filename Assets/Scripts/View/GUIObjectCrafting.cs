using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
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
	private const string CraftingElementPrefix = "crafting_element_";
	private const float ToLessResources = 0.5f;
	private static readonly Color32 Focused = new Color32(127, 127, 127, 255);
	private static readonly Color32 Normal = new Color32(255, 255, 255, 255);

	private dfButton _exitButton;
	private dfControl _box;
	private dfSprite _curElement;
	private dfSprite _nextElement;
	private dfSprite _focusedComponent;
	private Player.Faction _curFaction;

	private List<dfLabel> counterLabels = new List<dfLabel>();
	public List<dfSprite> recourceSprites = new List<dfSprite>();
	private List<Color32> factionColors = new List<Color32>();

	[SerializeField]
	private GameManager.ResourceElement _curResourceElement = GameManager.ResourceElement.energy;
	[SerializeField]
	private GameManager.ResourceLevel curResourceLevel = GameManager.ResourceLevel.biod;

	public bool UpdaetView = true;

	public void UpdateElement()
	{
		for (int i = 0; i < recourceSprites.Count; i++)
		{
			recourceSprites[i].SpriteName = "" + SpriteNamePrefix + (GameManager.ResourceLevel) i + "_" + _curResourceElement;
		}
		_nextElement.SpriteName = "" + NextElementPrefix + _curResourceElement;
		_curElement.SpriteName = "" + CraftingElementPrefix + _curResourceElement;
	}

	private void UpdateRecourceCount()
	{
		for (int i = 0; i < counterLabels.Count; i++)
		{
			int curCount = GameManager.Singleton.Player.Resources[i, (int) _curResourceElement];
			counterLabels[i].Text = curCount.ToString();
			if (curCount <= 0)
			{
				recourceSprites[i].IsInteractive = false;
				recourceSprites[i].Opacity = ToLessResources;
				continue;
			}

			recourceSprites[i].IsInteractive = true;
			recourceSprites[i].Opacity = 1;
			_nextElement.IsInteractive = true;
			_nextElement.Opacity = 1;
		}
	}

	public void MarkForCrafting(dfSprite sprite)
	{
		_nextElement.Color = Normal; 
		foreach (dfSprite s in recourceSprites)
		{
			s.Color = Normal;
		}
		Vector2 spriteCenter = new Vector3(sprite.RelativePosition.x + sprite.Size.x / 2, sprite.RelativePosition.y + sprite.Size.y / 2, 0);
		float dist;
		foreach (dfSprite recourceSprite in recourceSprites)
		{
			if (recourceSprite != sprite && recourceSprite.IsInteractive)
			{
				Vector2 curSpriteCenter = new Vector3(recourceSprite.RelativePosition.x + recourceSprite.Size.x / 2, recourceSprite.RelativePosition.y + recourceSprite.Size.y / 2);
				
				dist = (spriteCenter - curSpriteCenter).magnitude;

				if (dist < sprite.Size.x / 2)
				{
					recourceSprite.Color = Focused;
					_focusedComponent = recourceSprite;
					return;
				}
			}
		}
		Vector2 nextElementCenter = new Vector3(_nextElement.RelativePosition.x + _nextElement.Size.x / 2, _nextElement.RelativePosition.y + _nextElement.Size.y / 2);
		dist = (spriteCenter - nextElementCenter).magnitude;

		if (dist < sprite.Size.x / 2)
		{
			_nextElement.Color = Focused;
			_focusedComponent = _nextElement;
			return;
		}
		_focusedComponent = null;
	}

	public void Craft()
	{
		if (_focusedComponent == null) return;
		_focusedComponent.Color = Normal;
		if (_focusedComponent != _nextElement)
		{
			int craftingLvl = (int) GetResourceLevel(_focusedComponent.SpriteName);
			int diff;
			
			if (craftingLvl > (int)curResourceLevel)
			{
				diff = craftingLvl - (int) curResourceLevel;
				int exchangeAmount;
				if (curResourceLevel == GameManager.ResourceLevel.biod)
				{
					exchangeAmount = (int)Math.Pow(3, diff-1)*10;
				}
				else
				{
					exchangeAmount = (int)Math.Pow(3, diff);
				}
			    SoundController.PlaySound(SoundController.SFXlocation + SoundController.SoundCraftCombine, SoundController.ChannelSFX);
				GameManager.Singleton.Exchange((int)_curResourceElement, (int)curResourceLevel, exchangeAmount, GameManager.ExchangeMode.Up, diff);
			    if (curResourceLevel == GameManager.ResourceLevel.driod_lvl0)
                    GameManager.Singleton.GUICheckIrisSecondCraftingSuccess();
				_focusedComponent = null;
				return;
			}

			diff = (int) curResourceLevel - craftingLvl;
		    SoundController.PlaySound(SoundController.SFXlocation + SoundController.SoundCraftSplit, SoundController.ChannelSFX);
			GameManager.Singleton.Exchange((int)_curResourceElement, (int)curResourceLevel, 1, GameManager.ExchangeMode.Down, diff);
			_focusedComponent = null;
			return;
		}
	    SoundController.PlaySound(SoundController.SFXlocation + SoundController.SoundCraftExchange, SoundController.ChannelSFX);
		GameManager.Singleton.Exchange((int)_curResourceElement, (int)curResourceLevel, 10, GameManager.ExchangeMode.Cricle, 0);
		_focusedComponent = null;
	}

	public void ShowCraftingOptions(string spriteName)
	{
		curResourceLevel = GetResourceLevel(spriteName);
		foreach (dfSprite recourceSprite in recourceSprites)
		{
			recourceSprite.IsInteractive = true;
			recourceSprite.Opacity = 1;
		}

		switch (curResourceLevel)
		{
			case GameManager.ResourceLevel.biod:
			{
				int curCount = GameManager.Singleton.Player.Resources[0, (int)_curResourceElement];
				for (int i = 1; i <  recourceSprites.Count; i++)
				{
					if (curCount - (Math.Pow(3, i-1) * 10) < 0)
					{
						recourceSprites[i].IsInteractive = false;
						recourceSprites[i].Opacity = ToLessResources;
						continue;
					} 
					recourceSprites[i].IsInteractive = true;
					recourceSprites[i].Opacity = 1;
				}

				if (curCount < 10)
				{
					_nextElement.IsInteractive = false;
					_nextElement.Opacity = ToLessResources;
					break;
				}
				_nextElement.IsInteractive = true;
				_nextElement.Opacity = 1;
				break;
			}

			default:
			{
				_nextElement.IsInteractive = false;
				_nextElement.Opacity = ToLessResources;

				int curCount = GameManager.Singleton.Player.Resources[(int)curResourceLevel, (int)_curResourceElement];
				for (int i = (int)curResourceLevel+1; i < recourceSprites.Count; i++)
				{
					if (curCount - (Math.Pow(3, i - (int)curResourceLevel)) < 0)
					{
						recourceSprites[i].IsInteractive = false;
						recourceSprites[i].Opacity = ToLessResources;
						continue;
					}
					recourceSprites[i].IsInteractive = true;
					recourceSprites[i].Opacity = 1;
				}
				break;
			}
		}
	}

	public void NextElement()
	{
		_curResourceElement = (GameManager.ResourceElement)(((int)_curResourceElement + 1) % 5);
	}

	public void PreviousElement()
	{
		_curResourceElement = (GameManager.ResourceElement)(((int)_curResourceElement - 1) == -1 ? 4 : ((int)_curResourceElement - 1));
	}

	private GameManager.ResourceLevel GetResourceLevel(string spriteName)
	{
		spriteName = spriteName.Remove(0, SpriteNamePrefix.Length);
		int startNumber = spriteName.Length - _curResourceElement.ToString().Length;
		spriteName = spriteName.Remove(startNumber, _curResourceElement.ToString().Length);
		spriteName = spriteName.Remove(spriteName.Length - 1);

		for (int i = 0; i <= 6; i++)
		{
			if (spriteName.Equals("" + (GameManager.ResourceLevel) i))
			{
				return (GameManager.ResourceLevel) i;
			}
		}
		return GameManager.ResourceLevel.biod;
	}

	void Awake()
	{
        GameManager.Singleton.GUICheckIrisSecondCrafting();
		factionColors.Add(GameManager.White);
		factionColors.Add(GameManager.Black);

		_exitButton = transform.Find(ExitButtonStr).GetComponent<dfButton>();
		_exitButton.Click +=
				(control, @event) =>
				{
                    SoundController.PlaySound(SoundController.SFXlocation + SoundController.Faction + SoundController.SoundFacClick, SoundController.ChannelSFX);
					Remove();
				};

		Transform rootTransform = transform.Find(CraftingBoxStr);
		_box = rootTransform.GetComponent<dfControl>();
		_nextElement = rootTransform.Find(NextElemtenStr).GetComponent<dfSprite>();
		_curElement = transform.Find(CurElementStr).GetComponent<dfSprite>();

		for (int i = 0; i <= 6; i++)
		{
			counterLabels.Add(rootTransform.Find(LabelPrefix + ((GameManager.ResourceLevel)i).ToString() + LabelSufix).GetComponent<dfLabel>());
		}

		for (int i = 0; i <= 6; i++)
		{
			dfSprite curSprite = rootTransform.Find(SpritePrefix + ((GameManager.ResourceLevel) i).ToString()).GetComponent<dfSprite>();
			HandleDrag.AddHandleDrag(this, curSprite.gameObject, curSprite, true);
			recourceSprites.Add(curSprite);
		}
		HandleDrag.AddHandleDrag(this, _curElement.gameObject, _curElement, true, true);
	}

	private void SetColor(dfPanel guiComponent)
	{
		guiComponent.BackgroundColor = factionColors[(int)_curFaction];
	}

	public static GUIObjectCrafting Create(dfControl root)
    {
        dfControl cntrl = root.AddPrefab(Resources.Load<GameObject>(Prefab));
        cntrl.Size = cntrl.Parent.Size;
        cntrl.RelativePosition = Vector2.zero;
		GUIObjectCrafting obj = cntrl.GetComponent<GUIObjectCrafting>();
		obj._curFaction = GameManager.Singleton.Player.CurrentFaction;
		obj.SetColor((dfPanel)cntrl);
        return obj;
    }

	private void Remove()
	{
		Debug.Log("remove");
		Destroy(gameObject);
	}

	void Update()
	{
		if (_box.IsVisible && UpdaetView)
		{
			UpdateRecourceCount();	
			UpdateElement();
		}
	}
}
