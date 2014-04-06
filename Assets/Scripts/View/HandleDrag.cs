using UnityEngine;
using System.Collections;

public class HandleDrag : MonoBehaviour
{
	public Vector3 startPosition;
	public dfSprite Sprite;
	public dfPanel Panel;
	public bool draged = false;
	public bool inSlot = false;

	private GUIObjectCrafting _crafting;
	private GUIObjectBaseMenue _baseMenue;
	private dfDragHandle dragHandle;
	private bool clampY;

	private bool init = false;
	private bool eleUp = false;
	private bool eleDown = false;

	public static void AddHandleDrag(GUIObjectCrafting crafting, GameObject root, dfSprite sprite, bool addDragHandle, bool clampY = false)
	{
		HandleDrag cntr = root.AddComponent<HandleDrag>();
		cntr._crafting = crafting;
		cntr.Sprite = sprite;
		cntr.clampY = clampY;

		if (addDragHandle)
		{
			GameObject dragHandle = new GameObject("dragHandle");
			dragHandle.transform.parent = root.transform;
			cntr.dragHandle = dragHandle.AddComponent<dfDragHandle>();
			cntr.dragHandle.Position = new Vector3(0, 0, 0);
		}
	}

	public static void AddHandleDrag(GUIObjectBaseMenue baseMenue, GameObject root, dfPanel panel, bool clampY = true)
	{
		HandleDrag cntr = root.AddComponent<HandleDrag>();
		cntr._baseMenue = baseMenue;
		cntr.Panel = panel;
		cntr.clampY = clampY;

		
		GameObject dragHandle = new GameObject("dragHandle");
		dragHandle.transform.parent = root.transform;
		cntr.dragHandle = dragHandle.AddComponent<dfDragHandle>();
		cntr.dragHandle.Position = new Vector3(0, 0, 0);
		
	}

	private void Init()
	{
		if (init) return;
		init = true;

		if (_crafting)
		{
			startPosition = Sprite.RelativePosition;
			dragHandle.Size = new Vector2(Sprite.Size.x, Sprite.Size.y);
			return;
		}

		startPosition = Panel.RelativePosition;
		dragHandle.Size = new Vector2(Panel.Size.x, Panel.Size.y);
	}

	public void OnDragStart(dfControl control, dfDragEventArgs dragEvent)
	{
		Init();
	    SoundController.PlaySound(SoundController.SFXlocation + SoundController.SoundCraftTake, SoundController.ChannelSFX);
		dragEvent.Data = this;
		dragEvent.State = dfDragDropState.Dragging;
		dragEvent.Use();
		draged = true;

		if (_crafting)
		{
			Sprite.BringToFront();
			if (!clampY)
			{
				_crafting.ShowCraftingOptions(Sprite.SpriteName);
				_crafting.UpdaetView = false;
			}
		}
	}

	public void OnDragDrop(dfControl source, dfDragEventArgs args)
	{
		args.State = dfDragDropState.Dropped;
		args.Use();
	}

	

	public void OnDragEnd(dfControl control, dfDragEventArgs dragEvent)
	{
		if (dragEvent.State == dfDragDropState.Denied)return;
		dragEvent.State = dfDragDropState.Denied;


		if (_crafting)
		{
			Sprite.RelativePosition = startPosition;

			_crafting.Craft();
			Sprite.Opacity = 1;
			_crafting.UpdaetView = true;
		}
        else
		    Panel.RelativePosition = startPosition;

		eleUp = false;
		eleDown = false;
		draged = false;
		inSlot = false;
	       
	}

	private void ElementHandling(Vector2 curPosition, dfControl cntr)
	{
		//TODO improve it ;)
        float cntrSize;

		if (_crafting)
		{
			cntrSize = cntr.Size.x;
		}
		else
		{
			cntrSize = cntr.Size.x/3;
		}

		if (curPosition.x > startPosition.x + cntrSize)
		{
			cntr.Opacity = Mathf.Max((Screen.width / curPosition.x) - 0.75f,0.02f);
			if (eleUp) return;

			if (_crafting)
			{
				_crafting.NextElement();
			}
			else
			{
				_baseMenue.NextSpectre();
			}

			eleUp = true;
			return;
		}

		if (curPosition.x < startPosition.x - cntrSize)
		{
			float curOpecity = (curPosition.x / Screen.width) - 0.75f;

			cntr.Opacity = Mathf.Max(1.5f + curOpecity,0.02f);

			if (eleDown) return;

			if (_crafting)
			{
				_crafting.PreviousElement();
			}
			else
			{
				_baseMenue.PreviousSpectre();
			}

			eleDown = true;
			return;
		}

		if (eleUp)
		{
			if (_crafting)
			{
				_crafting.PreviousElement();
			}
			else
			{
				_baseMenue.PreviousSpectre();
			}

			eleUp = false;
			cntr.Opacity = 1;
			return;
		}

		if (eleDown)
		{
			if (_crafting)
			{
				_crafting.NextElement();
			}
			else
			{
				_baseMenue.NextSpectre();
			}

			eleDown = false;
			cntr.Opacity = 1;
		}
	}
	
	void Update() 
	{
		Init();

		if (draged)
		{
			if (_crafting)
			{
				SetPosition(Sprite);
				_crafting.MarkForCrafting(Sprite);

			}
			else
			{
                SetPosition(Panel);
			}
			
		}
	}

	private void SetPosition(dfControl cntr)
	{
		Vector2 position = cntr.GetManager().ScreenToGui(Input.mousePosition);

		Vector2 curPosition;

		if (clampY)
		{
			curPosition = new Vector2(position.x - cntr.Size.x * 0.5f, cntr.RelativePosition.y);

			cntr.RelativePosition = curPosition;

			ElementHandling(curPosition, cntr);

			return;
		}

		curPosition = position - cntr.Size * 0.5f;
		cntr.RelativePosition = curPosition;

	}
}
