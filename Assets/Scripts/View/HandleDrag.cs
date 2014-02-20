using UnityEngine;
using System.Collections;

public class HandleDrag : MonoBehaviour
{
	public Vector3 startPosition;
	public dfSprite sprite;
	public bool draged = false;
	public bool inSlot = false;

	private Vector2 rootPosition;
	private GUIObjectCrafting _crafting;
	private GameObject root;
	private dfDragHandle dragHandle;
	private bool clampY;

	private bool init = false;
	private bool eleUp = false;
	private bool eleDown = false;

	public static void AddHandleDrag(GUIObjectCrafting crafting, GameObject root, dfSprite sprite, bool addDragHandle, bool clampY = false)
	{
		HandleDrag cntr = root.AddComponent<HandleDrag>();
		cntr._crafting = crafting;
		cntr.sprite = sprite;
		cntr.root = root;
		cntr.clampY = clampY;

		if (addDragHandle)
		{
			GameObject dragHandle = new GameObject("dragHandle");
			dragHandle.transform.parent = root.transform;
			cntr.dragHandle = dragHandle.AddComponent<dfDragHandle>();
			cntr.dragHandle.Position = new Vector3(0, 0, 0);
		}
	}

	private void Init()
	{
		if (init) return;

		startPosition = sprite.RelativePosition;
		rootPosition = new Vector2(sprite.GetGUIScreenPos().x - sprite.Size.x, sprite.GetGUIScreenPos().y - sprite.Size.y);

		dragHandle.Size = new Vector2(sprite.Size.x, sprite.Size.y);

		init = true;
	}

	public void OnDragStart(dfControl control, dfDragEventArgs dragEvent)
	{
		Init();
		dragEvent.Data = this;
		dragEvent.State = dfDragDropState.Dragging;
		dragEvent.Use();

		sprite.BringToFront();

		if (!clampY)
		{
			_crafting.ShowCraftingOptions(sprite.SpriteName);
			_crafting.UpdaetView = false;
		}
		draged = true;
	}

	public void OnDragDrop(dfControl source, dfDragEventArgs args)
	{
		//HandleDrag curHandleDrag = (HandleDrag)args.Data;
		//dfSprite curSprite = curHandleDrag.sprite;

		//if (!curHandleDrag.draged) return;

		//if (sprite.SpriteName != "")
		//{
		//	Debug.Log(sprite.SpriteName);
		//	gameController.HandleDrag(gameController.spriteToElement[sprite.SpriteName], 1);
		//}
		//curHandleDrag.inSlot = true;
		//sprite.SpriteName = curSprite.SpriteName;

		//combine.FillSlot();

		args.State = dfDragDropState.Dropped;
		args.Use();
	}

	

	public void OnDragEnd(dfControl control, dfDragEventArgs dragEvent)
	{
		if (dragEvent.State == dfDragDropState.Denied)return;

		sprite.RelativePosition = startPosition;
		dragEvent.State = dfDragDropState.Denied;


		sprite.Opacity = 1;
		_crafting.UpdaetView = true;
		eleUp = false;
		eleDown = false;
		draged = false;
		inSlot = false;
	}
	
	void Update() 
	{
		Init();

		if (draged)
		{
			Vector2 position = sprite.GetManager().ScreenToGui(Input.mousePosition);
			//Debug.Log("position " + position + " rootposition " + rootPosition + " MousePosition " + sprite.GetManager().ScreenToGui(Input.mousePosition));

			if (clampY)
			{
				Vector2 curPosition = new Vector2(position.x - sprite.Size.x*0.5f, sprite.RelativePosition.y);

				//float curOpecity = (Screen.width/curPosition.x) - 0.75f;
				
				//if (curOpecity >= 0)
				//{
				//	sprite.Opacity = curOpecity;
				//}


				sprite.RelativePosition = curPosition;

				if (curPosition.x > startPosition.x + sprite.Size.x)
				{
					sprite.Opacity = (Screen.width/curPosition.x) - 0.75f;
					if (eleUp) return;
					_crafting.NextElement();
					eleUp = true;
					return;
				}

				if (curPosition.x < startPosition.x - sprite.Size.x)
				{
					float curOpecity = (curPosition.x / Screen.width) - 0.75f;

					sprite.Opacity = 1.5f + curOpecity;

					if (eleDown) return;
					_crafting.PreviousElement();
					eleDown = true;
					return;
				}
				
				if (eleUp)
				{
					_crafting.PreviousElement();
					eleUp = false;
					sprite.Opacity = 1;
					return;
				}

				if (eleDown)
				{
					_crafting.NextElement();
					eleDown = false;
					sprite.Opacity = 1;
				}

				return;
			}

			sprite.RelativePosition = position - sprite.Size * 0.5f;
		}
	}
}
