/* Copyright 2013 Daikon Forge */

/****************************************************************************
 * PLEASE NOTE: The code in this file is under extremely active development
 * and is likely to change quite frequently. It is not recommended to modify
 * the code in this file, as your changes are likely to be overwritten by
 * the next product update when it is published.
 * **************************************************************************/

using UnityEngine;

using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using UnityColor = UnityEngine.Color;
using UnityMaterial = UnityEngine.Material;

/// <summary>
/// Used to display pseudo-HTML "rich text" 
/// </summary>
[Serializable]
[ExecuteInEditMode]
[RequireComponent( typeof( BoxCollider ) )]
[AddComponentMenu( "Daikon Forge/User Interface/Rich Text Label" )]
public class dfRichTextLabel : dfControl, IDFMultiRender
{

	#region Public events

	/// <summary>
	/// Raised whenever the value of the <see cref="Text"/> property changes
	/// </summary>
	public event PropertyChangedEventHandler<string> TextChanged;

	/// <summary>
	/// Raised when the value of the <see cref="ScrollPosition"/> property has changed
	/// </summary>
	public event PropertyChangedEventHandler<Vector2> ScrollPositionChanged;

	/// <summary>
	/// Defines the signature for methods which handle user clicks on anchor tags
	/// </summary>
	/// <param name="sender">The dfRichTextLabel control which raised the event</param>
	/// <param name="tag">Reference to the dfMarkupTag instance that was clicked</param>
	[dfEventCategory( "Markup" )]
	public delegate void LinkClickEventHandler( dfRichTextLabel sender, dfMarkupTagAnchor tag );

	/// <summary>
	/// Raised when the user clicks on a link 
	/// </summary>
	public event LinkClickEventHandler LinkClicked;

	#endregion

	#region Protected serialized fields

	[SerializeField]
	protected dfAtlas atlas;

	[SerializeField]
	protected dfDynamicFont font;

	[SerializeField]
	protected string text = "Rich Text Label";

	[SerializeField]
	protected int fontSize = 16;

	[SerializeField]
	protected FontStyle fontStyle = FontStyle.Normal;

	[SerializeField]
	protected string blankTextureSprite;

	[SerializeField]
	protected dfMarkupTextAlign align;

	[SerializeField]
	protected dfScrollbar horzScrollbar;

	[SerializeField]
	protected dfScrollbar vertScrollbar;

	[SerializeField]
	protected bool useScrollMomentum = false;

	#endregion

	#region Private variables 

	private static dfRenderData clipBuffer = new dfRenderData();

	private dfList<dfRenderData> buffers = new dfList<dfRenderData>();
	private dfList<dfMarkupElement> elements = null;
	private dfMarkupBox viewportBox = null;

	private dfMarkupTag mouseDownTag = null;
	private Vector2 mouseDownScrollPosition = Vector2.zero;

	private Vector2 scrollPosition = Vector2.zero;
	private bool initialized = false;
	private bool isMouseDown = false;
	private Vector2 touchStartPosition = Vector2.zero;
	private Vector2 scrollMomentum = Vector2.zero;
	private bool isMarkupInvalidated = true;

	#endregion

	#region Public properties

	/// <summary>
	/// The <see cref="dfAtlas">Texture Atlas</see> containing the images used by 
	/// the <see cref="dfRichTextLabel"/>
	/// </summary>
	public dfAtlas Atlas
	{
		get
		{
			if( this.atlas == null )
			{
				var view = GetManager();
				if( view != null )
				{
					return this.atlas = view.DefaultAtlas;
				}
			}
			return this.atlas;
		}
		set
		{
			if( !dfAtlas.Equals( value, atlas ) )
			{
				this.atlas = value;
				Invalidate();
			}
		}
	}

	/// <summary>
	/// Gets or sets the default TrueType or OpenType baseFont that will 
	/// be used to render the label text
	/// </summary>
	public dfDynamicFont Font
	{
		get { return this.font; }
		set
		{
			if( value != this.font )
			{
				this.font = value;
				Invalidate();
			}
		}
	}

