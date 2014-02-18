using UnityEngine;
using System.Collections;

public class HandleDrag : MonoBehaviour
{
	public Vector3 startPosition;
	public dfSprite sprite;
	public bool draged = false;
	public bool inSlot = false;

	private Vector2 rootPosition;
	private GameObject root;
	private dfDragHandle dragHandle;
	private bool clampY;

	private bool init = false;


	public static void AddHandleDrag(GameObject root, dfSprite sprite, bool addDragHandle, bool clampY = false)
	{
		HandleDrag cntr = root.AddComponent<HandleDrag>();
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

		startPosition = root.transform.position;
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

		gameObject.transform.position = startPosition;
		dragEvent.State = dfDragDropState.Denied;

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
				sprite.RelativePosition = new Vector2(position.x - sprite.Size.x * 0.5f, sprite.RelativePosition.y);
				return;
			}
			sprite.RelativePosition = position - sprite.Size * 0.5f;
		}
	}
}
