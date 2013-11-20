/* Copyright 2013 Daikon Forge */
using UnityEngine;

using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Provides a basic Button implementation that allows the developer
/// to specify individual sprite images to be used to represent common 
/// button states.
/// </summary>
[Serializable]
[ExecuteInEditMode]
[RequireComponent( typeof( BoxCollider ) )]
[AddComponentMenu( "Daikon Forge/User Interface/Button" )]
public class dfButton : dfInteractiveBase
{

	#region Public enums

	/// <summary>
	/// Represents the state of a Button
	/// </summary>
	public enum ButtonState : int
	{
		/// <summary>
		/// The default state of the button
		/// </summary>
		Default,
		/// <summary>
		/// Indicates that the button is current, i.e. has input focus or is 
		/// the selected button in a group
		/// </summary>
		Focus,
		/// <summary>
		/// Indicates that the mouse is hovering over the control
		/// </summary>
		Hover,
		/// <summary>
		/// Indicates that the user has pressed the button
		/// </summary>
		Pressed,
		/// <summary>
		/// Indicates that the control is disabled and cannot respond to
		/// user events
		/// </summary>
		Disabled
	}

	#endregion

	#region Public events 

	/// <summary>
	/// Raised whenever the button's State property changes
	/// </summary>
	public event PropertyChangedEventHandler<ButtonState> ButtonStateChanged;

	#endregion

	#region Protected serialized members

	[SerializeField]
	protected dfFont font;

	[SerializeField]
	protected string pressedSprite;

	[SerializeField]
	protected ButtonState state;

	[SerializeField]
	protected dfControl group = null;

	[SerializeField]
	protected string text = "";

	[SerializeField]
	protected TextAlignment textAlign = TextAlignment.Center;

	[SerializeField]
	protected dfVerticalAlignment vertAlign = dfVerticalAlignment.Middle;

	[SerializeField]
	protected Color32 textColor = UnityEngine.Color.white;

	[SerializeField]
	protected Color32 hoverText = UnityEngine.Color.white;

	[SerializeField]
	protected Color32 pressedText = UnityEngine.Color.white;

	[SerializeField]
	protected Color32 focusText = UnityEngine.Color.white;

	[SerializeField]
	protected Color32 disabledText = UnityEngine.Color.white; 

	[SerializeField]
	protected float textScale = 1f;

	[SerializeField]
	protected bool wordWrap = false;

	[SerializeField]
	protected RectOffset padding = new RectOffset();

	[SerializeField]
	protected bool textShadow = false;

	[SerializeField]
	protected Color32 shadowColor = UnityEngine.Color.black;

	[SerializeField]
	protected Vector2 shadowOffset = new Vector2( 1, -1 );

	[SerializeField]
	protected bool autoSize = false;

	#endregion

	#region Public properties

	/// <summary>
	/// Gets or sets the button's current ButtonState value
	/// </summary>
	public ButtonState State
	{
		get { return this.state; }
		set
		{
			if( value != this.state )
			{
				OnButtonStateChanged( value );
				Invalidate();
			}
		}
	}

	/// <summary>
	/// Gets or sets the name of the sprite that will be displayed when 
	/// the button date is set to <see cref="ButtonState.Pressed"/>
	/// </summary>
	public string PressedSprite
	{
		get { return pressedSprite; }
		set
		{
			if( value != pressedSprite )
			{
				pressedSprite = value;
				Invalidate();
			}
		}
	}

	/// <summary>
	/// If set, only one Button attached to the indicated dfControl
	/// can have its State property set to <see cref="ButtonState.Pressed"/> 
	/// at a time. This is used to emulate Toolbar and TabStrip 
	/// functionality.
	/// </summary>
	public dfControl ButtonGroup
	{
		get { return this.group; }
		set
		{
			if( value != group )
			{
				group = value;
				Invalidate();
			}
		}
	}

	/// <summary>
	/// If set to TRUE, the <see cref="dfButton"/> will be automatically sized to 
	/// fit the <see cref="Text"/>
	/// </summary>
	public bool AutoSize
	{
		get { return this.autoSize; }
		set
		{
			if( value != this.autoSize )
			{
				this.autoSize = value;
				if( value ) this.textAlign = TextAlignment.Left;
				Invalidate();
			}
		}
	}