	/// <summary>
	/// The name of the image in the <see cref="Atlas"/> that will be used to 
	/// render the selection, background, and cursor of this label
	/// </summary>
	public string BlankTextureSprite
	{
		get { return blankTextureSprite; }
		set
		{
			if( value != blankTextureSprite )
			{
				blankTextureSprite = value;
				Invalidate();
			}
		}
	}

	/// <summary>
	/// Gets or sets the text that will be displayed
	/// </summary>
	public string Text
	{
		get { return this.text; }
		set
		{
			if( !string.Equals( this.text, value ) )
			{
				this.text = value;
				scrollPosition = Vector2.zero;
				Invalidate();
				OnTextChanged();
			}
		}
	}

	/// <summary>
	/// Gets or sets the default size (in pixels) of the rendered text. Refers to the 
	/// maximum pixel height of each character.
	/// </summary>
	public int FontSize
	{
		get { return this.fontSize; }
		set
		{
			value = Mathf.Max( 6, value );
			if( value != this.fontSize )
			{
				this.fontSize = value;
				Invalidate();
			}
		}
	}

	/// <summary>
	/// Gets or sets the default font style that will be used to render label text
	/// </summary>
	public FontStyle FontStyle
	{
		get { return this.fontStyle; }
		set
		{
			if( value != this.fontStyle )
			{
				this.fontStyle = value;
				Invalidate();
			}
		}
	}

	/// <summary>
	/// Gets or sets the type of text alignment to use when rendering the text
	/// </summary>
	public dfMarkupTextAlign TextAlignment
	{
		get { return this.align; }
		set
		{
			if( value != align )
			{
				align = value;
				Invalidate();
			}
		}
	}

	/// <summary>
	/// Gets or sets the upper-left position of the viewport relative
	/// to the entire scrollable area
	/// </summary>
	public Vector2 ScrollPosition
	{
		get { return this.scrollPosition; }
		set
		{

			var maxPosition = ContentSize - Size;

			value = Vector2.Min( maxPosition, value );
			value = Vector2.Max( Vector2.zero, value );
			value = value.RoundToInt();

			if( ( value - this.scrollPosition ).sqrMagnitude > float.Epsilon )
			{
				this.scrollPosition = value;
				updateScrollbars();
				OnScrollPositionChanged();
			}

		}
	}

	/// <summary>
	/// Gets or sets a reference the the <see cref="dfScrollBar"/> instance
	/// that is used to scroll this control horizontally
	/// </summary>
	public dfScrollbar HorizontalScrollbar
	{
		get { return this.horzScrollbar; }
		set
		{
			this.horzScrollbar = value;
			updateScrollbars();
		}
	}

	/// <summary>
	/// Gets or sets a reference the the <see cref="dfScrollBar"/> instance
	/// that is used to scroll this control vertically
	/// </summary>
	public dfScrollbar VerticalScrollbar 
	{ 
		get { return this.vertScrollbar; }
		set
		{
			this.vertScrollbar = value;
			updateScrollbars();
		}
	}

	/// <summary>
	/// Returns the size (in pixels) of the rendered rich text context
	/// </summary>
	public Vector2 ContentSize
	{

		get
		{

			if( this.viewportBox != null )
				return viewportBox.Size;

			return this.Size;

		}

	}

	/// <summary>
	/// Gets or sets whether scrolling with the mousewheel or touch swipe will
	/// add a momentum effect to scrolling
	/// </summary>
	public bool UseScrollMomentum
	{
		get { return this.useScrollMomentum; }
		set { this.useScrollMomentum = value; scrollMomentum = Vector2.zero; }
	}

	#endregion

	#region dfControl overrides 

	public override void Invalidate()
	{
		base.Invalidate();
		isMarkupInvalidated = true;
	}

	public override void OnEnable()
	{
		
		base.OnEnable();
		
		if( this.size.sqrMagnitude <= float.Epsilon )
		{
			this.Size = new Vector2( 320, 200 );
		}

	}

	public override void Update()
	{

		base.Update();

		if( useScrollMomentum && !isMouseDown )
		{
			ScrollPosition += scrollMomentum;
		}

		scrollMomentum *= ( 0.95f - Time.deltaTime );

	}

