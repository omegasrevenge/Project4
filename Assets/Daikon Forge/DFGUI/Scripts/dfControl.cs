// *******************************************************
// Copyright 2013 Daikon Forge
// *******************************************************
using UnityEngine;

using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using UnityColor = UnityEngine.Color;
using UnityColor32 = UnityEngine.Color32;

/// <summary>
/// <b>Base class for all GUI controls</b>. Provides common functionality, event
/// handling, and properties for all user interface controls.
/// </summary>
[Serializable]
[ExecuteInEditMode]
[RequireComponent( typeof( BoxCollider ) )]
public abstract class dfControl : MonoBehaviour, IComparable<dfControl>
{

	#region Public events 

	#region Child dfControl Events 

	/// <summary>
	/// Occurs when a control is added to the Controls collection
	/// </summary>
	[HideInInspector]
	public event ChildControlEventHandler ControlAdded;

	/// <summary>
	/// Occurs when a control is removed from the Controls collection
	/// </summary>
	[HideInInspector]
	public event ChildControlEventHandler ControlRemoved;

	#endregion

	#region Focus events 

	/// <summary>
	/// Occurs when the control receives input focus
	/// </summary>
	public event FocusEventHandler GotFocus;

	/// <summary>
	/// Occurs when the control or any of its child controls receives input focus
	/// </summary>
	public event FocusEventHandler EnterFocus;

	/// <summary>
	/// Occurs when the control loses input focus
	/// </summary>
	public event FocusEventHandler LostFocus;

	/// <summary>
	/// Occurs when the control and all of its child controls lose input focus
	/// </summary>
	public event FocusEventHandler LeaveFocus;

	#endregion

	#region Property change events 

	/// <summary>
	/// Occurs when the control's TabIndex property is changed
	/// </summary>
	public event PropertyChangedEventHandler<int> TabIndexChanged;

	/// <summary>
	/// Occurs when the control's Position property is changed
	/// </summary>
	public event PropertyChangedEventHandler<Vector2> PositionChanged;

	/// <summary>
	/// Occurs when the control's Size propert is changed
	/// </summary>
	public event PropertyChangedEventHandler<Vector2> SizeChanged;

	/// <summary>
	/// Occurs when the control's Color value changes
	/// </summary>
	[HideInInspector]
	public event PropertyChangedEventHandler<Color32> ColorChanged;

	/// <summary>
	/// Occurs when the control's Visible property changes
	/// </summary>
	public event PropertyChangedEventHandler<bool> IsVisibleChanged;

	/// <summary>
	/// Occurs when the control's Enabled property value changes
	/// </summary>
	public event PropertyChangedEventHandler<bool> IsEnabledChanged;

	/// <summary>
	/// Occurs when the control's Opacity property value changes
	/// </summary>
	[HideInInspector]
	public event PropertyChangedEventHandler<float> OpacityChanged;

	/// <summary>
	/// Occurs when the control's Anchor property value changes
	/// </summary>
	[HideInInspector]
	public event PropertyChangedEventHandler<dfAnchorStyle> AnchorChanged;

	/// <summary>
	/// Occurs when the control's Pivot property value changes
	/// </summary>
	[HideInInspector]
	public event PropertyChangedEventHandler<dfPivotPoint> PivotChanged;

	/// <summary>
	/// Occurs when the control's ZOrder property value changes
	/// </summary>
	[HideInInspector]
	public event PropertyChangedEventHandler<int> ZOrderChanged;

	#endregion

	#region Drag and Drop events 

	/// <summary> Occurs (on the source dfControl) when a drag-and-drop operation is starting </summary>
	public event DragEventHandler DragStart;

	/// <summary> Occurs (on the source dfControl) when a drag-and-drop operation has ended </summary>
	public event DragEventHandler DragEnd;

	/// <summary> Occurs when a drag-and-drop operation is completed </summary>
	public event DragEventHandler DragDrop;

	/// <summary> Occurs when an object is dragged into the control's bounds </summary>
	public event DragEventHandler DragEnter;

	/// <summary> Occurs when an object is dragged out of the control's bounds </summary>
	public event DragEventHandler DragLeave;

	/// <summary> Occurs when an object is dragged over the control's bounds </summary>
	public event DragEventHandler DragOver;

	#endregion

	#region Mouse and Input Events 

	/// <summary> Occurs when the user presses a key while the control has input focus </summary>
	public event KeyPressHandler KeyPress;

	/// <summary> Occurs when the user presses a key while the control has input focus </summary>
	public event KeyPressHandler KeyDown;

	/// <summary> Occurs when the user releases a key while the control has input focus </summary>
	public event KeyPressHandler KeyUp;

	/// <summary> Occurs when more than one Touch is active for the control </summary>
	public event ControlMultiTouchEventHandler MultiTouch;

	/// <summary> Occurs when the mouse pointer enters the control </summary>
	public event MouseEventHandler MouseEnter;

	/// <summary> Occurs when the mouse pointer is moved over the control </summary>
	public event MouseEventHandler MouseMove;

	/// <summary> Occurs when the mouse pointer rests on the control </summary>
	public event MouseEventHandler MouseHover;

	/// <summary> Occurs when the mouse pointer leaves the control </summary>
	public event MouseEventHandler MouseLeave;

	/// <summary> Occurs when the mouse pointer is over the control and a mouse button is pressed </summary>
	public event MouseEventHandler MouseDown;

	/// <summary> Occurs when a mouse button had previously been pressed on a control and a mouse button is released while the pointer is still over the control </summary>
	public event MouseEventHandler MouseUp;

	/// <summary> Occurs when the mouse wheel moves while the control has focus </summary>
	public event MouseEventHandler MouseWheel;

	/// <summary> Occurs when the control is clicked by the mouse </summary>
	public event MouseEventHandler Click;

	/// <summary> Occurs when the control is double clicked by the mouse </summary>
	public event MouseEventHandler DoubleClick;

	#endregion

	#endregion

	#region Serialized protected fields

	[SerializeField]
	protected bool isEnabled = true;

	[SerializeField]
	protected bool isVisible = true;

	[SerializeField]
	protected bool isInteractive = true;

	[SerializeField]
	protected string tooltip = null;

	[SerializeField]
	protected dfPivotPoint pivot = dfPivotPoint.TopLeft;

	[SerializeField]
	protected int zindex = -1;

	[SerializeField]
	protected Color32 color = new Color32( 255, 255, 255, 255 );

	[SerializeField]
	protected Color32 disabledColor = new Color32( 255, 255, 255, 255 );

	[SerializeField]
	protected Vector2 size = Vector2.zero;

	[SerializeField]
	protected Vector2 minSize = Vector2.zero;

	[SerializeField]
	protected Vector2 maxSize = Vector2.zero;

	[SerializeField]
	protected bool clipChildren = false;

	[SerializeField]
	protected int tabIndex = -1;

	[SerializeField]
	protected bool canFocus = false;

	/// <summary>
	/// Responsible for performing control resizing and layout to maintain 
	/// the anchor style
	/// </summary>
	[SerializeField]
	protected AnchorLayout layout = new AnchorLayout( dfAnchorStyle.Left | dfAnchorStyle.Top );

	/// <summary>
	/// Used by the GUI system to correctly determine the order in which 
	/// controls should be considered during raycasting
	/// </summary>
	[SerializeField]
	protected int renderOrder = -1;

	#endregion

	#region Private non-serialized fields 

	/// <summary>
	/// When set to TRUE, indicates that the control's render information is not 
	/// synchronized with the control's state and must be regenerated
	/// </summary>
	// @private
	protected bool isControlInvalidated = true;

	/// <summary>
	/// Gets or sets the parent container of the control
	/// </summary>
	// @private
	protected dfControl parent = null;

	/// <summary>
	/// The collection of controls contained within the control
	/// </summary>
	// @private
	protected dfList<dfControl> controls = dfList<dfControl>.Obtain();

	/// <summary>
	/// The <see cref="dfGUIManager"/> instance which is responsible for rendering this control
	/// </summary>
	// @private
	protected dfGUIManager manager = null;

	/// <summary>
	/// Used to detect when child nodes have been added to or removed
	/// from the dfControl's <see cref="UnityEngine.Transform"/>
	/// </summary>
	// @private
	protected int cachedChildCount = 0;

	/// <summary>
	/// Used to detect when the user moves the control via the 
	/// transform instead of the Position property
	/// </summary>
	// @private
	protected Vector3 cachedPosition = Vector3.one * float.MinValue;

	/// <summary>
	/// Used to detect when the control's rotation changes, in order
	/// to determine whether the control's render data needs to be rebuilt
	/// </summary>
	// @private
	protected Quaternion cachedRotation = Quaternion.identity;

	/// <summary>
	/// Used to detect when the control's scale changes, in order 
	/// to determine whether the control's render data needs to be rebuilt.
	/// Scale is not used by this library directly, but a developer may 
	/// wish to use scale for hover effects, etc.
	/// </summary>
	// @private
	protected Vector3 cachedScale = Vector3.one;

	/// <summary>
	/// Caching the "world units to pixels" conversion ratio allows the
	/// control to somewhat reduce the number of method calls
	/// </summary>
	// @private
	protected float cachedPixelSize = 0f;

	/// <summary>
	/// Maintains the information needed to render this control
	/// </summary>
	// @private
	protected dfRenderData renderData;

	/// <summary>
	/// Will be set to TRUE when the mouse is over the control. Can be used by 
	/// controls to determine how to display their current state
	/// </summary>
	// @private
	protected bool isMouseHovering = false;

	/// <summary>
	/// Contains user-defined data about the control
	/// </summary>
	// @private
	private new object tag;

	/// <summary>
	/// Will be set to TRUE when the object is being destroyed
	/// </summary>
	// @private
	protected bool isDisposing = false;

	/// <summary>
	/// Set when the control is performing layout, so that recursive 
	/// layouts do not loop forever
	/// </summary>
	// @private
	private bool performingLayout = false;

	/// <summary>
	/// Describes the control's upper-left, upper-right, bottom-left, and bottom-right 
	/// corner positions in world space
	/// </summary>
	// @private
	private Vector3[] cachedCorners = new Vector3[ 4 ];

	/// <summary>
	/// Describes the set of Plane objects that comprise the control's clipping region
	/// </summary>
	// @private
	private Plane[] cachedClippingPlanes = new Plane[ 4 ];

	/// <summary>
	/// This value will be updated each time the dfControl is invalidated, and can 
	/// be compared against a cached value to determine whether a dfControl has 
	/// changed the last time the value was queried.
	/// </summary>
	// @private
	private uint version = 0x00;

	/// <summary>
	/// Global version counter for controls, ensures that each control 
	/// has a unique Version number
	/// </summary>
	// @private
	private static uint versionCounter = 0x00;

	#endregion

	#region Public properties

	/// <summary>
	/// Returns a reference to the <see cref="dfGUIManager"/> instance 
	/// responsible for rendering this <see cref="dfControl"/>
	/// </summary>
	public dfGUIManager GUIManager
	{
		get { return GetManager(); }
	}