	/// <summary>
	/// Gets or sets the text alignment that will be used to render the 
	/// button's caption: Left, Right, or Centered
	/// </summary>
	public TextAlignment TextAlignment
	{
		get 
		{
			if( this.autoSize )
				return TextAlignment.Left;
			return this.textAlign; 
		}
		set
		{
			if( value != textAlign )
			{
				textAlign = value;
				Invalidate();
			}
		}
	}

	/// <summary>
	/// Gets or sets the vertical alignment to use when rendering the text
	/// </summary>
	public dfVerticalAlignment VerticalAlignment
	{
		get { return this.vertAlign; }
		set
		{
			if( value != this.vertAlign )
			{
				this.vertAlign = value;
				Invalidate();
			}
		}
	}

	/// <summary>
	/// Gets or sets the amount of padding that will be used when rendering 
	/// the button's caption when the <see cref="AutoSize"/> property is set
	/// to TRUE.
	/// </summary>
	public RectOffset Padding
	{
		get
		{
			if( padding == null ) padding = new RectOffset();
			return this.padding;
		}
		set
		{
			value = value.ConstrainPadding();
			if( !RectOffset.Equals( value, this.padding ) )
			{
				this.padding = value;
				Invalidate();
			}
		}
	}

	/// <summary>
	/// Gets or sets the <see cref="dfFont"/> instance that will be used 
	/// to render the button's caption
	/// </summary>
	public dfFont Font
	{
		get
		{
			if( this.font == null )
			{
				var view = this.GetManager();
				if( view != null )
				{
					this.font = view.DefaultFont;
				}
			}
			return this.font;
		}
		set
		{
			if( Atlas != null && value != this.font )
			{
				if( value != null )
				{
					if( atlas.material != value.Atlas.material )
					{
						throw new ArgumentException( "The assigned Font must use the same Atlas as the control" );
					}
				}
				this.font = value;
			}
			Invalidate();
		}
	}

	/// <summary>
	/// Gets or sets the button's caption text
	/// </summary>
	public string Text
	{
		get { return this.text; }
		set
		{
			if( value != this.text )
			{
				this.text = value;
				Invalidate();
			}
		}
	}

	/// <summary>
	/// Gets or sets the Color that will be used when rendering the 
	/// button's caption in its Normal state
	/// </summary>
	public Color32 TextColor
	{
		get { return this.textColor; }
		set
		{
			this.textColor = value;
			Invalidate();
		}
	}

	/// <summary>
	/// Gets or sets the Color that will be used when rendering the 
	/// button's caption in its Hover state
	/// </summary>
	public Color32 HoverTextColor
	{
		get { return this.hoverText; }
		set
		{
			this.hoverText = value;
			Invalidate();
		}
	}

	/// <summary>
	/// Gets or sets the Color that will be used when rendering the 
	/// button's caption in its Pressed state
	/// </summary>
	public Color32 PressedTextColor
	{
		get { return this.pressedText; }
		set
		{
			this.pressedText = value;
			Invalidate();
		}
	}

	/// <summary>
	/// Gets or sets the Color that will be used when rendering the 
	/// button's caption in its Focused state
	/// </summary>
	public Color32 FocusTextColor
	{
		get { return this.focusText; }
		set
		{
			this.focusText = value;
			Invalidate();
		}
	}

	/// <summary>
	/// Gets or sets the Color that will be used when rendering the 
	/// button's caption in its Disabled state
	/// </summary>
	public Color32 DisabledTextColor
	{
		get { return this.disabledText; }
		set
		{
			this.disabledText = value;
			Invalidate();
		}
	}

	/// <summary>
	/// Gets or sets the size multiplier that will be used when rendering
	/// the button's caption
	/// </summary>
	public float TextScale
	{
		get { return this.textScale; }
		set
		{
			value = Mathf.Max( 0.1f, value );
			if( !Mathf.Approximately( textScale, value ) )
			{
				this.textScale = value;
				Invalidate();
			}
		}
	}

	/// <summary>
	/// Gets or sets whether the button's caption will be word-wrapped
	/// when too long to fit as a single line of text
	/// </summary>
	public bool WordWrap
	{
		get { return this.wordWrap; }
		set
		{
			if( value != wordWrap )
			{
				wordWrap = value;
				Invalidate();
			}
		}
	}

	/// <summary>
	/// Gets or sets whether the button's caption will be rendered with 
	/// a shadow
	/// </summary>
	public bool Shadow
	{
		get { return this.textShadow; }
		set
		{
			if( value != textShadow )
			{
				textShadow = value;
				Invalidate();
			}
		}
	}