	public override void LateUpdate()
	{

		base.LateUpdate();

		// HACK: Need to perform initialization after all dependant objects 
		initialize();

	}

	#endregion

	#region Event handlers

	protected internal void OnTextChanged()
	{

		Invalidate();

		Signal( "OnTextChanged", this.text );

		if( TextChanged != null )
		{
			TextChanged( this, this.text );
		}

	}

	protected internal void OnScrollPositionChanged()
	{

		// NOTE: Not using this.Invalidate() because the markup is still valid,
		// we just need to signal that a re-render is necessary at the new 
		// scroll position
		base.Invalidate();

		SignalHierarchy( "OnScrollPositionChanged", this.ScrollPosition );

		if( ScrollPositionChanged != null )
		{
			ScrollPositionChanged( this, this.ScrollPosition );
		}

	}

	protected internal override void OnKeyDown( dfKeyEventArgs args )
	{

		if( args.Used )
		{
			base.OnKeyDown( args );
			return;
		}

		var horzAmount = FontSize; // horzScrollbar != null ? horzScrollbar.IncrementAmount : FontSize;
		var vertAmount = FontSize; // vertScrollbar != null ? vertScrollbar.IncrementAmount : FontSize;

		if( args.KeyCode == KeyCode.LeftArrow )
		{
			ScrollPosition += new Vector2( -horzAmount, 0 );
			args.Use();
		}
		else if( args.KeyCode == KeyCode.RightArrow )
		{
			ScrollPosition += new Vector2( horzAmount, 0 );
			args.Use();
		}
		else if( args.KeyCode == KeyCode.UpArrow )
		{
			ScrollPosition += new Vector2( 0, -vertAmount );
			args.Use();
		}
		else if( args.KeyCode == KeyCode.DownArrow )
		{
			ScrollPosition += new Vector2( 0, vertAmount );
			args.Use();
		}

		base.OnKeyDown( args );

	}

	internal override void OnDragEnd( dfDragEventArgs args )
	{
		base.OnDragEnd( args );
		isMouseDown = false;
	}

	protected internal override void OnMouseEnter( dfMouseEventArgs args )
	{
		base.OnMouseEnter( args );
		touchStartPosition = args.Position;
	}

	protected internal override void OnMouseDown( dfMouseEventArgs args )
	{

		base.OnMouseDown( args );

		this.mouseDownTag = hitTestTag( args );
		this.mouseDownScrollPosition = scrollPosition;

		scrollMomentum = Vector2.zero;
		touchStartPosition = args.Position;
		isMouseDown = true;

	}

	protected internal override void OnMouseUp( dfMouseEventArgs args )
	{

		base.OnMouseUp( args );

		isMouseDown = false;

		if( Vector2.Distance( scrollPosition, mouseDownScrollPosition ) <= 2 )
		{

			if( hitTestTag( args ) == mouseDownTag )
			{

				var linkTag = mouseDownTag;
				while( linkTag != null && !( linkTag is dfMarkupTagAnchor ) )
				{
					linkTag = linkTag.Parent as dfMarkupTag;
				}

				if( linkTag is dfMarkupTagAnchor )
				{

					Signal( "OnLinkClicked", linkTag );

					if( this.LinkClicked != null )
					{
						this.LinkClicked( this, linkTag as dfMarkupTagAnchor );
					}

				}

			}

		}

		mouseDownTag = null;
		mouseDownScrollPosition = scrollPosition;

	}

	protected internal override void OnMouseMove( dfMouseEventArgs args )
	{

		base.OnMouseMove( args );

		var scrollWithDrag =
			args is dfTouchEventArgs ||
			isMouseDown;

		if( scrollWithDrag )
		{
			if( ( args.Position - touchStartPosition ).magnitude > 5 )
			{
				var delta = args.MoveDelta.Scale( -1, 1 );
				ScrollPosition += delta;
				scrollMomentum = ( scrollMomentum + delta ) * 0.5f;
			}
		}

	}