	/// <summary>
	/// Gets or sets a value indicating whether the control can respond to 
	/// user interaction.
	/// </summary>
	public bool IsEnabled
	{
		get 
		{
			if( !enabled ) return false;
			if( gameObject != null && !gameObject.activeSelf ) return false;
			return parent != null ? isEnabled && parent.IsEnabled : isEnabled; 
		}
		set
		{
			if( value != isEnabled )
			{
				isEnabled = value;
				OnIsEnabledChanged();
			}
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether the control and all its child 
	/// controls are displayed.
	/// </summary>
	[SerializeField]
	public bool IsVisible
	{
		get 
		{
			return parent == null ? isVisible : isVisible && parent.IsVisible; 
		}
		set
		{
			if( value != this.isVisible )
			{

				if( Application.isPlaying && !this.IsInteractive )
				{
					collider.enabled = false;
				}
				else
				{
					collider.enabled = value;
				}

				isVisible = value;
				OnIsVisibleChanged();

			}
		}
	}

	/// <summary>
	/// Gets/Sets a value indicating whether the control is user-interactive,
	/// ie: Whether the control responds to user input. This flag controls 
	/// whether the control has an current BoxCollider, because there are real
	/// performance implications for having a collider on a large number of
	/// controls that will never process user input.
	/// </summary>
	public virtual bool IsInteractive
	{
		get { return this.isInteractive; }
		set { this.isInteractive = value; }
	}

	/// <summary>
	/// The tooltip to be displayed to the user when the mouse hovers over 
	/// the control. Currently not used directly by this library.
	/// </summary>
	[SerializeField]
	public string Tooltip
	{
		get { return this.tooltip; }
		set
		{
			if( value != tooltip )
			{
				tooltip = value;
				Invalidate();
			}
		}
	}

	/// <summary>
	/// Gets or sets the edges of the container to which a control is bound 
	/// and determines how a control is resized with its parent. 
	/// </summary>
	[SerializeField]
	public dfAnchorStyle Anchor
	{
		get 
		{
			ensureLayoutExists();
			return this.layout.AnchorStyle; 
		}
		set
		{
			ensureLayoutExists();
			if( value != this.layout.AnchorStyle )
			{

				this.layout.AnchorStyle = value;

				Invalidate();
				OnAnchorChanged();

			}
		}
	}

	/// <summary>
	/// Gets or sets the opacity level of the control
	/// </summary>
	public float Opacity
	{
		get { return (float)this.color.a / 255f; }
		set
		{
			value = Mathf.Max( 0, Mathf.Min( 1, value ) );
			var alpha = (float)this.color.a / 255f;
			if( value != alpha )
			{
				this.color.a = (byte)( value * 255 );
				OnOpacityChanged();
			}
		}
	}

	/// <summary>
	/// Gets or sets the topColor of the control
	/// </summary>
	public Color32 Color
	{
		get { return this.color; }
		set
		{
			if( !color.Equals( value ) )
			{
				color = value;
				OnColorChanged();
			}
		}
	}

	/// <summary>
	/// Gets or sets the topColor that will be used when this control is disabled
	/// </summary>
	public UnityColor32 DisabledColor
	{
		get { return this.disabledColor; }
		set
		{
			if( !value.Equals( disabledColor ) )
			{
				disabledColor = value;
				Invalidate();
			}
		}
	}

	/// <summary>
	/// Gets or sets the Pivot Point of the control
	/// </summary>
	public dfPivotPoint Pivot
	{
		get { return pivot; }
		set
		{

			if( value != pivot )
			{

				var pos = Position;

				pivot = value;
				var offset = Position - pos;

				SuspendLayout();

				Position = pos;

				for( int i = 0; i < controls.Count; i++ )
				{
					controls[ i ].Position += offset;
				}

				ResumeLayout();

				OnPivotChanged();

			}

		}
	}

	/// <summary>
	/// Returns the relative coordinates of the upper-left corner of the control
	/// relative to the upper-left corner of the parent control, expressed in 
	/// "screen space" coordinates (left-top origin): x increases right, 
	/// y increases down
	/// </summary>
	public Vector3 RelativePosition
	{
		get
		{
			return getRelativePosition();
		}
		set
		{
			setRelativePosition( value );
		}
	}

	/// <summary>
	/// Gets or sets the local position of the upper-left
	/// corner of the control relative to its container's 
	/// pivot point
	/// </summary>
	public Vector3 Position
	{
		get
		{
			var transformPos = transform.localPosition / PixelsToUnits();
			return transformPos + pivot.TransformToUpperLeft( Size );
		}
		set
		{
			setPositionInternal( value );
		}
	}

	/// <summary>
	/// Gets or sets the size (in pixels) of the control
	/// </summary>
	public Vector2 Size
	{
		get { return this.size; }
		set
		{

			// Enforce minimum size
			value = Vector2.Max( CalculateMinimumSize(), value );

			// Enforce maximum size, if specified (value of 0 means "unlimited")
			value.x = maxSize.x > 0f ? Mathf.Min( value.x, maxSize.x ) : value.x;
			value.y = maxSize.y > 0f ? Mathf.Min( value.y, maxSize.y ) : value.y;

			// If there is no significant difference between the current size and
			// the specified size, then just exit without doing anything.
			if( ( value - size ).sqrMagnitude <= float.Epsilon )
				return;

			// Assign the new size value
			size = value;

			// Notify any listeners that this control's Size has changed
			OnSizeChanged();

		}
	}

	/// <summary>
	/// Gets or sets the width of the control in pixels
	/// </summary>
	public float Width
	{
		get { return size.x; }
		set { Size = new Vector2( value, size.y ); }
	}

	/// <summary>
	/// Gets or sets the height of the control in pixels
	/// </summary>
	public float Height
	{
		get { return size.y; }
		set { Size = new Vector2( size.x, value ); }
	}

	/// <summary>
	/// Gets or sets the minimum allowed size of the control
	/// </summary>
	public Vector2 MinimumSize
	{
		get { return this.minSize; }
		set
		{
			value = Vector2.Max( Vector2.zero, value.RoundToInt() );
			if( value != minSize )
			{
				minSize = value;
				Invalidate();
			}
		}
	}

	/// <summary>
	/// Gets or sets the maximum allowed size of the control
	/// </summary>
	public Vector2 MaximumSize
	{
		get { return this.maxSize; }
		set
		{
			value = Vector2.Max( Vector2.zero, value.RoundToInt() );
			if( value != maxSize )
			{
				maxSize = value;
				Invalidate();
			}
		}
	}

	/// <summary>
	/// Gets/Sets a value indicating the rendering order of this control
	/// </summary>
	[HideInInspector]
	public int ZOrder
	{
		get 
		{ 
			return this.zindex; 
		}
		set
		{
			if( value != zindex )
			{

				this.zindex = Mathf.Max( 0, value );
				Invalidate();

				if( parent != null )
				{
					parent.SetControlIndex( this, value );
				}

				OnZOrderChanged();
					
			}
		}
	}

	/// <summary>
	/// Gets or sets the tab order of the control within its container. Set this
	/// value to -1 to remove this control from the tab order.
	/// </summary>
	[HideInInspector]
	public int TabIndex
	{
		get { return this.tabIndex; }
		set
		{
			if( value != this.tabIndex )
			{
				this.tabIndex = Mathf.Max( -1, value );
				OnTabIndexChanged();
			}
		}
	}

	/// <summary>
	/// Gets the collection of controls contained within the control
	/// </summary>
	public IList<dfControl> Controls
	{
		get { return this.controls; }
	}

	/// <summary>
	/// Gets the parent container of the control
	/// </summary>
	public dfControl Parent
	{
		get { return this.parent; }
	}

	/// <summary>
	/// Indicates whether child controls will be clipped to the bounds 
	/// of this control
	/// </summary>
	public bool ClipChildren
	{
		get { return this.clipChildren; }
		set
		{
			if( value != this.clipChildren )
			{
				this.clipChildren = value;
				Invalidate();
			}
		}
	}

	/// <summary>
	/// Returns a value indicating whether the dfControl's layout engine is 
	/// currently suspended
	/// </summary>
	protected bool IsLayoutSuspended
	{
		get
		{
			return ( performingLayout || ( layout != null && layout.IsLayoutSuspended ) );
		}
	}

	/// <summary>
	/// Returns TRUE if the control is currently performing a layout operation
	/// and FALSE otherwise
	/// </summary>
	protected bool IsPerformingLayout
	{
		get
		{

			if( performingLayout )
				return true;

			if( layout != null && layout.IsPerformingLayout )
				return true;

			//if( parent != null && parent.IsPerformingLayout )
			//    return false;

			return false;

		}
	}

	/// <summary>
	/// Can be used to store additional data about the control instance
	/// </summary>
	public object Tag
	{
		get { return this.tag; }
		set { this.tag = value; }
	}

	/// <summary>
	/// Represents a dfControl's "version number", which is changed each
	/// time a dfControl's properties change. Can be compared against a cached
	/// value to determine whether the dfControl has changed since the last 
	/// time the value was queried.
	/// </summary>
	internal uint Version
	{
		get { return this.version; }
	}

	#endregion

	#region Members used by GUIManager class and intended to be overridden 

	/// <summary>
	/// Gets a value indicating whether the control can receive focus.
	/// </summary>
	public virtual bool CanFocus 
	{ 
		get { return this.canFocus; }
		set { this.canFocus = value; }
	}

	/// <summary>
	/// Gets a value indicating whether the control, or one of its child controls, currently has the input focus.
	/// </summary>
	public virtual bool ContainsFocus
	{
		get { return dfGUIManager.ContainsFocus( this ); }
	}

	/// <summary>
	/// Gets a value indicating whether the control has user input focus.
	/// </summary>
	public virtual bool HasFocus 
	{
		get { return dfGUIManager.HasFocus( this ); }
	}

	/// <summary>
	/// Returns TRUE when the mouse is contained within the bounds of the control
	/// </summary>
	public bool ContainsMouse
	{
		get { return this.isMouseHovering; }
	}

	#region Raycast order management - Used for rendering and raycasting 

	internal void setRenderOrder( ref int order )
	{

		//updateControlHeirarchy();
		this.renderOrder = ++order; 

		for( int i = 0; i < controls.Count; i++ )
		{
			controls[ i ].setRenderOrder( ref order );
		}

	}

	/// <summary>
	/// Returns the order in which this dfControl will be rendered.
	/// </summary>
	[HideInInspector]
	public int RenderOrder { get { return renderOrder; } }

	#endregion

	#region Drag-and-Drop events 

	/// <summary>
	/// Called by the <see cref="dfInputManager"/> when a drag operation 
	/// is requested on the control. 
	/// </summary>
	/// <param name="args">Contains information regarding the drag operation.
	/// Set the <paramref name="args.State"/> property to <see cref="dfDragDropState.Dragging"/>
	/// to allow the drag operation to continue, or <see cref="dfDragDropState.Denied"/> to 
	/// prevent the drag operation</param>
	internal virtual void OnDragStart( dfDragEventArgs args )
	{

		if( !args.Used )
		{

			Signal( "OnDragStart", args );

			if( !args.Used && DragStart != null )
			{
				DragStart( this, args );
			}

		}

		if( parent != null )
		{
			parent.OnDragStart( args );
		}

	}

	/// <summary>
	/// Called by the <see cref="dfInputManager"/> when a drag operation 
	/// for this control instance has completed.
	/// </summary>
	/// <param name="args">Contains information regarding the drag operation. Query the 
	/// <paramref name="args.State"/> property for the reason for the drag operation ending.
	/// This property will contain <see cref="dfDragDropState.Dropped"/> if the control 
	/// was dropped on a valid target, <see cref="dfDragDropState.Denied"/> if the drop
	/// operation was denied, <see cref="dfDragDropState.Cancelled"/> if the user cancelled
	/// the drag-and-drop operation (by pressing ESC for instance), or 
	/// <see cref="dfDragDropState.CancelledNoTarget"/> if the control was dropped without 
	/// being dropped on a valid drop target. If <see cref="args.State"/> is set to 
	/// <see cref="dfDragDropState.CancelledNoTarget"/> then args.Ray can be used to 
	/// perform a raycast to see where the control was dropped (such as a user dropping
	/// an inventory item on the ground, etc)</param>
	internal virtual void OnDragEnd( dfDragEventArgs args )
	{

		if( !args.Used )
		{

			Signal( "OnDragEnd", args );

			if( !args.Used && DragEnd != null )
			{
				DragEnd( this, args );
			}

		}

		if( parent != null )
		{
			parent.OnDragEnd( args );
		}

	}

	/// <summary>
	/// Called by the <see cref="dfInputManager"/> when another control is dropped
	/// on this control instance.
	/// </summary>
	/// <param name="args">Contains information about the drop operation. Set 
	/// <paramref name="args.State"/> to <see cref="dfDragDropState.Dropped"/> or 
	/// call <paramref name="args.Use()"/> to indicate a successful drop operation.</param>
	internal virtual void OnDragDrop( dfDragEventArgs args )
	{

		if( !args.Used )
		{

			Signal( "OnDragDrop", args );

			if( !args.Used && DragDrop != null )
			{
				DragDrop( this, args );
			}

		}

		if( parent != null )
		{
			parent.OnDragDrop( args );
		}

	}

	/// <summary>
	/// Called by the <see cref="dfInputManager"/> when another control is being 
	/// dragged over this control and has just entered the bounds of this control.
	/// </summary>
	/// <param name="args">Contains information about the drag operation</param>
	internal virtual void OnDragEnter( dfDragEventArgs args )
	{

		if( !args.Used )
		{

			Signal( "OnDragEnter", args );

			if( !args.Used && DragEnter != null )
			{
				DragEnter( this, args );
			}

		}

		if( parent != null )
		{
			parent.OnDragEnter( args );
		}

	}

	/// <summary>
	/// Called by the <see cref="dfInputManager"/> when another control is being 
	/// dragged over this control and has just left the bounds of this control.
	/// </summary>
	/// <param name="args">Contains information about the drag operation</param>
	internal virtual void OnDragLeave( dfDragEventArgs args )
	{

		if( !args.Used )
		{

			Signal( "OnDragLeave", args );

			if( !args.Used && DragLeave != null )
			{
				DragLeave( this, args );
			}

		}

		if( parent != null )
		{
			parent.OnDragLeave( args );
		}

	}

	/// <summary>
	/// Called by the <see cref="dfInputManager"/> when another control is being 
	/// dragged over this control and was already within the bounds of this control
	/// </summary>
	/// <param name="args">Contains information about the drag operation</param>
	internal virtual void OnDragOver( dfDragEventArgs args )
	{

		if( !args.Used )
		{

			Signal( "OnDragOver", args );

			if( !args.Used && DragOver != null )
			{
				DragOver( this, args );
			}

		}

		if( parent != null )
		{
			parent.OnDragOver( args );
		}

	}

	#endregion

	#region Multi-touch events 

	/// <summary>
	/// Called by the <see cref="dfInputManager"/> when there is more than one 
	/// Touch event active for this control
	/// </summary>
	/// <param name="args">Contains information for all active Touch events</param>
	protected internal virtual void OnMultiTouch( dfTouchEventArgs args )
	{

		if( !args.Used )
		{

			Signal( "OnMultiTouch", args );

			if( MultiTouch != null )
			{
				MultiTouch( this, args );
			}

		}

		if( parent != null )
		{
			parent.OnMultiTouch( args );
		}

	}

	#endregion

	#region Mouse events

	/// <summary>
	/// Called by the <see cref="dfInputManager"/> when the mouse enters the
	/// bounds of this control
	/// </summary>
	/// <param name="args">Contains information about the action that triggered this event</param>
	protected internal virtual void OnMouseEnter( dfMouseEventArgs args )
	{

		this.isMouseHovering = true;
		
		if( !args.Used )
		{

			Signal( "OnMouseEnter", args );

			if( MouseEnter != null )
			{
				MouseEnter( this, args );
			}

		}

		if( parent != null )
		{
			parent.OnMouseEnter( args );
		}

	}

	/// <summary>
	/// Called by the <see cref="dfInputManager"/> when the mouse leaves the
	/// bounds of this control
	/// </summary>
	/// <param name="args">Contains information about the action that triggered this event</param>
	protected internal virtual void OnMouseLeave( dfMouseEventArgs args )
	{

		isMouseHovering = false;

		if( !args.Used )
		{

			Signal( "OnMouseLeave", args );

			if( MouseLeave != null )
			{
				MouseLeave( this, args );
			}

		}

		if( parent != null )
		{
			parent.OnMouseLeave( args );
		}

	}

	/// <summary>
	/// Called by the <see cref="dfInputManager"/> when the mouse is being moved
	/// over this control.
	/// </summary>
	/// <param name="args">Contains information about the action that triggered this event</param>
	protected internal virtual void OnMouseMove( dfMouseEventArgs args )
	{

		if( !args.Used )
		{

			Signal( "OnMouseMove", args );

			if( MouseMove != null )
			{
				MouseMove( this, args );
			}

		}

		if( parent != null )
		{
			parent.OnMouseMove( args );
		}

	}

	/// <summary>
	/// Called by the <see cref="dfInputManager"/> periodically when the mouse 
	/// is within the bounds of this control but is not being moved by the user.
	/// </summary>
	/// <param name="args">Contains information about the action that triggered this event</param>
	protected internal virtual void OnMouseHover( dfMouseEventArgs args )
	{

		if( !args.Used )
		{

			Signal( "OnMouseHover", args );

			if( MouseHover != null )
			{
				MouseHover( this, args );
			}

		}

		if( parent != null )
		{
			parent.OnMouseHover( args );
		}

	}

	/// <summary>
	/// Called by the <see cref="dfInputManager"/> when the user rotates the
	/// mouse scroll wheel
	/// </summary>
	/// <param name="args">Contains information about the mouse action that triggered this event</param>
	protected internal virtual void OnMouseWheel( dfMouseEventArgs args )
	{

		if( !args.Used )
		{

			Signal( "OnMouseWheel", args );

			if( MouseWheel != null )
			{
				MouseWheel( this, args );
			}

		}

		if( parent != null )
		{
			parent.OnMouseWheel( args );
		}

	}

	/// <summary>
	/// Called by <see cref="dfInputManager"/> when the user presses a mouse
	/// button while the mouse is over this control
	/// </summary>
	/// <param name="args">Contains information about the mouse action that triggered this event</param>
	protected internal virtual void OnMouseDown( dfMouseEventArgs args )
	{

		var canSetFocus =
			Opacity > 0.01f &&
			IsVisible &&
			IsEnabled &&
			this.CanFocus &&
			!this.ContainsFocus;

		if( canSetFocus )
		{
			this.Focus();
		}

		if( !args.Used )
		{

			Signal( "OnMouseDown", args );

			if( MouseDown != null )
			{
				MouseDown( this, args );
			}

		}

		if( parent != null )
		{
			parent.OnMouseDown( args );
		}

	}

	/// <summary>
	/// Called by <see cref="dfInputManager"/> when the user releases a mouse
	/// button while the mouse is over this control
	/// </summary>
	/// <param name="args">Contains information about the mouse action that triggered this event</param>
	protected internal virtual void OnMouseUp( dfMouseEventArgs args )
	{

		if( !args.Used )
		{

			Signal( "OnMouseUp", args );

			if( MouseUp != null )
			{
				MouseUp( this, args );
			}

		}

		if( parent != null )
		{
			parent.OnMouseUp( args );
		}

	}

	/// <summary>
	/// Processes a user click event
	/// </summary>
	protected internal virtual void OnClick( dfMouseEventArgs args )
	{

		if( !args.Used )
		{

			Signal( "OnClick", args );

			if( Click != null )
			{
				Click( this, args );
			}

		}

		if( parent != null )
		{
			parent.OnClick( args );
		}

	}

	/// <summary>
	/// Processes a user double-click event
	/// </summary>
	protected internal virtual void OnDoubleClick( dfMouseEventArgs args )
	{

		if( !args.Used )
		{

			Signal( "OnDoubleClick", args );

			if( DoubleClick != null )
			{
				DoubleClick( this, args );
			}

		}

		if( parent != null )
		{
			parent.OnDoubleClick( args );
		}

	}

	#endregion

	#region Keyboard events 

	/// <summary>
	/// Called by <see cref="dfInputManager"/> when the user presses a key
	/// while this control contains input focus. This method differs from
	/// <see cref="OnKeyDown"/> in that it is called to process user 
	/// text input events rather than for control key events.
	/// </summary>
	/// <param name="args">Contains information about the keyboard action that triggered this event</param>
	protected internal virtual void OnKeyPress( dfKeyEventArgs args )
	{

		if( !args.Used )
		{

			Signal( "OnKeyPress", args );

			if( KeyPress != null )
			{
				KeyPress( this, args );
			}

		}

		if( parent != null )
		{
			parent.OnKeyPress( args );
		}

	}

	/// <summary>
	/// Called by <see cref="dfInputManager"/> when the user presses a key
	/// while this control contains input focus. This method differs from 
	/// <see cref="OnKeyPress"/> in that it is called to process control key 
	/// events rather than text input events
	/// </summary>
	/// <param name="args">Contains information about the keyboard action that triggered this event</param>
	protected internal virtual void OnKeyDown( dfKeyEventArgs args )
	{

		if( !args.Used )
		{

			Signal( "OnKeyDown", args );

			if( KeyDown != null )
			{
				KeyDown( this, args );
			}

		}

		if( parent != null )
		{
			parent.OnKeyDown( args );
		}

	}

	/// <summary>
	/// Called by <see cref="dfInputManager"/> when the user releases a key
	/// while this control contains user input focus
	/// </summary>
	/// <param name="args">Contains information about the keyboard action that triggered this event</param>
	protected internal virtual void OnKeyUp( dfKeyEventArgs args )
	{

		Signal( "OnKeyUp", args );

		if( KeyUp != null )
		{
			KeyUp( this, args );
		}

		if( parent != null )
		{
			parent.OnKeyUp( args );
		}

	}

	#endregion

	#region Focus events

	/// <summary>
	/// Called by <see cref="dfGUIManager"/> when this control or one of its
	/// children (at any level) obtains user input focus. This differs from 
	/// the <see cref="OnGotFocus"/> event in that <see cref="OnGotFocus"/>
	/// refers specifically to the control itself gaining input focus, while
	/// this method will be called if any control lower in the hierarchy 
	/// gains input focus. This is analogous to the difference between the
	/// <see cref="HasFocus"/> and <see cref="ContainsFocus"/> members.
	/// </summary>
	/// <param name="args">Contains information about the focus change event</param>
	protected internal virtual void OnEnterFocus( dfFocusEventArgs args )
	{

		Signal( "OnEnterFocus", args );

		if( EnterFocus != null )
		{
			EnterFocus( this, args );
		}

		// NOTE: This event does not bubble up the hierarchy,
		// the UI manager sends it directly to each control 
		// individually

	}

	/// <summary>
	/// Called by <see cref="dfGUIManager"/> when this control and all of its
	/// child controls (at any level) no longer contain user input focus. This 
	/// differs from <see cref="OnLostFocus"/> in that <see cref="OnLostFocus"/>
	/// refers specifically to the control itself losing input focus, which 
	/// this method is only called when all controls lower in the hierarchy no
	/// longer have input focus. This is analogous to the difference between the
	/// <see cref="HasFocus"/> and <see cref="ContainsFocus"/> members.
	/// </summary>
	/// <param name="args">Contains information about the focus change event</param>
	protected internal virtual void OnLeaveFocus( dfFocusEventArgs args )
	{

		Signal( "OnLeaveFocus", args );

		if( LeaveFocus != null )
		{
			LeaveFocus( this, args );
		}

		// NOTE: This event does not bubble up the hierarchy,
		// the UI manager sends it directly to each control 
		// individually

	}

	/// <summary>
	/// Called by <see cref="dfGUIManager"/> when the control gains input focus
	/// </summary>
	/// <param name="args">Contains information about the focus change event</param>
	protected internal virtual void OnGotFocus( dfFocusEventArgs args )
	{

		if( !args.Used )
		{

			Signal( "OnGotFocus", args );

			if( GotFocus != null )
			{
				GotFocus( this, args );
			}

		}

		if( parent != null )
		{
			parent.OnGotFocus( args );
		}

	}

	/// <summary>
	/// Called by <see cref="dfGUIManager"/> when the control loses input focus
	/// </summary>
	/// <param name="args">Contains information about the focus change event</param>
	protected internal virtual void OnLostFocus( dfFocusEventArgs args )
	{

		if( !args.Used )
		{

			Signal( "OnLostFocus", args );

			if( LostFocus != null )
			{
				LostFocus( this, args );
			}

		}

		if( parent != null )
		{
			parent.OnLostFocus( args );
		}

	}

	#endregion

	/// <summary>
	/// Raises the named event. This method is only provided for the use of 
	/// derived classes, which cannot directly raise events on a base class
	/// due to constraints in the C# language specification.
	/// </summary>
	/// <param name="eventName">The name of the event to be raise (Click, MouseDown, etc)</param>
	/// <param name="args">The parameters to be passed to the event</param>
	[HideInInspector]
	protected internal void RaiseEvent( string eventName, params object[] args )
	{

		var eventField =
			this.GetType()
			.GetAllFields()
			.Where( f => f.Name == eventName )
			.FirstOrDefault();

		if( eventField != null )
		{

			var eventDelegate = eventField.GetValue( this );
			if( eventDelegate != null )
			{
				( (Delegate)eventDelegate ).DynamicInvoke( args );
			}

		}

	}

	/// <summary>
	/// Performs a SendMessage()-like event notification by searching the GameObject
	/// for components which have a method with the same name as the <paramref name="eventName"/>
	/// parameter and which have a signature that matches the types in the 
	/// <paramref name="args"/> array. 
	/// </summary>
	/// <param name="eventName">The name of the method to invoke</param>
	/// <param name="args">The parameters that will be passed to the method</param>
	/// <returns>Returns TRUE if a matching event handler was found and invoked</returns>
	protected internal bool Signal( string eventName, params object[] args )
	{
		return Signal( this.gameObject, eventName, args );
	}

	/// <summary>
	/// Performs a SendMessage()-like event notification by searching the GameObject
	/// for components which have a method with the same name as the <paramref name="eventName"/>
	/// parameter and which have a signature that matches the types in the 
	/// <paramref name="args"/> array. This function will walk up the object hierarchy until
	/// it finds a component with a matching event handler.
	/// </summary>
	/// <param name="eventName">The name of the method to invoke</param>
	/// <param name="args">The parameters that will be passed to the method</param>
	/// <returns>Returns TRUE if a matching event handler was found and invoked</returns>
	protected internal bool SignalHierarchy( string eventName, params object[] args )
	{

		var signalReceived = false;
		var loop = transform;

		while( !signalReceived && loop != null )
		{
			signalReceived = Signal( loop.gameObject, eventName, args );
			loop = loop.parent;
		}

		return signalReceived;

	}

	/// <summary>
	/// Performs a SendMessage()-like event notification by searching the GameObject
	/// for components which have a method with the same name as the <paramref name="eventName"/>
	/// parameter and which have a signature that matches the types in the 
	/// <paramref name="args"/> array. 
	/// </summary>
	/// <param name="target">The GameObject on which to raise the event</param>
	/// <param name="eventName">The name of the method to invoke</param>
	/// <param name="args">The parameters that will be passed to the method</param>
	/// <returns>Returns TRUE if a matching event handler was found and invoked</returns>
	[HideInInspector]
	protected internal bool Signal( GameObject target, string eventName, params object[] args )
	{

		// Retrieve the list of MonoBehaviour instances on the target object
		var components = target.GetComponents( typeof( MonoBehaviour ) );

		// Exit early if there are no attached behaviors or this dfControl is 
		// the only attached behavior
		if( components == null || ( target == this.gameObject && components.Length == 1 ) )
			return false;

		// Need to ensure that the 'this' pointer is always sent with the 
		// event arguments. This also has the side benefit of differentiating
		// the method signatures from the built-in methods
		if( args.Length == 0 || !object.ReferenceEquals( args[ 0 ], this ) )
		{
			var newArgs = new object[ args.Length + 1 ];
			Array.Copy( args, 0, newArgs, 1, args.Length );
			newArgs[ 0 ] = this;
			args = newArgs;
		}

		// Compile a list of Type definitions that defines the desired method signature
		var paramTypes = new Type[ args.Length ];
		for( int i = 0; i < paramTypes.Length; i++ )
		{
			if( args[ i ] == null )
			{
				paramTypes[ i ] = typeof( object );
			}
			else
			{
				paramTypes[ i ] = args[ i ].GetType();
			}
		}

		bool wasHandled = false;

		for( int i = 0; i < components.Length; i++ )
		{

			var component = components[ i ];

			// Should never happen, but seems to happen occasionally during a 
			// long recompile in the Editor. Unity bug?
			if( component == null || component.GetType() == null )
				continue;

			if( component is MonoBehaviour && !( (MonoBehaviour)component ).enabled )
				continue;

			if( component == this )
				continue;

			#region First try to find a MethodInfo with the exact signature

			var handlerWithParams = component.GetType().GetMethod(
				eventName,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
				null,
				paramTypes,
				null
			);

			if( handlerWithParams != null )
			{
				handlerWithParams.Invoke( component, args );
				wasHandled = true;
				continue;
			}

			#endregion

			if( args.Length == 0 )
				continue;

			#region Look for a parameterless method with the given name

			var handlerWithoutParams = component.GetType().GetMethod(
				eventName,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
				null,
				Type.EmptyTypes,
				null
			);

			if( handlerWithoutParams != null )
			{
				handlerWithoutParams.Invoke( component, null );
				wasHandled = true;
			}

			#endregion

		}

		return wasHandled;

	}

	#endregion

	#region Public methods 

	/// <summary>
	/// Simulates the user clicking on the control
	/// </summary>
	public void DoClick()
	{

		var renderCamera = GetCamera();
		var location = renderCamera.WorldToScreenPoint( GetCenter() );
		var ray = renderCamera.ScreenPointToRay( location );

		this.OnClick( new dfMouseEventArgs( this, dfMouseButtons.Left, 1, ray, location, 0f ) );

	}

	/// <summary>
	/// Detaches all event handlers for the named event
	/// </summary>
	/// <param name="EventName"></param>
	[HideInInspector]
	protected internal void RemoveEventHandlers( string EventName )
	{

		var namedEvent =
			this.GetType()
			.GetAllFields()
			.Where( f => 
				typeof( Delegate ).IsAssignableFrom( f.FieldType ) &&
				f.Name == EventName
			)
			.FirstOrDefault();

		if( namedEvent != null )
		{
			namedEvent.SetValue( this, null );
		}

	}

	/// <summary>
	/// Detaches all event handlers for all events on this <see cref="dfControl"/>
	/// </summary>
	[HideInInspector]
	internal void RemoveAllEventHandlers()
	{

		var events =
			this.GetType()
			.GetAllFields()
			.Where( f => typeof( Delegate ).IsAssignableFrom( f.FieldType ) )
			.ToArray();

		for( int i = 0; i < events.Length; i++ )
		{
			events[ i ].SetValue( this, null );
		}

	}

	/// <summary>
	/// Show the control. Bindable alternative to setting IsVisible = true
	/// </summary>
	public void Show()
	{
		this.IsVisible = true;
	}

	/// <summary>
	/// Hide the control. Bindable alternative to setting IsVisible = false
	/// </summary>
	public void Hide()
	{
		this.IsVisible = false;
	}

	/// <summary>
	/// Enable the control. Bindable alternative to setting IsEnabled = true
	/// </summary>
	public void Enable()
	{
		this.IsEnabled = true;
	}

	/// <summary>
	/// Disables the control. Bindable alternative to setting IsEnabled = false
	/// </summary>
	public void Disable()
	{
		this.IsEnabled = false;
	}

	/// <summary>
	/// Returns the relative position in screen coordinates (X increases to the right, 
	/// Y increases downward, top-left origin) of the point where the ray intersects this control. Returns 
	/// TRUE if the ray intersects the control and assigns the relative hit location to 
	/// the <paramref name="position"/> argument.
	/// </summary>
	public bool GetHitPosition( Ray ray, out Vector2 position )
	{

		position = Vector2.one * float.MinValue;

		var plane = new Plane( transform.TransformDirection( Vector3.back ), transform.position );

		var distance = 0f;
		if( !plane.Raycast( ray, out distance ) )
		{
			return false;
		}

		var hit = ray.origin + ray.direction * distance;

		var planes = ClipChildren ? this.GetClippingPlanes() : null;
		if( planes != null && planes.Length > 0 )
		{
			for( int i = 0; i < planes.Length; i++ )
			{
				if( !planes[ i ].GetSide( hit ) )
				{
					return false;
				}
			}
		}

		var corners = GetCorners();
		var ul = corners[ 0 ];
		var ur = corners[ 1 ];
		var bl = corners[ 2 ];

		var closest = closestPointOnLine( ul, ur, hit, true );
		var lerp = ( closest - ul ).magnitude / ( ur - ul ).magnitude;
		var x = size.x * lerp;

		closest = closestPointOnLine( ul, bl, hit, true );
		lerp = ( closest - ul ).magnitude / ( bl - ul ).magnitude;
		var y = size.y * lerp;

		position = new Vector2( x, y );

		return true;

	}

	/// <summary>
	/// Performs a breadth-first search for a dfControl instance with the same
	/// name as the <paramref name="Name"/> argument <i>and</i> which is of the specified Type. 
	/// This search is case-sensitive.
	/// </summary>
	/// <typeparam name="T">The Type of control to find (must derive from <see cref="dfControl"/>)</typeparam>
	/// <param name="Name">The name of the dfControl you wish to find</param>
	/// <returns>TRUE if the dfControl was located, FALSE otherwise</returns>
	public T Find<T>( string Name ) where T : dfControl
	{

		if( this.name == Name && this is T )
			return (T)this;

		updateControlHeirarchy( true );

		for( int i = 0; i < controls.Count; i++ )
		{
			var test = controls[ i ] as T;
			if( test != null && test.name == Name )
				return test;
		}

		for( int i = 0; i < controls.Count; i++ )
		{
			var test = controls[ i ].Find<T>( Name );
			if( test != null )
				return test;
		}


		return (T)null;

	}

	/// <summary>
	/// Performs a breadth-first search for a dfControl instance with the same
	/// name as the <paramref name="Name"/> argument. This search is case-sensitive.
	/// </summary>
	/// <param name="Name">The name of the dfControl you wish to find</param>
	/// <returns>TRUE if the dfControl was located, FALSE otherwise</returns>
	public dfControl Find( string Name )
	{

		if( this.name == Name )
			return this;

		updateControlHeirarchy( true );

		for( int i = 0; i < controls.Count; i++ )
		{
			var test = controls[ i ];
			if( test.name == Name )
				return test;
		}

		for( int i = 0; i < controls.Count; i++ )
		{
			var test = controls[ i ].Find( Name );
			if( test != null )
				return test;
		}

		return null;

	}

	/// <summary>
	/// Sets the global input focus to this object
	/// </summary>
	public void Focus()
	{

		if( !CanFocus || HasFocus || !IsEnabled || !IsVisible )
			return;

		dfGUIManager.SetFocus( this );

		Invalidate();

	}

	/// <summary>
	/// Removes global input focus from this object
	/// </summary>
	public void Unfocus()
	{
		if( ContainsFocus )
		{
			dfGUIManager.SetFocus( null );
		}
	}

	/// <summary>
	/// Returns a reference to the top-most container for this control
	/// </summary>
	public dfControl GetRootContainer()
	{
		var loop = this;
		while( loop.Parent != null )
		{
			loop = loop.Parent;
		}
		return loop;
	}

	/// <summary>
	/// Brings this control to the front so that it appears in front of all 
	/// other controls within the same container
	/// </summary>
	public virtual void BringToFront()
	{
		if( parent == null )
		{
			GetManager().BringToFront( this );
		}
		else
		{
			parent.SetControlIndex( this, parent.controls.Count - 1 );
		}
		Invalidate();
	}

	/// <summary>
	/// Sends this control to the back so that it appears behind all 
	/// other controls within the same container
	/// </summary>
	public virtual void SendToBack()
	{
		if( parent == null )
		{
			GetManager().SendToBack( this );
		}
		else
		{
			parent.SetControlIndex( this, 0 );
		}
		Invalidate();
	}

	/// <summary>
	/// Renders this control to a <see cref="dfRenderData"/> buffer and
	/// returns the buffer
	/// </summary>
	/// <returns>A <see cref="dfRenderData"/> buffer containing the data 
	/// needed to render this <see cref="dfControl"/> instance as a Mesh</returns>
	private bool rendering = false;
	internal dfRenderData Render()
	{

		// Prevent recursion 
		if( rendering )
			return this.renderData;

		try
		{

			rendering = true;

			// NOTE: We can get away with checking the isVisible field instead
			// of the recursive IsVisible property because GUIManager.Render()
			// recursively iterates through all controls, and it's the first 
			// dfControl with the isVisible field set to FALSE that controls the
			// entire heirarchy below it.
			var controlIsVisible = this.isVisible || this.Opacity < 0.1f;
			var controlIsEnabled = this.enabled && gameObject.activeSelf;
			if( !controlIsVisible || !controlIsEnabled )
				return null;

			if( renderData == null )
			{
				renderData = dfRenderData.Obtain();
				isControlInvalidated = true;
			}

			if( isControlInvalidated )
			{

				// NOTE: This should come first so that controls may choose
				// whether to re-invalidate themselves during render, such 
				// as controls which animate some element etc.
				isControlInvalidated = false;

				renderData.Clear();

				OnRebuildRenderData();

				updateCollider();

			}

			// *ALWAYS* provide the current local-to-world transform!
			// This allows the RenderData to be re-used when the control
			// is translated, rotated, or scaled without having to completely
			// rebuild the buffer.
			renderData.Transform = this.transform.localToWorldMatrix;

			return renderData;

		}
		finally
		{
			rendering = false;
		}

	}

	/// <summary>
	/// Called when the control needs to rebuild its render information
	/// </summary>
	public virtual void Invalidate()
	{

		this.isControlInvalidated = true;

		updateVersion();

		// NOTE: Not using the cached [view] property here. Workaround for a Unity bug
		var myView = GetManager();
		if( myView != null )
		{
			myView.Invalidate();
		}

	}

	/// <summary>
	/// Causes the control to reset all layout information 
	/// </summary>
	/// <param name="recursive">Set to TRUE if the layout information should be
	/// reset recursively</param>
	[HideInInspector]
	public virtual void ResetLayout( bool recursive = false )
	{

		bool dontPerformLayout =
			IsPerformingLayout ||
			IsLayoutSuspended;

		if( dontPerformLayout )
			return;

		ensureLayoutExists();
		layout.Reset();

		if( recursive )
		{

			// Recursively reset the layout for all child controls
			for( int i = 0; i < Controls.Count; i++ )
			{
				controls[ i ].ResetLayout();
			}

		}

	}

	/// <summary>
	/// Causes this control to update its layout
	/// </summary>
	[HideInInspector]
	public virtual void PerformLayout()
	{

		if( isDisposing || performingLayout )
			return;

		try
		{
			
			performingLayout = true;

			ensureLayoutExists();

			layout.PerformLayout();
			Invalidate();

		}
		finally
		{
			performingLayout = false;
		}

	}

	/// <summary>
	/// Temporarily suspends the layout logic for the control
	/// </summary>
	[HideInInspector]
	public virtual void SuspendLayout()
	{

		ensureLayoutExists(); 
		layout.SuspendLayout();

		for( int i = 0; i < controls.Count; i++ )
		{
			controls[ i ].SuspendLayout();
		}

	}

	/// <summary>
	/// Resumes usual layout logic
	/// </summary>
	[HideInInspector]
	public virtual void ResumeLayout()
	{

		ensureLayoutExists(); 
		layout.ResumeLayout();

		for( int i = 0; i < controls.Count; i++ )
		{
			controls[ i ].ResumeLayout();
		}

	}

	/// <summary>
	/// Used during layout to determine the effective minimum size of the control,
	/// which may be different than that specified by the MinimumSize property. 
	/// </summary>
	/// <returns>A Vector2 value that represents the minimum control size</returns>
	public virtual Vector2 CalculateMinimumSize()
	{
		return this.MinimumSize;
	}

	/// <summary>
	/// Causes this control to textAlign its <see cref="Position"/> and <see cref="Size"/>
	/// properties so that they lie exactly on pixel boundaries
	/// </summary>
	[HideInInspector]
	public void MakePixelPerfect()
	{

		this.size = this.size.RoundToInt();

		var p2u = PixelsToUnits();
		transform.position = ( transform.position / p2u ).RoundToInt() * p2u;
		cachedPosition = transform.localPosition;

		for( int i = 0; i < controls.Count; i++ )
		{
			controls[ i ].MakePixelPerfect();
		}

		Invalidate();

	}

	/// <summary>
	/// Returns the axis-aligned bounding box enclosing this <see cref="dfControl"/>
	/// </summary>
	public Bounds GetBounds()
	{

		var corners = GetCorners();

		var center = corners[ 0 ] + ( corners[ 3 ] - corners[ 0 ] ) * 0.5f;
		var min = center;
		var max = center;

		for( int i = 0; i < corners.Length; i++ )
		{
			min = Vector3.Min( min, corners[ i ] );
			max = Vector3.Max( max, corners[ i ] );
		}

		return new Bounds( center, ( max - min ) );

	}

	/// <summary>
	/// Returns a <see cref="UnityEngine.Vector3"/> representing the global 
	/// coordinates of the dfControl's center
	/// </summary>
	public Vector3 GetCenter()
	{
		return transform.position + Pivot.TransformToCenter( Size ) * PixelsToUnits();
	}

	/// <summary>
	/// Returns an array of Vector3 values corresponding to the global
	/// positions of this object's bounding box. The corners are specified
	/// in the following order: Top Left, Top Right, Bottom Left, Bottom Right
	/// </summary>
	public Vector3[] GetCorners()
	{

		var p2u = PixelsToUnits();
		var matrix = transform.localToWorldMatrix;

		var upperLeft = pivot.TransformToUpperLeft( size );
		var upperRight = upperLeft + new Vector3( size.x, 0 );
		var bottomLeft = upperLeft + new Vector3( 0, -size.y );
		var bottomRight = upperRight + new Vector3( 0, -size.y );

		cachedCorners[ 0 ] = matrix.MultiplyPoint( upperLeft * p2u );
		cachedCorners[ 1 ] = matrix.MultiplyPoint( upperRight * p2u );
		cachedCorners[ 2 ] = matrix.MultiplyPoint( bottomLeft * p2u );
		cachedCorners[ 3 ] = matrix.MultiplyPoint( bottomRight * p2u );

		return cachedCorners;

	}

	/// <summary>
	/// Returns a reference to the <see cref="UnityEngine.Camera"/> that is
	/// responsible for rendering this <see cref="dfControl"/>
	/// </summary>
	public Camera GetCamera()
	{

		var view = GetManager();
		if( view == null )
		{
			Debug.LogError( "The Manager hosting this control could not be determined" );
			return null;
		}

		return view.RenderCamera;

	}

	/// <summary>
	/// Returns the Screen-based coordinates containing this control
	/// </summary>
	public Rect GetScreenRect()
	{

		// TODO: Investigate non-"Pixel perfect" behavior - http://www.daikonforge.com/dfgui/forums/topic/getscreenrect-texture-sprite-and-pixelperfect-off/

		var camera = GetCamera();
		var corners = GetCorners();

		var screenPos = camera.WorldToScreenPoint( corners[ 0 ] );

		return new Rect(
			screenPos.x, 
			camera.pixelHeight - screenPos.y,
			Size.x,
			Size.y
		);

	}

	/// <summary>
	/// Returns a reference to the <see cref="dfGUIManager"/> instance that 
	/// is responsible for rendering the control
	/// </summary>
	public dfGUIManager GetManager()
	{

		// If the view is already cached or there is no way of obtaining
		// a reference to the view, return the cached value
		if( manager != null || !gameObject.activeInHierarchy )
			return manager;

		// If this dfControl's parent has already done the work of looking
		// for the Manager, then use that glyphData instead
		if( parent != null && parent.manager != null )
			return manager = parent.manager;

		// Walk up the scene hierarchy looking for the root Manager
		var loop = this.gameObject;
		while( loop != null )
		{

			var test = loop.GetComponent<dfGUIManager>();
			if( test != null )
				return manager = test;

			if( loop.transform.parent == null )
				break;

			loop = loop.transform.parent.gameObject;

		}

		//
		// If the Transform hierarchy didn't work, then attempt to find any 
		// Manager in the scene. If one can be found return it but do not 
		// cache the value on the assumption that later calls to this 
		// function will eventually be able to find the Manager correctly.
		//
		// It is assumed that this is *fairly* safe to do because in most 
		// cases there will only ever be one Manager in the scene at any given 
		// time. It is certainly possible that this will not always be the
		// case, but as this bug is an infrequent edge case to begin with 
		// and there is no better solution apparent at this time... Meh.
		//
		var findView = FindObjectsOfType( typeof( dfGUIManager ) ).FirstOrDefault() as dfGUIManager;

		if( findView != null )
		{
			return findView;
		}

		return null;

	}

	/// <summary>
	/// Returns a number representing the conversion of World Units to pixels,
	/// used to convert a dfControl's "pixel-based" position and location properties
	/// into world units for rendering and raycasting purposes.
	/// </summary>
	protected internal float PixelsToUnits()
	{

		if( cachedPixelSize > float.Epsilon )
			return cachedPixelSize;

		var view = GetManager();
		if( view == null )
		{

			// When no camera is available to get pixel size information from,
			// this default will return a reasonable approximation of a screen
			// whose height is 768 pixels
			const float DEFAULT_PIXEL_SIZE = 0.0026f;

			return DEFAULT_PIXEL_SIZE;

		}

		return cachedPixelSize = view.PixelsToUnits();

	}

	/// <summary>
	/// Returns the set of clipping planes used to clip child controls.
	/// Planes are specified in the following order: Left, Right, Top, Bottom
	/// </summary>
	/// <returns>Returns an array of <see cref="Plane"/> that enclose the object in world coordinates</returns>
	protected internal virtual Plane[] GetClippingPlanes()
	{

		var corners = GetCorners();

		var right = transform.TransformDirection( Vector3.right );
		var left = transform.TransformDirection( Vector3.left );
		var up = transform.TransformDirection( Vector3.up );
		var down = transform.TransformDirection( Vector3.down );

		cachedClippingPlanes[ 0 ] = new Plane( right, corners[ 0 ] );
		cachedClippingPlanes[ 1 ] = new Plane( left, corners[ 1 ] );
		cachedClippingPlanes[ 2 ] = new Plane( up, corners[ 2 ] );
		cachedClippingPlanes[ 3 ] = new Plane( down, corners[ 0 ] );

		return cachedClippingPlanes;

	}

	/// <summary>
	/// Retrieves a value indicating whether the specified control is a child of this control.
	/// </summary>
	/// <param name="child">The <see cref="dfControl"/> to evaluate</param>
	/// <returns>TRUE if the specified control is a child of the control and FALSE otherwise</returns>
	public bool Contains( dfControl child )
	{
		return ( child != null ) && child.transform.IsChildOf( this.transform );
	}

	#endregion

	#region Protected methods 

	[HideInInspector]
	protected internal virtual void updateCollider()
	{

		var myCollider = collider as BoxCollider;
		if( myCollider == null )
		{
			myCollider = gameObject.AddComponent<BoxCollider>();
		}

		var p2u = PixelsToUnits();
		var sizeInUnits = size * p2u;
		var center = pivot.TransformToCenter( sizeInUnits );

		myCollider.size = new Vector3( sizeInUnits.x, sizeInUnits.y, 0.001f );
		myCollider.center = center;

		if( Application.isPlaying && !this.IsInteractive )
			myCollider.enabled = false;
		else
			myCollider.enabled = this.enabled && this.IsVisible;

	}

	/// <summary>
	/// Called by the <see cref="dfControl"/> class during rendering to 
	/// allow any derived classes to rebuild the <see cref="renderData"/>
	/// buffer.
	/// </summary>
	[HideInInspector]
	protected virtual void OnRebuildRenderData()
	{
		// This is just a placeholder function - It is intended to be overridden
	}

	/// <summary>
	/// Raises the ControlAdded event
	/// </summary>
	[HideInInspector]
	protected internal virtual void OnControlAdded( dfControl child )
	{

		Invalidate();

		if( ControlAdded != null )
		{
			ControlAdded( this, child );
		}

	}

	/// <summary>
	/// Raises the ControlRemoved event
	/// </summary>
	[HideInInspector]
	protected internal virtual void OnControlRemoved( dfControl child )
	{

		Invalidate();

		if( ControlRemoved != null )
		{
			ControlRemoved( this, child );
		}

	}

	/// <summary>
	/// Raises the PositionChanged event
	/// </summary>
	[HideInInspector]
	protected internal virtual void OnPositionChanged()
	{

		transform.hasChanged = false;

		if( renderData != null )
		{

			// The dfControl's cached RenderData is still usable, 
			// just need to re-render with a new transpose Matrix 
			// (which will be set in dfControl.Render)
			updateVersion();
			GetManager().Invalidate();

		}
		else
		{
			Invalidate();
		}

		ResetLayout();

		if( PositionChanged != null )
		{
			PositionChanged( this, Position );
		}

	}

	/// <summary>
	/// Raises the SizeChanged event
	/// </summary>
	[HideInInspector]
	protected internal virtual void OnSizeChanged()
	{

		updateCollider();
		Invalidate();

		// If there is no layout being performed (check in ResetLayout), it 
		// means that the Size was set independantly of layout either in code 
		// or by the developer, so reset the layout.
		ResetLayout();

		if( SizeChanged != null )
		{
			SizeChanged( this, Size );
		}

		for( int i = 0; i < controls.Count; i++ )
		{
			controls[ i ].PerformLayout();
		}

	}

	/// <summary>
	/// Raises the PivotChanged event
	/// </summary>
	[HideInInspector]
	protected internal virtual void OnPivotChanged()
	{

		Invalidate();

		if( PivotChanged != null )
		{
			PivotChanged( this, pivot );
		}

	}

	/// <summary>
	/// Raises the AnchorChanged event
	/// </summary>
	[HideInInspector]
	protected internal virtual void OnAnchorChanged()
	{

		var anchor = this.layout.AnchorStyle;

		Invalidate();
		ResetLayout();

		if( anchor.IsAnyFlagSet( dfAnchorStyle.CenterHorizontal | dfAnchorStyle.CenterVertical ) )
		{
			PerformLayout();
		}

		if( AnchorChanged != null )
		{
			AnchorChanged( this, anchor );
		}

	}

	[HideInInspector]
	protected internal virtual void OnOpacityChanged()
	{

		Invalidate();

		if( OpacityChanged != null )
		{
			OpacityChanged( this, Opacity );
		}

		for( int i = 0; i < controls.Count; i++ )
		{
			controls[ i ].OnOpacityChanged();
		}

	}

	[HideInInspector]
	protected internal virtual void OnColorChanged()
	{

		Invalidate();

		if( ColorChanged != null )
		{
			ColorChanged( this, Color );
		}

		for( int i = 0; i < controls.Count; i++ )
		{
			controls[ i ].OnColorChanged();
		}

	}

	[HideInInspector]
	protected internal virtual void OnZOrderChanged()
	{

		Invalidate();

		if( ZOrderChanged != null )
		{
			ZOrderChanged( this, this.zindex );
		}

	}

	[HideInInspector]
	protected internal virtual void OnTabIndexChanged()
	{

		Invalidate();

		if( TabIndexChanged != null )
		{
			TabIndexChanged( this, this.tabIndex );
		}

	}

	[HideInInspector]
	protected internal virtual void OnIsVisibleChanged()
	{

		if( HasFocus && !IsVisible )
		{
			dfGUIManager.SetFocus( null );
		}

		Invalidate();

		Signal( "OnIsVisibleChanged", this, IsVisible );

		if( IsVisibleChanged != null )
		{
			IsVisibleChanged( this, isVisible );
		}

		for( int i = 0; i < controls.Count; i++ )
		{
			controls[ i ].OnIsVisibleChanged();
		}

	}

	[HideInInspector]
	protected internal virtual void OnIsEnabledChanged()
	{

		if( dfGUIManager.ContainsFocus( this ) && !IsEnabled )
		{
			dfGUIManager.SetFocus( null );
		}

		Invalidate();

		Signal( "OnIsEnabledChanged", this, IsEnabled );

		if( IsEnabledChanged != null )
		{
			IsEnabledChanged( this, isEnabled );
		}

		for( int i = 0; i < controls.Count; i++ )
		{
			controls[ i ].OnIsEnabledChanged();
		}

	}

	/// <summary>
	/// Calculates the final opacity of this control, taking into account
	/// the opacity of all controls higher in the control hierarchy
	/// </summary>
	/// <returns>The final opacity that should be used to render this control</returns>
	protected internal float CalculateOpacity()
	{
		if( parent == null ) return this.Opacity;
		return this.Opacity * parent.CalculateOpacity();
	}

	/// <summary>
	/// Applies the results of <see cref="CalculateOpacity"/> to the given 
	/// <see cref="Color32"/> value. This is a convenience function used 
	/// to determine vector colors when rendering the control.
	/// </summary>
	protected internal Color32 ApplyOpacity( Color32 color )
	{
		float opacity = CalculateOpacity();
		color.a = (byte)(opacity * 255);
		return color;
	}

	/// <summary>
	/// Returns the relative position in screen coordinates of the mouse cursor within
	/// the bounds of the control. Returns (Vector2.one * float.MinValue) if the mouse
	/// cursor does not fall within the bounds of the control.
	/// </summary>
	protected internal Vector2 GetHitPosition( dfMouseEventArgs args )
	{

		Vector2 result;
		GetHitPosition( args.Ray, out result );

		return result;

	}

	/// <summary>
	/// Transforms a direction vector and scales it to the same scale
	/// as the Manager which contains this control
	/// </summary>
	/// <param name="direction"></param>
	/// <returns></returns>
	protected internal Vector3 getScaledDirection( Vector3 direction )
	{

		var scale = GetManager().transform.localScale;

		direction = transform.TransformDirection( direction );

		return Vector3.Scale( direction, scale );

	}

	/// <summary>
	/// Transforms and scales a vector such that it will always
	/// represent the same fixed distance on-screen even when the 
	/// user interface is scaled
	/// </summary>
	/// <param name="offset"></param>
	/// <returns></returns>
	protected internal Vector3 transformOffset( Vector3 offset )
	{

		var x = offset.x * getScaledDirection( Vector3.right );
		var y = offset.y * getScaledDirection( Vector3.down );

		return ( x + y ) * PixelsToUnits();

	}

	/// <summary>
	/// This function is called by the GUI system when the screen resolution has changed.
	/// </summary>
	/// <param name="previousResolution">The previous screen resolution</param>
	/// <param name="currentResolution">The new screen resolution</param>
	protected internal virtual void OnResolutionChanged( Vector2 previousResolution, Vector2 currentResolution )
	{

		// Make sure that the control gets rendered on the next frame
		Invalidate();

		// OnResolutionChanged() happens very early in the startup process, and the 
		// execution order is indeterminate, so make sure that the control hierarchy 
		// is properly set up when this function is called.
		updateControlHeirarchy();

		// Ensure that the control's layout is aware of its owner
		layout.Attach( this );

		// Make sure that the control remains at the same relative position
		cachedPixelSize = 0f;
		var oldPos = transform.localPosition / ( 2f / previousResolution.y.RoundToNearest( 2 ) );
		cachedPosition = transform.localPosition = oldPos * ( 2f / currentResolution.y.RoundToNearest( 2 ) );

		// Make sure that the control's collider reflects the new position
		updateCollider();

		// Notify any third-party scripts that this control has changed due to 
		// the change in screen resolution
		Signal( "OnResolutionChanged", this, previousResolution, currentResolution );

	}

	#endregion

	#region Unity events 

#if UNITY_EDITOR

	[HideInInspector]
	public virtual void OnDrawGizmos()
	{

		var collider = this.collider as BoxCollider;
		collider.hideFlags = HideFlags.HideInInspector;

		if( !IsVisible || Opacity < 0.1f )
			return;

		var center = pivot.TransformToCenter( Size ) * PixelsToUnits();
		var size = ( (Vector3)this.Size ) * this.PixelsToUnits();

		Gizmos.matrix = Matrix4x4.TRS( transform.position, transform.rotation, transform.localScale );

		Gizmos.color = new UnityColor( 0, 0, 0, 0.175f );
		Gizmos.DrawWireCube( center, size );

		// Rendering a clear cube allows the user to click on the control
		// in the Unity Editor Scene Manager
		var thickness = new Vector3( 0, 0, 0.003f * ( renderOrder + 1 ) );
		Gizmos.color = UnityColor.clear;
		Gizmos.DrawCube( center, size + thickness );

	}

	[HideInInspector]
	public virtual void OnDrawGizmosSelected()
	{

		if( UnityEditor.Selection.activeObject != gameObject )
		{
			return;
		}

		if( !IsVisible )
			return;

		var center = pivot.TransformToCenter( Size ) * PixelsToUnits();
		var size = ( (Vector3)this.Size ) * this.PixelsToUnits();

		Gizmos.matrix = Matrix4x4.TRS( transform.position, transform.rotation, transform.localScale );

		Gizmos.color = new UnityColor( 0, 1, 0, 0.25f );
		Gizmos.DrawWireCube( center, size );

		// Rendering a clear cube allows the user to click on the control
		// in the Unity Editor Scene Manager
		var thickness = new Vector3( 0, 0, 0.003f * ( renderOrder + 1 ) );
		Gizmos.color = UnityColor.clear;
		Gizmos.DrawCube( center, size + thickness );

	}

#endif

	/// <summary>
	/// Awake is called by the Unity engine when the script instance is being loaded.
	/// </summary>
	[HideInInspector]
	public virtual void Awake()
	{
		if( layout != null ) layout.Attach( this );
	}

	/// <summary>
	/// Start is called by the Unity engine before any of the <see cref="Update"/> 
	/// methods is called for the first time
	/// </summary>
	[HideInInspector]
	public virtual void Start()
	{

		if( this.controls == null )
		{
			updateControlHeirarchy();
		}

		if( Application.isPlaying )
		{
			collider.enabled = this.IsInteractive;
		}

	}

	/// <summary>
	/// This function is called by the Unity engine when the object becomes enabled and current.
	/// </summary>
	[HideInInspector]
	public virtual void OnEnable()
	{

		initializeControl();

		if( controls == null || controls.Count == 0 )
		{
			updateControlHeirarchy();
		}

		// Prefabs should not attempt to reuse invalid layout information
		if( !gameObject.activeInHierarchy )
		{
			ResetLayout();
		}

#if UNITY_EDITOR
		// HACK: Without this, the layouts in the editor window are messed up
		// until the user clicks within the viewport
		if( Application.isEditor && !Application.isPlaying )
		{
			Update();
		}
#endif

		OnIsEnabledChanged();

	}

	/// <summary>
	/// Sent to all game objects by the Unity engine before the application is quit.
	/// </summary>
	[HideInInspector]
	public virtual void OnApplicationQuit()
	{
		RemoveAllEventHandlers();
	}

	/// <summary>
	/// This function is called by the Unity engine when the cotnrol becomes 
	/// disabled or inactive.
	/// </summary>
	[HideInInspector]
	public virtual void OnDisable()
	{

		// NOTE: Try..Catch was added to work around an uncommon and as far as I could
		// discover a non-deterministic Unity issue in web builds. 
		try
		{

			Invalidate();

			if( this.renderData != null )
			{
				this.renderData.Release();
				this.renderData = null;
			}

			// NOTE: Not using the cached [view] property here. Workaround for a Unity bug
			if( dfGUIManager.HasFocus( this ) )
			{
				dfGUIManager.SetFocus( null );
			}

			OnIsEnabledChanged();

		}
		catch { }
			
	}

	/// <summary>
	/// This function is called by the Unity engine when the control will be destroyed.
	/// </summary>
	[HideInInspector]
	public virtual void OnDestroy()
	{

		isDisposing = true;

		// Detach all event handlers
		if( Application.isPlaying )
		{
			RemoveAllEventHandlers();
		}

		// Make sure that the layout component does not hold on to references
		if( layout != null )
		{
			layout.Dispose();
		}

		// Make sure that the parent control no longer holds a reference to this control
		if( parent != null && parent.controls != null && !parent.isDisposing )
		{
			if( parent.controls.Remove( this ) )
			{
				parent.cachedChildCount -= 1;
				parent.OnControlRemoved( this );
			}
		}

		// Make sure that child controls no longer hold a reference to this control
		for( int i = 0; i < controls.Count; i++ )
		{

			if( controls[ i ].layout != null )
			{
				controls[ i ].layout.Dispose();
				controls[ i ].layout = null;
			}

			controls[ i ].parent = null;

		}

		// Make sure that this control no longer holds references to any child controls
		controls.Release();

		// Let the GUIManager instance responsible for rendering this control
		// know that it needs to refresh
		if( manager != null )
		{
			manager.Invalidate();
		}

		// Make sure the control does not hold on to cached render data
		if( this.renderData != null )
		{
			this.renderData.Release();
		}

		// Clear references
		layout = null;
		manager = null;
		parent = null;
		cachedClippingPlanes = null;
		cachedCorners = null;
		renderData = null;
		controls = null;

	}

	/// <summary>
	/// Called by the Unity engine every frame (after <see cref="Update"/>) if
	/// the control component is enabled
	/// </summary>
	[HideInInspector]
	public virtual void LateUpdate()
	{

		// Make sure that pending layout requests get processed. There might be
		// pending layout requests if a control's PerformLayout() method was 
		// called while layout was suspended using SuspendLayout/ResumeLayout
		if( layout != null && layout.HasPendingLayoutRequest )
		{
			layout.PerformLayout();
		}

	}

	/// <summary>
	/// Called by the Unity engine every frame if the control component is enabled
	/// </summary>
	[HideInInspector]
	public virtual void Update()
	{

		// Cache the .transform property to avoid repeated calls to the
		// property get method
		var transform = this.transform;

		// Maintain the control hierarchy - Keeps track of when child Controls are 
		// added to or removed from this dfControl.
		updateControlHeirarchy();

		// If the transform has been changed this dfControl needs to determine whether
		// the changes are already reflected in the Position and Size properties and
		// act accordingly if they are not
		if( transform.hasChanged )
		{

			//! Lock the object's scale. This library does not use Scale to size
			//! controls, and always assumes unit scale
			if( !Application.isPlaying )
			{
#if UNITY_EDITOR
				if( Vector3.Distance( this.transform.localScale, Vector3.one ) > float.Epsilon )
				{
					this.transform.localScale = Vector3.one;
				}
#endif
			}
			else
			{
				// If the control's scale has changed, need to rebuild render data
				if( cachedScale != transform.localScale )
				{
					cachedScale = transform.localScale;
					Invalidate();
				}
			}

			// If the developer has moved the control using the on-screen translate
			// widget in the editor or the control has been moved via script without
			// using the built-in Position/RelativePosition methods, keep track of
			// the new position and raise the PositionChanged event
			if( ( cachedPosition - transform.localPosition ).sqrMagnitude > float.Epsilon )
			{

				// Keep track of "last known" position so that the OnPositionChanged event
				// can be fired when the user moves the control through either the Position
				// property in the Inspector or via gizmos in the scene.
				cachedPosition = transform.localPosition;

				// Notify any observers that the control has been moved
				OnPositionChanged();

			}

			// If the control was rotated, notify any observers
			if( cachedRotation != transform.localRotation )
			{
				cachedRotation = transform.localRotation;
				Invalidate();
			}

			// Clear the changed flag
			transform.hasChanged = false;

		}

	}

	#endregion

	#region Manage control hierarchy 

	protected internal void SetControlIndex( dfControl child, int zindex )
	{

		var swap = controls.FirstOrDefault( c => c.zindex == zindex && c != child );
		if( swap != null )
		{
			swap.zindex = controls.IndexOf( child );
		}

		child.zindex = zindex;

		rebuildControlOrder();

	}

	/// <summary>
	/// Creates a new <see cref="dfControl"/> instance of the specified
	/// type and adds it as a child of this instance
	/// </summary>
	/// <typeparam name="T">The type of control to be created</typeparam>
	/// <returns>A reference to the newly-created control instance</returns>
	public T AddControl<T>() where T : dfControl
	{

		var go = new GameObject( typeof( T ).Name );
		go.transform.parent = this.transform;
		go.layer = this.gameObject.layer;

		go.transform.localPosition = (Vector3)( Size * PixelsToUnits() * 0.5f );

		var child = go.AddComponent<T>();
		child.parent = this;
		child.ZOrder = -1;

		AddControl( child );

		return child;

	}

	/// <summary>
	/// Creates a new <see cref="dfControl"/> instance of the specified
	/// type and adds it as a child of this instance
	/// </summary>
	/// <param name="ControlType">The type of control to be created</param>
	/// <returns>A reference to the newly-created control instance</returns>
	public dfControl AddControl( Type ControlType )
	{

		if( !typeof( dfControl ).IsAssignableFrom( ControlType ) )
		{
			throw new InvalidCastException();
		}

		var go = new GameObject( ControlType.Name );
		go.transform.parent = this.transform;
		go.layer = this.gameObject.layer;

		var position = Size * PixelsToUnits() * 0.5f;
		go.transform.localPosition = new Vector3( position.x, position.y, 0 );

		var child = go.AddComponent( ControlType ) as dfControl;
		child.parent = this;
		child.ZOrder = -1;

		AddControl( child );

		return child;

	}

	/// <summary>
	/// Adds the child control to the list of child controls for this instance
	/// </summary>
	/// <param name="child">The <see cref="dfControl"/> instance to add to the list of child controls</param>
	protected internal void AddControl( dfControl child )
	{

		// Cannot just call AddControl( new dfControl() ), use AddControl<Type>() instead.
		if( child.transform == null )
		{
			throw new NullReferenceException( "The child control does not have a Transform" );
		}

		// Nothing to do if the control is already in the collection
		if( controls.Contains( child ) )
			return;

		// Add the control to the hierarchy
		controls.Add( child );
		child.parent = this;
		child.transform.parent = this.transform;
			
		// A ZOrder value of -1 means "make it the topmost control"
		if( child.ZOrder == -1 )
		{
			child.ZOrder = getMaxZOrder() + 1;
		}

		// Notify all observers that a control has been added
		OnControlAdded( child );

	}

	/// <summary>
	/// Returns the highest value of the ZOrder property of this control's child controls
	/// </summary>
	private int getMaxZOrder()
	{

		var max = 0;
		for( int i = 0; i < controls.Count; i++ )
		{
			max = Mathf.Max( controls[ i ].ZOrder, max );
		}

		return max;

	}

	/// <summary>
	/// Removes the indicated control from the list of child controls for this instance
	/// </summary>
	/// <param name="child">The control to remove from the list of child controls</param>
	protected internal void RemoveControl( dfControl child )
	{

		if( isDisposing )
			return;

		if( controls.Remove( child ) )
		{
			rebuildControlOrder();
			OnControlRemoved( child );
		}

	}

	/// <summary>
	/// Compensates for Unity3D's lack of a consistent order of startup events
	/// and lack of hierarchy change notifications by manually tracking changes 
	/// to the GameObject's hierarchy and updating the Controls collection to match.
	/// </summary>
	private void updateControlHeirarchy( bool force = false )
	{

		// Assume that the control hierarchy is correct if Transform.childCount
		// matches the number of controls in our collection
		var transformChildCount = transform.childCount;
		if( !force && transformChildCount == cachedChildCount )
		{
			return;
		}

		// Keep track of the number of nodes attached to the dfControl's Transform
		cachedChildCount = transformChildCount;

		var childControls = getChildControls();

		for( int i = 0; i < childControls.Count; i++ )
		{

			var child = childControls[ i ];
			if( controls.Contains( child ) )
				continue;

			// Add the dfControl to the hierarchy
			child.parent = this;

			// Controls need a new layout when parented to another dfControl
			//child.ResetLayout();

			// Notify any observers that the control was added
			OnControlAdded( child );
			child.updateControlHeirarchy();

		}

		// Determine whether any controls have been deleted without performing 
		// cleanup (such as removing itself from the parent's Controls list). 
		// This can happen when the child control is deleted in the Unity Editor,
		// which does not seem to fire the OnDestroy function while in Edit Mode
		for( int i = 0; i < controls.Count; i++ )
		{

			var child = controls[ i ];
			if( child == null || !childControls.Contains( child ) )
			{

				// Notify any observers that this dfControl's hierarchy has
				// changed. Observers will need to check for NULL, but 
				// unfortunately there's no way to know which control was 
				// removed at this point.
				OnControlRemoved( child );

				// If the control was reparented in the Editor Scene Hierarchy
				// it will probably still have an invalid Parent reference to 
				// this control. 
				if( child != null && child.parent == this )
				{
					child.parent = null;
				}

			}

		}

		// Start using the new list
		this.controls.Release();
		this.controls = childControls;

		// Always keep the Controls sorted by ZOrder
		rebuildControlOrder();

	}

	/// <summary>
	/// Iterates the Transform's children looking for dfControl components
	/// </summary>
	/// <returns></returns>
	private dfList<dfControl> getChildControls()
	{

		var childCount = transform.childCount;

		var controls = dfList<dfControl>.Obtain();
		controls.EnsureCapacity( childCount );

		for( int i = 0; i < childCount; i++ )
		{
				
			var childTransform = transform.GetChild( i );
			if( !childTransform.gameObject.activeSelf )
				continue;

			var control = childTransform.GetComponent<dfControl>();
			if( control != null )
			{
				controls.Add( control );
			}

		}

		return controls;

	}

	private void rebuildControlOrder()
	{

		var rebuild = false;

		controls.Sort();
		for( int i = 0; i < controls.Count; i++ )
		{
			if( controls[ i ].ZOrder != i )
			{
				rebuild = true;
				break;
			}
		}

		if( !rebuild )
			return;

		controls.Sort();
		for( int i = 0; i < controls.Count; i++ )
		{
			controls[ i ].zindex = i;
		}

	}

	private void ensureLayoutExists()
	{

		// If a valid layout already exists, nothing to do
		if( layout == null )
		{
			layout = new AnchorLayout( dfAnchorStyle.Left | dfAnchorStyle.Top, this );
		}

		// Make sure all child controls also have a layout
		for( int i = 0; i < Controls.Count; i++ )
		{
			controls[ i ].ensureLayoutExists();
		}

	}

	#endregion

	#region Private utility methods

	protected internal void updateVersion()
	{
		this.version = ++versionCounter;
	}

	private void setPositionInternal( Vector3 value )
	{

		value += pivot.UpperLeftToTransform( Size );
		value *= PixelsToUnits();

		if( ( value - cachedPosition ).sqrMagnitude <= float.Epsilon )
			return;

		cachedPosition = transform.localPosition = value;

		OnPositionChanged();

	}

	private void initializeControl()
	{

		ensureLayoutExists();

		if( renderOrder == -1 )
		{
			renderOrder = ZOrder;
		}

		if( transform.parent != null )
		{

			var parentControl = transform.parent.GetComponent<dfControl>();
			if( parentControl != null )
			{
				parentControl.AddControl( this );
			}

		}

		updateControlHeirarchy( true );

		Invalidate();

		// TODO: Review why control colliders are set to trigger: See http://www.daikonforge.com/dfgui/forums/topic/no-mouse-input/
		collider.isTrigger = true;

	}

	private Vector3 getRelativePosition()
	{

		if( transform.parent == null )
		{
			//Debug.LogError( "Cannot compute relative position without a parent Transform for control " + this );
			return Vector3.zero;
		}

		if( parent != null )
		{

			var p2u = PixelsToUnits();
			var parentPos = transform.parent.position;
			var controlPos = transform.position;

			var relativeTransform = transform.parent;

			var relativeParentPos = relativeTransform.InverseTransformPoint( parentPos / p2u );
			relativeParentPos += parent.pivot.TransformToUpperLeft( parent.size );

			var relativeControlPos = relativeTransform.InverseTransformPoint( controlPos / p2u );
			relativeControlPos += this.pivot.TransformToUpperLeft( this.size );

			var relativePos = relativeControlPos - relativeParentPos;

			return relativePos.Scale( 1, -1, 1 );

		}

		// Need a reference to the parent Manager in order to proceed
		var view = GetManager();
		if( view == null )
		{
			Debug.LogError( "Cannot get position: View not found" );
			return Vector3.zero;
		}

		// Calculate the upper left corner in world-space coordinates
		var pixelSize = PixelsToUnits();
		var upperLeft = transform.position + pivot.TransformToUpperLeft( size ) * pixelSize;

		// The Manager's clipping planes are ordered left, right, top, bottom
		var planes = view.GetClippingPlanes();
		var x = planes[ 0 ].GetDistanceToPoint( upperLeft ) / pixelSize;
		var y = planes[ 3 ].GetDistanceToPoint( upperLeft ) / pixelSize;

		// Return the relative distance from the left and top frustum planes
		var relative = new Vector3( x, y ).RoundToInt();

		return relative;

	}

	private void setRelativePosition( Vector3 value )
	{


		if( transform.parent == null )
		{
			Debug.LogError( "Cannot set relative position without a parent Transform." );
			return;
		}

		if( ( value - getRelativePosition() ).sqrMagnitude <= float.Epsilon )
			return;

		if( parent != null )
		{

			var newPos =
				value.Scale( 1, -1, 1 )
				+ pivot.UpperLeftToTransform( this.size )
				- parent.pivot.UpperLeftToTransform( parent.size )
				;

			newPos = newPos * PixelsToUnits();

			if( ( newPos - transform.localPosition ).sqrMagnitude >= float.Epsilon )
			{

				transform.localPosition = newPos;
				cachedPosition = newPos;

				OnPositionChanged();

			}

			return;

		}

		// Need a reference to the parent Manager in order to proceed
		var view = GetManager();
		if( view == null )
		{
			Debug.LogError( "Cannot get position: View not found" );
			return;
		}

		// The Manager's corners are ordered TopLeft, TopRight, BottomRight, BottomLeft
		var corners = view.GetCorners();
		var screenUpperLeft = corners[ 0 ];

		// Convert value to world coordinates
		var pixelSize = PixelsToUnits(); 
		value = value.Scale( 1, -1, 1 ) * pixelSize;

		// Calculate offset needed to translate from upper left to transform
		var offset = pivot.UpperLeftToTransform( Size ) * pixelSize;

		// Calculate new position 
		var newPosition =
			screenUpperLeft +
			view.transform.TransformDirection( value ) +
			offset;

		// No need to fire the OnPositionChanged event if the position has not
		// actually changed significantly
		if( ( newPosition - cachedPosition ).sqrMagnitude > float.Epsilon )
		{

			// Assign the transform position and keep a cached copy
			transform.position = newPosition;
			cachedPosition = transform.localPosition;

			// Notify any observers that the control's position has changed
			OnPositionChanged();

		}

	}

	private static float distanceFromLine( Vector3 start, Vector3 end, Vector3 test )
	{

		Vector3 v = start - end;
		Vector3 w = test - end;

		float c1 = Vector3.Dot( w, v );
		if( c1 <= 0 )
			return Vector3.Distance( test, end );

		float c2 = Vector3.Dot( v, v );
		if( c2 <= c1 )
			return Vector3.Distance( test, start );

		float b = c1 / c2;
		Vector3 Pb = end + b * v;

		return Vector3.Distance( test, Pb );

	}

	private static Vector3 closestPointOnLine( Vector3 start, Vector3 end, Vector3 test, bool clamp )
	{

		// http://www.gamedev.net/community/forums/topic.asp?topic_id=198199&whichpage=1&#1250842

		Vector3 c = test - start;				// Vector from a to Point
		Vector3 v = ( end - start ).normalized;	// Unit Vector from a to b
		float d = ( end - start ).magnitude;	// Length of the line segment
		float t = Vector3.Dot( v, c );			// Intersection point Distance from a

		// Check to see if the point is on the line
		// if not then return the endpoint
		if( clamp )
		{
			if( t < 0 ) return start;
			if( t > d ) return end;
		}

		// get the distance to move from point a
		v *= t;

		// move from point a to the nearest point on the segment
		return start + v;

	}

	#endregion

	#region IComparable<dfControl> Members

	/// <summary>
	/// Used to compare <see cref="dfControl"/> instances in order to 
	/// sort them according to <see cref="ZOrder"/> value.
	/// </summary>
	/// <param name="other">The other <see cref="dfControl"/> instance to compare against</param>
	/// <returns>
	/// A signed number indicating the relative values of this instance and value: 
	/// Less than zero if this instance has a lower ZOrder value than <paramref name="other"/>, 
	/// greater than zero if this instance has a higher ZOrder value than <paramref name="other"/>,
	/// and zero if both instances have the same ZOrder value
	/// </returns>
	public int CompareTo( dfControl other )
	{

		// Sort dfControl instances with a negative ZOrder *after* controls
		// with already-assigned ZOrder values
		if( this.ZOrder < 0 )
		{
			if( other.ZOrder < 0 )
				return 0;
			else
				return 1;
		}

		return ZOrder.CompareTo( other.ZOrder );

	}

	#endregion

	#region Private nested classes 

	/// <summary>
	/// Implements basic Anchor Layout - Allows the control to "anchor" each edge
	/// to the corresponding edge of its container such that the control resizes 
	/// properly when its container is resized.
	/// @internal
	/// </summary>
	[Serializable]
	protected class AnchorLayout
	{

		#region Protected serialized fields 

		[SerializeField]
		protected dfAnchorStyle anchorStyle;

		[SerializeField]
		protected dfAnchorMargins margins;

		[SerializeField]
		protected dfControl owner;

		#endregion

		#region Private fields 

		private int suspendLayoutCounter = 0;
		private bool performingLayout = false;
		private bool disposed = false;
		private bool pendingLayoutRequest = false;

		#endregion

		#region Constructor

		internal AnchorLayout( dfAnchorStyle anchorStyle )
		{
			this.anchorStyle = anchorStyle;
		}

		internal AnchorLayout( dfAnchorStyle anchorStyle, dfControl owner )
			: this( anchorStyle )
		{
			Attach( owner );
			Reset();
		}

		#endregion

		#region Public properties 

		internal dfAnchorStyle AnchorStyle
		{
			get { return this.anchorStyle; }
			set
			{
				if( value != this.anchorStyle )
				{
					this.anchorStyle = value;
					Reset();
				}
			}
		}

		internal bool IsPerformingLayout 
		{ 
			get { return performingLayout; } 
		}

		internal bool IsLayoutSuspended 
		{ 
			get { return suspendLayoutCounter > 0; } 
		}

		internal bool HasPendingLayoutRequest 
		{ 
			get { return pendingLayoutRequest; } 
		}

		#endregion

		#region Public methods

		internal void Dispose()
		{

			if( !disposed )
			{
				disposed = true;
				owner = null;
			}

		}

		internal void SuspendLayout()
		{
			suspendLayoutCounter += 1;
		}

		internal void ResumeLayout()
		{

			var wasSuspended = suspendLayoutCounter > 0;
			suspendLayoutCounter = Mathf.Max( 0, suspendLayoutCounter - 1 );
			if( wasSuspended && suspendLayoutCounter == 0 && pendingLayoutRequest )
			{
				PerformLayout();
			}

		}

		internal void PerformLayout()
		{

			if( disposed )
				return;

			if( suspendLayoutCounter > 0 )
			{
				pendingLayoutRequest = true;
			}
			else
			{
				performLayoutInternal();
			}

		}

		internal void Attach( dfControl ownerControl )
		{
			this.owner = ownerControl;
		}

		internal void Reset()
		{

			var cannotReset =
				anchorStyle == dfAnchorStyle.None ||
				IsPerformingLayout ||
				IsLayoutSuspended ||
				owner == null ||
				!owner.gameObject.activeSelf;

			if( cannotReset )
				return;

			var upperLeft = owner.RelativePosition;//.RoundToInt();
			var controlSize = owner.Size;//.RoundToInt();
			var parentSize = getParentSize();//.RoundToInt();

			var left = upperLeft.x;
			var top = upperLeft.y;
			var right = parentSize.x - controlSize.x - left;
			var bottom = parentSize.y - controlSize.y - top;

			if( margins == null ) margins = new dfAnchorMargins();

			margins.left = left;
			margins.right = right;
			margins.top = top;
			margins.bottom = bottom;

		}

		#endregion

		#region Private utility methods 

		protected void performLayoutInternal()
		{

			var cannotPerformLayout =
				margins == null ||
				anchorStyle == dfAnchorStyle.None ||
				IsPerformingLayout ||
				IsLayoutSuspended ||
				owner == null ||
				!owner.gameObject.activeSelf;

			if( cannotPerformLayout )
				return;

			try
			{

				performingLayout = true;
				pendingLayoutRequest = false;

				var parentSize = getParentSize();
				var controlSize = owner.Size;

				var left = margins.left;
				var top = margins.top;
				var right = left + controlSize.x;
				var bottom = top + controlSize.y;

				if( anchorStyle.IsFlagSet( dfAnchorStyle.CenterHorizontal ) )
				{
					left = Mathf.RoundToInt( ( parentSize.x - controlSize.x ) * 0.5f );
					right = Mathf.RoundToInt( left + controlSize.x );
				}
				else
				{

					if( anchorStyle.IsFlagSet( dfAnchorStyle.Left ) )
					{
						left = margins.left;
						right = left + controlSize.x;
					}

					if( anchorStyle.IsFlagSet( dfAnchorStyle.Right ) )
					{
						right = ( parentSize.x - margins.right );
						if( !anchorStyle.IsFlagSet( dfAnchorStyle.Left ) )
						{
							left = right - controlSize.x;
						}
					}

				}

				if( anchorStyle.IsFlagSet( dfAnchorStyle.CenterVertical ) )
				{
					top = Mathf.RoundToInt( ( parentSize.y - controlSize.y ) * 0.5f );
					bottom = Mathf.RoundToInt( top + controlSize.y );
				}
				else
				{

					if( anchorStyle.IsFlagSet( dfAnchorStyle.Top ) )
					{
						top = margins.top;
						bottom = top + controlSize.y;
					}

					if( anchorStyle.IsFlagSet( dfAnchorStyle.Bottom ) )
					{
						bottom = ( parentSize.y - margins.bottom );
						if( !anchorStyle.IsFlagSet( dfAnchorStyle.Top ) )
						{
							top = bottom - controlSize.y;
						}
					}

				}

				var newSize = new Vector2(
					Mathf.Max( 0, right - left ),
					Mathf.Max( 0, bottom - top )
					);
					
				// NOTE: It is very important to set Size before setting Position,
				// since the Position property relies on the value of the Size
				// property when calculating the transform position based on the 
				// current value of the Pivot property
				owner.Size = newSize; 
				owner.RelativePosition = new Vector3( left, top );

			}
			finally
			{
				performingLayout = false;
			}

		}

		private Vector2 getParentSize()
		{

			if( owner.parent != null )
				return owner.parent.Size;
				
			var manager = owner.GetManager();
			return manager.GetScreenSize();

		}

		#endregion

		#region System.Object overrides 

		/// <summary>
		/// Returns a formatted string summarizing this object's state
		/// </summary>
		public override string ToString()
		{

			if( owner == null )
				return "NO OWNER FOR ANCHOR";

			var parent = owner.parent;

			return string.Format( "{0}.{1} - {2}", parent != null ? parent.name : "SCREEN", owner.name, margins );

		}

		#endregion

	}

	#endregion

}