	/// <summary>
	/// Gets or sets the topColor that will be used to render the caption's shadow
	/// if the <see cref="Shadow"/> property is set to TRUE
	/// </summary>
	public Color32 ShadowColor
	{
		get { return this.shadowColor; }
		set
		{
			if( !value.Equals( shadowColor ) )
			{
				shadowColor = value;
				Invalidate();
			}
		}
	}

	/// <summary>
	/// Gets or sets the distance in pixels that the caption's shadow will 
	/// be offset if the <see cref="Shadow"/> property is set to TRUE
	/// </summary>
	public Vector2 ShadowOffset
	{
		get { return this.shadowOffset; }
		set
		{
			if( value != shadowOffset )
			{
				shadowOffset = value;
				Invalidate();
			}
		}
	}

	#endregion 

	#region Overrides and event handling 

	public override void Invalidate()
	{

		base.Invalidate();

		if( this.AutoSize )
		{
			this.autoSizeToText();
		}

	}

	public override void OnEnable()
	{

		base.OnEnable();

		#region Ensure that this control always has a valid font, if possible 

		var validFont =
			Font != null &&
			Font.IsValid;

		if( Application.isPlaying && !validFont )
		{
			Font = GetManager().DefaultFont;
		}

		#endregion

	}

	public override void Update()
	{
		base.Update();
	}

	protected internal override void OnEnterFocus( dfFocusEventArgs args )
	{
	
		if( this.State != ButtonState.Pressed )
		{
			this.State = ButtonState.Focus;
		}

		base.OnEnterFocus( args );
		
	}

	protected internal override void OnLeaveFocus( dfFocusEventArgs args )
	{
		this.State = ButtonState.Default;
		base.OnLeaveFocus( args );
	}

	protected internal override void OnKeyPress( dfKeyEventArgs args )
	{
	
		if( args.KeyCode == KeyCode.Space )
		{
			OnClick( new dfMouseEventArgs( this, dfMouseButtons.Left, 1, new Ray(), Vector2.zero, 0 ) );
			return;
		}
		
		base.OnKeyPress( args );
		
	}

	protected internal override void OnClick( dfMouseEventArgs args )
	{

		if( group != null )
		{

			var list = transform.parent.GetComponentsInChildren<dfButton>();
			for( int i = 0; i < list.Length; i++ )
			{
				var control = list[ i ];
				if( control != this && control.ButtonGroup == this.ButtonGroup )
				{
					if( control != this )
					{
						control.State = ButtonState.Default;
					}
				}
			}

			// Need to manually signal the Group object if it's not part 
			// of this dfControl's hierarchy
			if( !transform.IsChildOf( group.transform ) )
			{
				Signal( group.gameObject, "OnClick", args );
			}

		}

		base.OnClick( args );

	}

	protected internal override void OnMouseDown( dfMouseEventArgs args )
	{

		this.State = ButtonState.Pressed;

		base.OnMouseDown( args );

	}

	protected internal override void OnMouseUp( dfMouseEventArgs args )
	{

		if( HasFocus )
		{
			this.State = ButtonState.Focus;
		}
#if !UNITY_IPHONE && !UNITY_ANDROID
		else if( isMouseHovering )
		{
			this.State = ButtonState.Hover;
		}
#endif
		else
		{
			this.State = ButtonState.Default;
		}

		base.OnMouseUp( args );

	}

	protected internal override void OnMouseEnter( dfMouseEventArgs args )
	{

#if !UNITY_IPHONE && !UNITY_ANDROID
		this.State = ButtonState.Hover;
#endif
		base.OnMouseEnter( args );

	}

	protected internal override void OnMouseLeave( dfMouseEventArgs args )
	{

		if( this.ContainsFocus )
			this.State = ButtonState.Focus;
		else
			this.State = ButtonState.Default;

		base.OnMouseLeave( args );

	}

	protected internal override void OnIsEnabledChanged()
	{

		if( !this.IsEnabled )
		{
			this.State = ButtonState.Disabled;
		}
		else
		{
			this.State = ButtonState.Default;
		}

		base.OnIsEnabledChanged();

	}

	protected virtual void OnButtonStateChanged( ButtonState value )
	{

		// Cannot change button state when button is disabled
		if( !this.isEnabled && value != ButtonState.Disabled )
		{
			return;
		}

		this.state = value;

		Signal( "OnButtonStateChanged", value );

		if( ButtonStateChanged != null )
		{
			ButtonStateChanged( this, value );
		}

		Invalidate();

	}