	protected internal override void OnMouseWheel( dfMouseEventArgs args )
	{

		try
		{

			if( args.Used )
				return;

			var wheelAmount = this.UseScrollMomentum ? 1 : 3;
			var amount = vertScrollbar != null ? vertScrollbar.IncrementAmount : FontSize * wheelAmount;

			ScrollPosition = new Vector2( scrollPosition.x, scrollPosition.y - amount * args.WheelDelta );
			scrollMomentum = new Vector2( 0, -amount * args.WheelDelta );

			args.Use();
			Signal( "OnMouseWheel", args );

		}
		finally
		{
			base.OnMouseWheel( args );
		}

	}

	#endregion 

	#region Public methods 

	/// <summary>
	/// Sets the scrollposition to the top
	/// </summary>
	public void ScrollToTop()
	{
		this.ScrollPosition = new Vector2( this.scrollPosition.x, 0 );
	}

	/// <summary>
	/// Sets the scrollposition to the top
	/// </summary>
	public void ScrollToBottom()
	{
		this.ScrollPosition = new Vector2( this.scrollPosition.x, int.MaxValue );
	}

	/// <summary>
	/// Sets the scrollposition to the top
	/// </summary>
	public void ScrollToLeft()
	{
		this.ScrollPosition = new Vector2( 0, this.scrollPosition.y );
	}

	/// <summary>
	/// Sets the scrollposition to the top
	/// </summary>
	public void ScrollToRight()
	{
		this.ScrollPosition = new Vector2( int.MaxValue, this.scrollPosition.y );
	}

	#endregion

	#region IDFMultiRender Members

	public dfList<dfRenderData> RenderMultiple()
	{

		if( !this.isVisible || this.Font == null )
			return null;

		if( !this.isControlInvalidated )
		{
			return this.buffers;
		}

		this.isControlInvalidated = false;

		Profiler.BeginSample( "Render " + this.name );
		try
		{

			// Parse the markup and perform document layout
			if( isMarkupInvalidated )
			{
				isMarkupInvalidated = false;
				processMarkup();
			}

			// Ensure that our viewport box has been properly resized to 
			// fully encompass all nodes, because the resulting size of 
			// the viewport box will be used to update scrollbars and 
			// determine max scroll position.
			viewportBox.FitToContents();
			updateScrollbars();

			Profiler.BeginSample( "Gather markup render buffers" );
			buffers.Clear();
			gatherRenderBuffers( viewportBox, this.buffers );
			Profiler.EndSample();

			return this.buffers;

		}
		finally
		{
			Profiler.EndSample();
		}

	}

	#endregion 

	#region Private utility methods 

	private dfMarkupTag hitTestTag( dfMouseEventArgs args )
	{

		var relativeMousePosition = this.GetHitPosition( args ) + scrollPosition;
		var hitBox = viewportBox.HitTest( relativeMousePosition );
		if( hitBox != null )
		{

			var tag = hitBox.Element;
			while( tag != null && !( tag is dfMarkupTag ) )
			{
				tag = tag.Parent;
			}

			return tag as dfMarkupTag;

		}

		return null;

	}

	private void processMarkup()
	{

		releaseMarkupReferences();

		this.elements = dfMarkupParser.Parse( this, this.text );

		var multiplier = (float)FontSize / (float)font.FontSize;

		var style = new dfMarkupStyle()
		{
			Host = this,
			Atlas = this.Atlas,
			Font = this.Font,
			FontSize = this.FontSize,
			FontStyle = this.FontStyle,
			LineHeight = (int)( this.Font.LineHeight * multiplier ),
			Color = this.Color,
			Align = this.TextAlignment,
			PreserveWhitespace = false
		};

		viewportBox = new dfMarkupBox( null, dfMarkupDisplayType.block, style )
		{
			Size = this.Size
		};

		Profiler.BeginSample( "Perform layout on markup" );
		for( int i = 0; i < elements.Count; i++ )
		{
			var child = elements[ i ];
			if( child != null )
			{
				child.PerformLayout( viewportBox, style );
			}
		}
		Profiler.EndSample();

	}

	private void releaseMarkupReferences()
	{

		this.mouseDownTag = null;

		if( viewportBox != null )
		{
			viewportBox.Release();
		}

		if( elements != null )
		{

			for( int i = 0; i < elements.Count; i++ )
			{
				elements[ i ].Release();
			}

			elements.Release();

		}

	}

	[HideInInspector]
	private void initialize()
	{

		if( initialized )
			return;

		initialized = true;

		if( Application.isPlaying )
		{

			if( horzScrollbar != null )
			{
				horzScrollbar.ValueChanged += horzScroll_ValueChanged;
			}

			if( vertScrollbar != null )
			{
				vertScrollbar.ValueChanged += vertScroll_ValueChanged;
			}

		}

		Invalidate();
		ScrollPosition = Vector2.zero;

		updateScrollbars();

	}

	private void vertScroll_ValueChanged( dfControl control, float value )
	{
		ScrollPosition = new Vector2( scrollPosition.x, value );
	}

	private void horzScroll_ValueChanged( dfControl control, float value )
	{
		ScrollPosition = new Vector2( value, ScrollPosition.y );
	}

	private void updateScrollbars()
	{

		if( horzScrollbar != null )
		{
			horzScrollbar.MinValue = 0;
			horzScrollbar.MaxValue = ContentSize.x;
			horzScrollbar.ScrollSize = Size.x;
			horzScrollbar.Value = ScrollPosition.x;
		}

		if( vertScrollbar != null )
		{
			vertScrollbar.MinValue = 0;
			vertScrollbar.MaxValue = ContentSize.y;
			vertScrollbar.ScrollSize = Size.y;
			vertScrollbar.Value = ScrollPosition.y;
		}

	}

	private void gatherRenderBuffers( dfMarkupBox box, dfList<dfRenderData> buffers )
	{

		var intersectionType = getViewportIntersection( box );
		if( intersectionType == dfIntersectionType.None )
		{
			return;
		}

		var buffer = box.Render();
		if( buffer != null )
		{

			if( buffer.Material == null )
			{
				if( this.atlas != null )
				{
					buffer.Material = atlas.material;
				}
			}

			var p2u = PixelsToUnits();
			var scroll = -scrollPosition.Scale( 1, -1 ).RoundToInt();
			var offset = (Vector3)( scroll + box.GetOffset().Scale( 1, -1 ) );
			
			var vertices = buffer.Vertices;
			for( int i = 0; i < buffer.Vertices.Count; i++ )
			{
				vertices[ i ] += offset;
			}

			var matrix = transform.localToWorldMatrix;
			for( int i = 0; i < buffer.Vertices.Count; i++ )
			{
				vertices[ i ] = matrix.MultiplyPoint( vertices[ i ] * p2u );
			}

			if( intersectionType == dfIntersectionType.Intersecting )
			{
				clipToViewport( buffer );
			}

			buffers.Add( buffer );

		}

		for( int i = 0; i < box.Children.Count; i++ )
		{
			gatherRenderBuffers( box.Children[ i ], buffers );
		}

	}

	private dfIntersectionType getViewportIntersection( dfMarkupBox box )
	{

		if( box.Display == dfMarkupDisplayType.none )
			return dfIntersectionType.None;

		var viewSize = this.Size;
		var min = box.GetOffset() - scrollPosition;
		var max = min + box.Size;

		if( max.x <= 0 || max.y <= 0 )
			return dfIntersectionType.None;

		if( min.x >= viewSize.x || min.y >= viewSize.y )
			return dfIntersectionType.None;

		if( min.x < 0 || min.y < 0 || max.x > viewSize.x || max.y > viewSize.y )
			return dfIntersectionType.Intersecting;

		return dfIntersectionType.Inside;

	}

	private void clipToViewport( dfRenderData renderData )
	{

		Profiler.BeginSample( "Clip markup box to viewport" );

		var planes = this.GetClippingPlanes();
		var material = renderData.Material;
		var matrix = renderData.Transform;

		dfClippingUtil.Clip( planes, renderData, clipBuffer );

		renderData.Clear();
		renderData.Merge( clipBuffer, false );
		renderData.Material = material;
		renderData.Transform = matrix;

		clipBuffer.Clear();

		Profiler.EndSample();

	}

	#endregion

}