	protected override void OnRebuildRenderData()
	{

		if( Atlas == null )
			return;

		renderData.Material = Atlas.material;

		renderBackground();
		renderText();

	}

	#endregion

	#region Private utility methods

	private void autoSizeToText()
	{

		if( Font == null || Font.Atlas == null || string.IsNullOrEmpty( Text ) )
			return;

		using( var textRenderer = obtainTextRenderer() )
		{

			var textSize = textRenderer.MeasureString( this.Text );
			var newSize = new Vector2( textSize.x + padding.horizontal, textSize.y + padding.vertical );

			this.Size = newSize;

		}

	}

	private void renderText()
	{

		if( Font == null || Font.Atlas == null || string.IsNullOrEmpty( Text ) )
			return;

		using( var textRenderer = obtainTextRenderer() )
		{
			textRenderer.Render( text, renderData );
		}

	}

	private dfFont.TextRenderer obtainTextRenderer()
	{

		var p2u = PixelsToUnits();
		var maxSize = new Vector2( this.size.x - padding.horizontal, this.size.y - padding.vertical );
		if( AutoSize )
		{
			maxSize = Vector2.one * int.MaxValue;
		}

		var pivotOffset = pivot.TransformToUpperLeft( Size );
		var origin = new Vector3(
			pivotOffset.x + padding.left,
			pivotOffset.y - padding.top,
			0
		) * p2u;

		var renderColor = ApplyOpacity( getTextColorForState() );

		var textRenderer = font.ObtainRenderer();

		textRenderer.WordWrap = wordWrap;
		textRenderer.MultiLine = wordWrap;
		textRenderer.MaxSize = maxSize;
		textRenderer.PixelRatio = p2u;
		textRenderer.TextScale = TextScale;
		textRenderer.VectorOffset = origin;
		textRenderer.TextAlign = TextAlignment;
		textRenderer.ProcessMarkup = true;
		textRenderer.DefaultColor = renderColor;
		textRenderer.OverrideMarkupColors = false;
		textRenderer.Opacity = this.CalculateOpacity();
		textRenderer.Shadow = Shadow;
		textRenderer.ShadowColor = ShadowColor;
		textRenderer.ShadowOffset = ShadowOffset;

		if( !AutoSize && this.vertAlign != dfVerticalAlignment.Top )
		{
			textRenderer.VectorOffset = getVertAlignOffset( textRenderer );
		}

		return textRenderer;

	}

	private Color32 getTextColorForState()
	{

		if( !IsEnabled )
			return this.DisabledTextColor;

		switch( this.state )
		{
			case ButtonState.Default:
				return this.TextColor;
			case ButtonState.Focus:
				return this.FocusTextColor;
			case ButtonState.Hover:
				return this.HoverTextColor;
			case ButtonState.Pressed:
				return this.PressedTextColor;
			case ButtonState.Disabled:
				return this.DisabledTextColor;
		}

		return UnityEngine.Color.white;

	}

	private Vector3 getVertAlignOffset( dfFont.TextRenderer textRenderer )
	{

		var p2u = PixelsToUnits();
		var renderedSize = textRenderer.MeasureString( this.text ) * p2u;
		var origin = textRenderer.VectorOffset;
		var clientHeight = ( Height - padding.vertical ) * p2u;

		if( renderedSize.y >= clientHeight )
			return origin;

		switch( this.vertAlign )
		{
			case dfVerticalAlignment.Middle:
				origin.y -= ( clientHeight - renderedSize.y ) * 0.5f;
				break;
			case dfVerticalAlignment.Bottom:
				origin.y -= clientHeight - renderedSize.y;
				break;
		}

		return origin;

	}

	protected internal override dfAtlas.ItemInfo getBackgroundSprite()
	{

		if( Atlas == null )
			return null;

		var result = (dfAtlas.ItemInfo)null;

		switch( this.state )
		{

			case ButtonState.Default:
				result = atlas[ backgroundSprite ];
				break;

			case ButtonState.Focus:
				result = atlas[ focusSprite ];
				break;

			case ButtonState.Hover:
				result = atlas[ hoverSprite ];
				break;

			case ButtonState.Pressed:
				result = atlas[ pressedSprite ];
				break;

			case ButtonState.Disabled:
				result = atlas[ disabledSprite ];
				break;

		}

		// TODO: Implement some sort of logic-based "fallback" logic when indicated sprite is not available?

		if( result == null ) result = atlas[ backgroundSprite ];

		return result;

	}

	#endregion 

}

