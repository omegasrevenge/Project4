/* Copyright 2013 Daikon Forge */
using UnityEngine;

using System;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Implements a text entry control.
/// </summary>
[Serializable]
[ExecuteInEditMode]
[RequireComponent( typeof( BoxCollider ) )]
[AddComponentMenu( "Daikon Forge/User Interface/Textbox" )]
public class dfTextbox : dfInteractiveBase
{

	#region Public events 

	/// <summary>
	/// Raised whenever the value of the <see cref="ReadOnly"/> property has changed
	/// </summary>
	public event PropertyChangedEventHandler<bool> ReadOnlyChanged;

	/// <summary>
	/// Raised whenever the value of the <see cref="PasswordCharacter"/> property has changed
	/// </summary>
	public event PropertyChangedEventHandler<string> PasswordCharacterChanged;

	/// <summary>
	/// Raised whenever the value of the <see cref="Text"/> property has changed
	/// </summary>
	public event PropertyChangedEventHandler<string> TextChanged;

	/// <summary>
	/// Raised when the user has indicated that they are done entering text, 
	/// such as by pressing the RETURN key when this control has input focus
	/// </summary>
	public event PropertyChangedEventHandler<string> TextSubmitted;

	/// <summary>
	/// Raised when the user has indicated that they would like to abort
	/// editing of the <see cref="Text"/> and would like to revert to the 
	/// previous value, such as by pressing the ESC key when this control 
	/// has input focus
	/// </summary>
	public event PropertyChangedEventHandler<string> TextCancelled;

	#endregion

	#region Protected serialized fields

	[SerializeField]
	protected dfFont font;

	[SerializeField]
	protected bool acceptsTab = false;

	[SerializeField]
	protected bool displayAsPassword = false;

	[SerializeField]
	protected string passwordChar = "*";

	[SerializeField]
	protected bool readOnly = false;

	[SerializeField]
	protected string text = "";

	[SerializeField]
	protected Color32 textColor = UnityEngine.Color.white;

	[SerializeField]
	protected Color32 selectionBackground = new Color32( 0, 105, 210, 255 );

	[SerializeField]
	protected string selectionSprite = "";

	[SerializeField]
	protected float textScale = 1f;

	[SerializeField]
	protected dfTextScaleMode textScaleMode = dfTextScaleMode.None;

	[SerializeField]
	protected RectOffset padding = new RectOffset();

	[SerializeField]
	protected float cursorBlinkTime = 0.45f;

	[SerializeField]
	protected int cursorWidth = 1;

	[SerializeField]
	protected int maxLength = 1024;

	[SerializeField]
	protected bool selectOnFocus = false;

	[SerializeField]
	protected bool shadow = false;

	[SerializeField]
	protected Color32 shadowColor = UnityEngine.Color.black;

	[SerializeField]
	protected Vector2 shadowOffset = new Vector2( 1, -1 );

	[SerializeField]
	protected bool useMobileKeyboard = false;

	[SerializeField]
	protected int mobileKeyboardType = 0;

	[SerializeField]
	protected bool mobileAutoCorrect = false;

	[SerializeField]
	protected bool mobileHideInputField = false;

	[SerializeField]
	protected dfMobileKeyboardTrigger mobileKeyboardTrigger = dfMobileKeyboardTrigger.Manual;

	[SerializeField]
	protected TextAlignment textAlign;

	#endregion

	#region Private unserialized fields 

	private Vector2 startSize = Vector2.zero;

	private int selectionStart = 0;
	private int selectionEnd = 0;
	private int mouseSelectionAnchor = 0;
	private int scrollIndex = 0;
	private int cursorIndex = 0;
	private float leftOffset = 0f;
	private bool cursorShown = false;
	private float[] charWidths;
	private float whenGotFocus = 0f;
	private string undoText = "";

#if UNITY_IPHONE || UNITY_ANDROID || UNITY_BLACKBERRY || UNITY_WP8 || UNITY_EDITOR
	private static TouchScreenKeyboard mobileKeyboard;
#endif

	#endregion

	#region Public properties

	/// <summary>
	/// Gets or sets a reference to the <see cref="dfFont"/> that will be 
	/// used to render the text for this control
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
			if( value != this.font )
			{
				this.font = value;
				Invalidate();
			}
		}
	}

	/// <summary>
	/// Gets or sets the character index for the start of the text selection range
	/// </summary>
	public int SelectionStart
	{
		get { return this.selectionStart; }
		set
		{
			if( value != selectionStart )
			{
				selectionStart = Mathf.Max( 0, Mathf.Min( value, text.Length ) );
				selectionEnd = Mathf.Max( selectionEnd, selectionStart );
				Invalidate();
			}
		}
	}

	/// <summary>
	/// Gets or sets the character index for the end of the text selection range
	/// </summary>
	public int SelectionEnd
	{
		get { return this.selectionEnd; }
		set
		{
			if( value != selectionEnd )
			{
				selectionEnd = Mathf.Max( 0, Mathf.Min( value, text.Length ) );
				selectionStart = Mathf.Max( selectionStart, selectionEnd );
				Invalidate();
			}
		}
	}

	/// <summary>
	/// Returns the length of the selected text
	/// </summary>
	public int SelectionLength
	{
		get { return selectionEnd - selectionStart; }
	}

	/// <summary>
	/// Returns the value of the selected text
	/// </summary>
	public string SelectedText
	{
		get
		{
			if( selectionEnd == selectionStart )
				return "";
			return text.Substring( selectionStart, selectionEnd - selectionStart );
		}
	}

	/// <summary>
	/// If set to TRUE, then all text will be selected when this control
	/// receives input focus
	/// </summary>
	public bool SelectOnFocus
	{
		get { return this.selectOnFocus; }
		set { this.selectOnFocus = value; }
	}

	/// <summary>
	/// Gets or sets the amount of padding that will be applied when 
	/// rendering text for this control
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
	/// Gets or sets a value indicating whether this control will be used
	/// for entering passwords. If set to TRUE, then only the character 
	/// specified by the <see cref="PasswordCharacter"/> property will be
	/// displayed instead of the actual text
	/// </summary>
	public bool IsPasswordField
	{
		get { return this.displayAsPassword; }
		set
		{
			if( value != this.displayAsPassword )
			{
				this.displayAsPassword = value;
				Invalidate();
			}
		}
	}

	/// <summary>
	/// Gets or sets the character that will be substituted for each
	/// character of text entered when <see cref="IsPasswordField"/>
	/// is set to TRUE
	/// </summary>
	public string PasswordCharacter
	{
		get { return this.passwordChar; }
		set
		{
			if( !string.IsNullOrEmpty( value ) )
			{
				passwordChar = value[ 0 ].ToString();
			}
			else
			{
				passwordChar = value;
			}
			OnPasswordCharacterChanged();
			Invalidate();
		}
	}

	/// <summary>
	/// Gets or sets the amount of time in seconds that the caret will blink
	/// </summary>
	public float CursorBlinkTime
	{
		get { return this.cursorBlinkTime; }
		set { cursorBlinkTime = value; }
	}

	/// <summary>
	/// Gets or sets the width of the caret, in pixels
	/// </summary>
	public int CursorWidth
	{
		get { return this.cursorWidth; }
		set { this.cursorWidth = value; }
	}

	/// <summary>
	/// Gets or sets the character position of the cursor
	/// </summary>
	public int CursorIndex
	{
		get { return this.cursorIndex; }
		set { this.cursorIndex = value; }
	}

	/// <summary>
	/// Gets or sets a value indicating whether the user is allowed to 
	/// change the value of the <see cref="Text"/>
	/// </summary>
	public bool ReadOnly
	{
		get { return this.readOnly; }
		set
		{
			if( value != this.readOnly )
			{
				this.readOnly = value;
				OnReadOnlyChanged();
				Invalidate();
			}
		}
	}

	/// <summary>
	/// Gets or sets the text value
	/// </summary>
	public string Text
	{
		get { return this.text; }
		set
		{
			if( value.Length > MaxLength )
			{
				value = value.Substring( 0, MaxLength );
			}
			value = value.Replace( "\t", " " );
			if( value != this.text )
			{
				this.text = value;
				scrollIndex = cursorIndex = 0;
				OnTextChanged();
				Invalidate();
			}
		}
	}

	/// <summary>
	/// Gets or sets the topColor that will be used to render text for this control
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
	/// Specifies the name of a sprite that will be used to render the 
	/// text selection background and the caret.
	/// </summary>
	public string SelectionSprite
	{
		get { return this.selectionSprite; }
		set
		{
			if( value != this.selectionSprite )
			{
				this.selectionSprite = value;
				Invalidate();
			}
		}
	}

	/// <summary>
	/// Gets or sets the topColor that will be used to render the text 
	/// selection background
	/// </summary>
	public Color32 SelectionBackgroundColor
	{
		get { return this.selectionBackground; }
		set
		{
			this.selectionBackground = value;
			Invalidate();
		}
	}

	/// <summary>
	/// Gets or sets the size multiplier that will be applied to 
	/// all text rendered for this control
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
	/// Gets or sets whether the TextScale property will be automatically 
	/// adjusted to match runtime screen resolution
	/// </summary>
	public dfTextScaleMode TextScaleMode
	{
		get { return this.textScaleMode; }
		set { this.textScaleMode = value; Invalidate(); }
	}

	/// <summary>
	/// Gets or sets the maximum number of characters that can be entered
	/// by the user
	/// </summary>
	public int MaxLength
	{
		get { return this.maxLength; }
		set
		{
			if( value != this.maxLength )
			{
				this.maxLength = Mathf.Max( 0, value );
				if( maxLength < text.Length )
				{
					Text = text.Substring( 0, maxLength );
				}
				Invalidate();
			}
		}
	}

	/// <summary>
	/// Gets or sets the type of text alignment to use when rendering the text
	/// </summary>
	public TextAlignment TextAlignment
	{
		get { return this.textAlign; }
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
	/// Gets or sets a value indicating whether text will be rendered
	/// with a shadow
	/// </summary>
	public bool Shadow
	{
		get { return this.shadow; }
		set
		{
			if( value != shadow )
			{
				shadow = value;
				Invalidate();
			}
		}
	}

	/// <summary>
	/// Gets or sets the topColor that will be used to render text shadows
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
	/// Gets or sets the distance that text shadows will be offset
	/// if the <see cref="Shadow"/> is set to TRUE
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

	/// <summary>
	/// Gets or sets whether to use the on-screen keyboard on mobile devices
	/// </summary>
	public bool UseMobileKeyboard
	{
		get { return this.useMobileKeyboard; }
		set { this.useMobileKeyboard = value; }
	}

#if UNITY_IPHONE || UNITY_ANDROID || UNITY_BLACKBERRY || UNITY_WP8 || UNITY_EDITOR
	/// <summary>
	/// Gets or sets the type of on-screen keyboard to display
	/// </summary>
	public TouchScreenKeyboardType MobileKeyboardType
	{
		get { return (TouchScreenKeyboardType)this.mobileKeyboardType; }
		set { this.mobileKeyboardType = (int)value; }
	}
#endif

	/// <summary>
	/// Gets or sets whether to use the auto-correct feature on mobile devices
	/// </summary>
	public bool MobileAutoCorrect
	{
		get { return this.mobileAutoCorrect; }
		set { this.mobileAutoCorrect = value; }
	}

	/// <summary>
	/// Gets or sets whether the input field will be visible when the 
	/// mobile keyboard is active
	/// </summary>
	public bool HideMobileInputField
	{
		get { return this.mobileHideInputField; }
		set { this.mobileHideInputField = value; }
	}

	/// <summary>
	/// Gets or sets the condition that will cause the on-screen keyboard
	/// to be displayed on mobile devices
	/// </summary>
	public dfMobileKeyboardTrigger MobileKeyboardTrigger
	{
		get { return this.mobileKeyboardTrigger; }
		set { this.mobileKeyboardTrigger = value; }
	}

	#endregion

	#region Overrides and events

	protected override void OnTabKeyPressed( dfKeyEventArgs args )
	{

		if( acceptsTab )
		{

			// Give event observers the opportunity to cancel the event
			base.OnKeyPress( args );
			if( args.Used )
				return;

			// Handle the tab key like any other key
			args.Character = '\t';
			processKeyPress( args );

		}
		else
		{
			base.OnTabKeyPressed( args );
		}

	}

	protected internal override void OnKeyPress( dfKeyEventArgs args )
	{

		if( ReadOnly || args.KeyCode < KeyCode.Space || args.KeyCode > KeyCode.Z )
		{
			base.OnKeyPress( args );
			return;
		}

		// Give event observers the opportunity to cancel the event
		base.OnKeyPress( args );
		if( args.Used )
			return;

#if !( UNITY_IPHONE || UNITY_ANDROID || UNITY_BLACKBERRY || UNITY_WP8 ) || UNITY_EDITOR

		processKeyPress( args );

#endif

	}

	private void processKeyPress( dfKeyEventArgs args )
	{

		deleteSelection();

		if( text.Length < MaxLength )
		{

			if( cursorIndex == text.Length )
			{
				text += args.Character;
			}
			else
			{
				text = text.Insert( cursorIndex, args.Character.ToString() );
			}

			cursorIndex += 1;

			OnTextChanged();
			Invalidate();

		}

		args.Use();

	}

	protected internal override void OnKeyDown( dfKeyEventArgs args )
	{

		if( ReadOnly )
			return;

		// Give event observers the opportunity to cancel the event
		base.OnKeyDown( args );
		if( args.Used )
			return;

#if !( UNITY_IPHONE || UNITY_ANDROID || UNITY_BLACKBERRY || UNITY_WP8 ) || UNITY_EDITOR

		switch( args.KeyCode )
		{
			case KeyCode.A:
				if( args.Control )
				{
					selectAll();
				}
				break;
			case KeyCode.Insert:
				if( args.Shift )
				{
					var clipData = dfClipboardHelper.clipBoard;
					if( !string.IsNullOrEmpty( clipData ) )
					{
						pasteAtCursor( clipData );
					}
				}
				break;
			case KeyCode.V:
				if( args.Control )
				{
					var clipData = dfClipboardHelper.clipBoard;
					if( !string.IsNullOrEmpty( clipData ) )
					{
						pasteAtCursor( clipData );
					}
				}
				break;
			case KeyCode.C:
				if( args.Control )
				{
					copySelectionToClipboard();
				}
				break;
			case KeyCode.X:
				if( args.Control )
				{
					cutSelectionToClipboard();
				}
				break;
			case KeyCode.LeftArrow:
				if( args.Control )
				{
					if( args.Shift )
						moveSelectionPointLeftWord();
					else
						moveToPreviousWord();
				}
				else if( args.Shift )
					moveSelectionPointLeft();
				else
					moveToPreviousChar();
				break;
			case KeyCode.RightArrow:
				if( args.Control )
				{
					if( args.Shift )
						moveSelectionPointRightWord();
					else
						moveToNextWord();
				}
				else if( args.Shift )
					moveSelectionPointRight();
				else
					moveToNextChar();
				break;
			case KeyCode.Home:
				if( args.Shift )
					selectToStart();
				else
					moveToStart();
				break;
			case KeyCode.End:
				if( args.Shift )
					selectToEnd();
				else
					moveToEnd();
				break;
			case KeyCode.Delete:
				if( selectionStart != selectionEnd )
					deleteSelection();
				else if( args.Control )
					deleteNextWord();
				else
					deleteNextChar();
				break;
			case KeyCode.Backspace:
				if( args.Control )
					deletePreviousWord();
				else
					deletePreviousChar();
				break;
			case KeyCode.Escape:
				ClearSelection();
				cursorIndex = scrollIndex = 0;
				Invalidate();
				OnCancel();
				break;
			case KeyCode.Return:
				OnSubmit();
				break;
			default:
				base.OnKeyDown( args );
				return;
		}

		args.Use();

#endif

	}

	private void selectAll()
	{
		selectionStart = 0;
		selectionEnd = text.Length;
		scrollIndex = 0;
		setCursorPos( 0 );
	}

	private void cutSelectionToClipboard()
	{
		copySelectionToClipboard();
		deleteSelection();
	}

	private void copySelectionToClipboard()
	{

		if( selectionStart == selectionEnd )
			return;

		dfClipboardHelper.clipBoard = text.Substring( selectionStart, selectionEnd - selectionStart );

	}

	private void pasteAtCursor( string clipData )
	{

		deleteSelection();

		var buffer = new System.Text.StringBuilder( text.Length + clipData.Length );
		buffer.Append( text );

		for( int i = 0; i < clipData.Length; i++ )
		{
			var ch = clipData[ i ];
			if( ch >= ' ' )
			{
				buffer.Insert( cursorIndex++, ch );
			}
		}

		buffer.Length = Mathf.Min( buffer.Length, maxLength );
		text = buffer.ToString();

		setCursorPos( cursorIndex );

		OnTextChanged();
		Invalidate();

	}

	private void selectWordAtIndex( int index )
	{

		index = Mathf.Max( Mathf.Min( text.Length - 1, index ), 0 );

		var ch = text[ index ];

		if( !char.IsLetterOrDigit( ch ) )
		{
			selectionStart = index;
			selectionEnd = index + 1;
			mouseSelectionAnchor = 0;
		}
		else
		{

			selectionStart = index;
			for( int i = index; i > 0; i-- )
			{
				if( char.IsLetterOrDigit( text[ i - 1 ] ) )
					selectionStart -= 1;
				else
					break;
			}

			selectionEnd = index;
			for( int i = index; i < text.Length; i++ )
			{
				if( char.IsLetterOrDigit( text[ i ] ) )
					selectionEnd = i + 1;
				else
					break;
			}

		}

		cursorIndex = selectionStart;

		Invalidate();

	}

	private void moveToNextWord()
	{

		ClearSelection();

		if( cursorIndex == text.Length )
			return;

		var index = findNextWord( cursorIndex );
		setCursorPos( index );

	}

	private void moveToPreviousWord()
	{

		ClearSelection();

		if( cursorIndex == 0 )
			return;

		int index = findPreviousWord( cursorIndex );
		setCursorPos( index );

	}

	private void deletePreviousChar()
	{

		if( selectionStart != selectionEnd )
		{
			var index = selectionStart;
			deleteSelection();
			setCursorPos( index );
			return;
		}
		
		ClearSelection();

		if( cursorIndex == 0 )
			return;

		text = text.Remove( cursorIndex - 1, 1 );

		cursorIndex -= 1;
		cursorShown = true;

		OnTextChanged();
		Invalidate();
		
	}

	private void deletePreviousWord()
	{

		ClearSelection();
		if( cursorIndex == 0 )
			return;

		int startIndex = findPreviousWord( cursorIndex );
		if( startIndex == cursorIndex )
			startIndex = 0;

		text = text.Remove( startIndex, cursorIndex - startIndex );

		setCursorPos( startIndex );

		OnTextChanged();
		Invalidate();

	}

	private void deleteSelection()
	{

		if( selectionStart == selectionEnd )
			return;

		text = text.Remove( selectionStart, selectionEnd - selectionStart );

		setCursorPos( selectionStart );
		ClearSelection();

		OnTextChanged();
		Invalidate();

	}

	private void deleteNextChar()
	{

		ClearSelection();
		if( cursorIndex >= text.Length )
			return;

		text = text.Remove( cursorIndex, 1 );
		cursorShown = true;

		OnTextChanged();
		Invalidate();
		
	}

	private void deleteNextWord()
	{

		ClearSelection();
		if( cursorIndex == text.Length )
			return;

		int endIndex = findNextWord( cursorIndex );
		if( endIndex == cursorIndex )
			endIndex = text.Length;

		text = text.Remove( cursorIndex, endIndex - cursorIndex );

		OnTextChanged();
		Invalidate();

	}

	private void selectToStart()
	{

		if( cursorIndex == 0 )
			return;

		if( selectionEnd == selectionStart )
		{
			selectionEnd = cursorIndex;
		}
		else if( selectionEnd == cursorIndex )
		{
			selectionEnd = selectionStart;
		}

		selectionStart = 0;
		setCursorPos( 0 );

	}

	private void selectToEnd()
	{

		if( cursorIndex == text.Length )
			return;

		if( selectionEnd == selectionStart )
		{
			selectionStart = cursorIndex;
		}
		else if( selectionStart == cursorIndex )
		{
			selectionStart = selectionEnd;
		}

		selectionEnd = text.Length;
		setCursorPos( text.Length );

	}

	private void moveToEnd()
	{
		ClearSelection();
		setCursorPos( text.Length );
	}

	private void moveToStart()
	{
		ClearSelection();
		setCursorPos( 0 );
	}

	private void moveToNextChar()
	{
		ClearSelection();
		setCursorPos( cursorIndex + 1 );
	}

	private void moveSelectionPointRightWord()
	{

		if( cursorIndex == text.Length )
			return;

		var nextWordIndex = findNextWord( cursorIndex );

		if( selectionEnd == selectionStart )
		{
			selectionStart = cursorIndex;
			selectionEnd = nextWordIndex;
		}
		else if( selectionEnd == cursorIndex )
		{
			selectionEnd = nextWordIndex;
		}
		else if( selectionStart == cursorIndex )
		{
			selectionStart = nextWordIndex;
		}

		setCursorPos( nextWordIndex );

	}

	private void moveSelectionPointLeftWord()
	{

		if( cursorIndex == 0 )
			return;

		var prevWordIndex = findPreviousWord( cursorIndex );

		if( selectionEnd == selectionStart )
		{
			selectionEnd = cursorIndex;
			selectionStart = prevWordIndex;
		}
		else if( selectionEnd == cursorIndex )
		{
			selectionEnd = prevWordIndex;
		}
		else if( selectionStart == cursorIndex )
		{
			selectionStart = prevWordIndex;
		}

		setCursorPos( prevWordIndex );

	}

	private void moveSelectionPointRight()
	{

		if( cursorIndex == text.Length )
			return;

		if( selectionEnd == selectionStart )
		{
			selectionEnd = cursorIndex + 1;
			selectionStart = cursorIndex;
		}
		else if( selectionEnd == cursorIndex )
		{
			selectionEnd += 1;
		}
		else if( selectionStart == cursorIndex )
		{
			selectionStart += 1;
		}

		setCursorPos( cursorIndex + 1 );

	}

	private void moveSelectionPointLeft()
	{

		if( cursorIndex == 0 )
			return;

		if( selectionEnd == selectionStart )
		{
			selectionEnd = cursorIndex;
			selectionStart = cursorIndex - 1;
		}
		else if( selectionEnd == cursorIndex )
		{
			selectionEnd -= 1;
		}
		else if( selectionStart == cursorIndex )
		{
			selectionStart -= 1;
		}

		setCursorPos( cursorIndex - 1 );

	}

	private void moveToPreviousChar()
	{
		ClearSelection();
		setCursorPos( cursorIndex - 1 );
	}

	private void setCursorPos( int index )
	{

		index = Mathf.Max( 0, Mathf.Min( text.Length, index ) );
		if( index == cursorIndex )
			return;

		cursorIndex = index;
		cursorShown = HasFocus;

		scrollIndex = Mathf.Min( scrollIndex, cursorIndex );
		
		Invalidate();

	}

	private int findPreviousWord( int startIndex )
	{

		int index = startIndex;

		while( index > 0 )
		{

			var ch = text[ index - 1 ];

			if( char.IsWhiteSpace( ch ) || char.IsSeparator( ch ) || char.IsPunctuation( ch ) )
				index -= 1;
			else
				break;

		}

		for( int i = index; i >= 0; i-- )
		{

			if( i == 0 )
			{
				index = 0;
				break;
			}

			var ch = text[ i - 1 ];
			if( char.IsWhiteSpace( ch ) || char.IsSeparator( ch ) || char.IsPunctuation( ch ) )
			{
				index = i;
				break;
			}
		}

		return index;

	}

	private int findNextWord( int startIndex )
	{

		var textLength = text.Length;
		var index = startIndex;

		for( int i = index; i < textLength; i++ )
		{
			var ch = text[ i ];
			if( char.IsWhiteSpace( ch ) || char.IsSeparator( ch ) || char.IsPunctuation( ch ) )
			{
				index = i;
				break;
			}
		}

		while( index < textLength )
		{

			var ch = text[ index ];

			if( char.IsWhiteSpace( ch ) || char.IsSeparator( ch ) || char.IsPunctuation( ch ) )
				index += 1;
			else
				break;

		}

		return index;

	}

	protected override void OnRebuildRenderData()
	{

		if( Atlas == null || Font == null )
			return;

		renderData.Material = Atlas.Material;

		renderBackground();
		renderText();

	}

	public override void OnEnable()
	{

		if( padding == null )
			padding = new RectOffset();

		base.OnEnable();

		if( size.magnitude == 0 )
		{
			Size = new Vector2( 100, 20 );
		}

		cursorShown = false;
		cursorIndex = scrollIndex = 0;

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

	public override void Awake()
	{
		base.Awake();
		startSize = this.Size;
	}

#if ( UNITY_IPHONE || UNITY_ANDROID || UNITY_BLACKBERRY || UNITY_WP8 ) && !UNITY_EDITOR
	public override void Update()
	{

		base.Update();

		// This functionality cannot be used in the Editor, so just exit
		if( Application.isEditor )
			return;

		// Since this function is only concerned with the mobile keyboard, 
		// if this control does not have input focus, no further action is
		// necessary.
		if( this.HasFocus && mobileKeyboard != null )
		{
			if( mobileKeyboard.done )
			{

				ClearSelection();

				this.Text = mobileKeyboard.text;
				mobileKeyboard = null;

				OnSubmit();
				
			}
			else if( mobileKeyboard.wasCanceled )
			{
				mobileKeyboard = null;
				OnCancel();
			}
			else if( mobileHideInputField )
			{
				this.Text = mobileKeyboard.text;
				moveToEnd();
			}

		}

	}

	protected internal override void OnClick( dfMouseEventArgs args )
	{
		
		base.OnClick( args );

		// http://www.daikonforge.com/dfgui/forums/topic/variable-bug-with-mobile-keyboard/
		this.Focus();

		if( useMobileKeyboard && this.mobileKeyboardTrigger == dfMobileKeyboardTrigger.ShowOnClick )
		{
			ClearSelection();
			selectToEnd();
			TouchScreenKeyboard.hideInput = mobileHideInputField;
			mobileKeyboard = TouchScreenKeyboard.Open( this.text, (TouchScreenKeyboardType)mobileKeyboardType, mobileAutoCorrect, false, IsPasswordField );
		}

	}
#endif

	protected internal override void OnGotFocus( dfFocusEventArgs args )
	{

		base.OnGotFocus( args );

		this.undoText = this.Text;

		if( !ReadOnly )
		{

			whenGotFocus = Time.realtimeSinceStartup;
			StartCoroutine( doCursorBlink() );

			if( selectOnFocus )
			{
				selectionStart = 0;
				selectionEnd = text.Length;
			}
			else
			{
				selectionStart = selectionEnd = 0;
			}

#if ( UNITY_IPHONE || UNITY_ANDROID || UNITY_BLACKBERRY || UNITY_WP8 ) && !UNITY_EDITOR
			if( useMobileKeyboard && this.mobileKeyboardTrigger == dfMobileKeyboardTrigger.ShowOnFocus )
			{
				ClearSelection();
				selectToEnd();
				TouchScreenKeyboard.hideInput = mobileHideInputField;
				mobileKeyboard = TouchScreenKeyboard.Open( this.text, (TouchScreenKeyboardType)mobileKeyboardType, mobileAutoCorrect, false, IsPasswordField );
			}
#endif

		}

		Invalidate();

	}

	protected internal override void OnLostFocus( dfFocusEventArgs args )
	{

		base.OnLostFocus( args );

#if UNITY_IPHONE || UNITY_ANDROID || UNITY_BLACKBERRY || UNITY_WP8 || UNITY_EDITOR
		if( mobileKeyboard != null )
		{
			mobileKeyboard.active = false;
			mobileKeyboard = null;
		}
#endif

		cursorShown = false;
		ClearSelection();

		Invalidate();

		whenGotFocus = 0f;

	}

	protected internal override void OnDoubleClick( dfMouseEventArgs args )
	{

		if( !ReadOnly && HasFocus && args.Buttons.IsSet( dfMouseButtons.Left ) && ( Time.realtimeSinceStartup - whenGotFocus ) > 0.5f )
		{
			var index = getCharIndexOfMouse( args );
			selectWordAtIndex( index );
		}

		base.OnDoubleClick( args );

	}

	protected internal override void OnMouseDown( dfMouseEventArgs args )
	{

		if( !ReadOnly && HasFocus && args.Buttons.IsSet( dfMouseButtons.Left ) && ( Time.realtimeSinceStartup - whenGotFocus ) > 0.25f )
		{

			var index = getCharIndexOfMouse( args );
			if( index != cursorIndex )
			{
				cursorIndex = index;
				cursorShown = true;
				Invalidate();
				args.Use();
			}

			mouseSelectionAnchor = cursorIndex;
			selectionStart = selectionEnd = cursorIndex;

		}

		base.OnMouseDown( args );

	}

	protected internal override void OnMouseMove( dfMouseEventArgs args )
	{

		if( !ReadOnly && HasFocus && args.Buttons.IsSet( dfMouseButtons.Left ) )
		{

			var index = getCharIndexOfMouse( args );
			if( index != cursorIndex )
			{

				cursorIndex = index;
				cursorShown = true;
				Invalidate();
				args.Use();

				selectionStart = Mathf.Min( mouseSelectionAnchor, index );
				selectionEnd = Mathf.Max( mouseSelectionAnchor, index );

				return;

			}

		}

		base.OnMouseMove( args );

	}

	protected internal virtual void OnTextChanged()
	{

		SignalHierarchy( "OnTextChanged", this.text );

		if( TextChanged != null )
		{
			TextChanged( this, this.text );
		}

	}

	protected internal virtual void OnReadOnlyChanged()
	{

		//Signal( "OnReadOnlyChanged", this.readOnly );

		if( ReadOnlyChanged != null )
		{
			ReadOnlyChanged( this, this.readOnly );
		}

	}

	protected internal virtual void OnPasswordCharacterChanged()
	{

		//Signal( "OnPasswordCharacterChanged", this.passwordChar );

		if( PasswordCharacterChanged != null )
		{
			PasswordCharacterChanged( this, this.passwordChar );
		}

	}

	protected internal virtual void OnSubmit()
	{

		SignalHierarchy( "OnTextSubmitted", this, this.text );

		if( TextSubmitted != null )
		{
			TextSubmitted( this, this.text );
		}

	}

	protected internal virtual void OnCancel()
	{

		this.text = this.undoText;

		SignalHierarchy( "OnTextCancelled", this, this.text );

		if( TextCancelled != null )
		{
			TextCancelled( this, this.text );
		}

	}

	#endregion

	#region Public methods 

	/// <summary>
	/// Clears the text selection range
	/// </summary>
	public void ClearSelection()
	{
		selectionStart = 0;
		selectionEnd = 0;
		mouseSelectionAnchor = 0;
	}

	#endregion

	#region Private utility methods

	private IEnumerator doCursorBlink()
	{

		if( !Application.isPlaying )
			yield break;

		cursorShown = true;

		while( HasFocus )
		{
			yield return new WaitForSeconds( cursorBlinkTime );
			cursorShown = !cursorShown;
			Invalidate();
		}

		cursorShown = false;

	}

	private void renderText()
	{

		var p2u = PixelsToUnits();
		var maxSize = new Vector2( size.x - padding.horizontal, this.size.y - padding.vertical );

		var pivotOffset = pivot.TransformToUpperLeft( Size );
		var origin = new Vector3(
			pivotOffset.x + padding.left,
			pivotOffset.y - padding.top,
			0
		) * p2u;

		var displayText = IsPasswordField && !string.IsNullOrEmpty( this.passwordChar ) ? passwordDisplayText() : this.text;

		var renderColor = IsEnabled ? TextColor : DisabledColor;

		var scaleMultiplier = getTextScaleMultiplier();

		using( var textRenderer = font.ObtainRenderer() )
		{

			textRenderer.WordWrap = false;
			textRenderer.MaxSize = maxSize;
			textRenderer.PixelRatio = p2u;
			textRenderer.TextScale = TextScale * scaleMultiplier;
			textRenderer.VectorOffset = origin;
			textRenderer.MultiLine = false;
			textRenderer.TextAlign = TextAlignment.Left;
			textRenderer.ProcessMarkup = false;
			textRenderer.DefaultColor = renderColor;
			textRenderer.OverrideMarkupColors = true;
			textRenderer.Opacity = this.CalculateOpacity();
			textRenderer.Shadow = this.Shadow;
			textRenderer.ShadowColor = this.ShadowColor;
			textRenderer.ShadowOffset = this.ShadowOffset;

			#region Manage the scroll position - Keep cursor in view at all times 

			cursorIndex = Mathf.Min( cursorIndex, displayText.Length );
			scrollIndex = Mathf.Min( Mathf.Min( scrollIndex, cursorIndex ), displayText.Length );

			charWidths = textRenderer.GetCharacterWidths( displayText );
			var maxRenderSize = maxSize * p2u;

			leftOffset = 0f;
			if( textAlign == TextAlignment.Left )
			{

				// Measure everything from the current scroll position up to the cursor
				var renderedWidth = 0f;
				for( int i = scrollIndex; i < cursorIndex; i++ )
				{
					renderedWidth += charWidths[ i ];
				}

				// Make sure that the cursor can still be viewed
				while( renderedWidth >= maxRenderSize.x && scrollIndex < cursorIndex )
				{
					renderedWidth -= charWidths[ scrollIndex++ ];
				}

			}
			else
			{

				scrollIndex = Mathf.Max( 0, Mathf.Min( cursorIndex, displayText.Length - 1 ) );

				var renderedWidth = 0f;
				var slop = font.FontSize * 1.25f * p2u;
				while( scrollIndex > 0 && renderedWidth < maxRenderSize.x - slop )
				{
					renderedWidth += charWidths[ scrollIndex-- ];
				}

				var textSize = ( displayText.Length > 0 ) ? textRenderer.GetCharacterWidths( displayText.Substring( scrollIndex ) ).Sum() : 0;

				switch( textAlign )
				{
					case TextAlignment.Center:
						leftOffset = Mathf.Max( 0, ( maxRenderSize.x - textSize ) * 0.5f );
						break;
					case TextAlignment.Right:
						leftOffset = Mathf.Max( 0, maxRenderSize.x - textSize );
						break;
				}

				origin.x += leftOffset;
				textRenderer.VectorOffset = origin;

			}

			#endregion

			if( selectionEnd != selectionStart )
			{
				renderSelection( scrollIndex, charWidths, leftOffset );
			}

			textRenderer.Render( displayText.Substring( scrollIndex ), renderData );

			if( cursorShown && selectionEnd == selectionStart )
			{
				renderCursor( scrollIndex, cursorIndex, charWidths, leftOffset );
			}

		}

	}

	private float getTextScaleMultiplier()
	{

		if( textScaleMode == dfTextScaleMode.None || !Application.isPlaying )
			return 1f;

		// Return the difference between design resolution and current resolution
		if( textScaleMode == dfTextScaleMode.ScreenResolution )
		{
			return (float)Screen.height / (float)manager.FixedHeight;
		}

		// Return scale based on control size
		return Size.y / startSize.y;

	}

	private string passwordDisplayText()
	{
		return new string( this.passwordChar[ 0 ], this.text.Length );
	}

	private void renderSelection( int scrollIndex, float[] charWidths, float leftOffset )
	{

		// Cannot render the selection without a blank texture
		if( string.IsNullOrEmpty( SelectionSprite ) || Atlas == null )
			return;

		var p2u = PixelsToUnits();
		var maxSize = ( size.x - padding.horizontal ) * p2u;

		var lastVisibleIndex = scrollIndex;
		var renderWidth = 0f;
		
		for( int i = scrollIndex; i < text.Length; i++ )
		{

			lastVisibleIndex += 1;

			renderWidth += charWidths[ i ];
			if( renderWidth > maxSize )
				break;

		}

		if( selectionStart > lastVisibleIndex || selectionEnd < scrollIndex )
			return;

		var startIndex = Mathf.Max( scrollIndex, selectionStart );
		if( startIndex > lastVisibleIndex )
			return;

		var endIndex = Mathf.Min( selectionEnd, lastVisibleIndex );
		if( endIndex <= scrollIndex )
			return;

		var startX = 0f;
		var endX = 0f;
		renderWidth = 0f;

		for( int i = scrollIndex; i <= lastVisibleIndex; i++ )
		{

			if( i == startIndex )
			{
				startX = renderWidth;
			}
			
			if( i == endIndex )
			{
				endX = renderWidth;
				break;
			}

			renderWidth += charWidths[ i ];

		}

		var height = Size.y * p2u;

		addQuadIndices( renderData.Vertices, renderData.Triangles );

		var left = startX + leftOffset + ( padding.left * p2u );
		var right = left + Mathf.Min( ( endX - startX ), maxSize );
		var top = -( padding.top + 1 ) * p2u;
		var bottom = top - height + ( padding.vertical + 2 ) * p2u;

		var pivotOffset = pivot.TransformToUpperLeft( Size ) * p2u;
		var topLeft = new Vector3( left, top ) + pivotOffset;
		var topRight = new Vector3( right, top ) + pivotOffset;
		var bottomLeft = new Vector3( left, bottom ) + pivotOffset;
		var bottomRight = new Vector3( right, bottom ) + pivotOffset;

		renderData.Vertices.Add( topLeft );
		renderData.Vertices.Add( topRight );
		renderData.Vertices.Add( bottomRight );
		renderData.Vertices.Add( bottomLeft );

		var selectionColor = ApplyOpacity( this.SelectionBackgroundColor );
		renderData.Colors.Add( selectionColor );
		renderData.Colors.Add( selectionColor );
		renderData.Colors.Add( selectionColor );
		renderData.Colors.Add( selectionColor );

		var blankTexture = Atlas[ SelectionSprite ];
		var rect = blankTexture.region;
		var uvx = rect.width / blankTexture.sizeInPixels.x;
		var uvy = rect.height / blankTexture.sizeInPixels.y;
		renderData.UV.Add( new Vector2( rect.x + uvx, rect.yMax - uvy ) );
		renderData.UV.Add( new Vector2( rect.xMax - uvx, rect.yMax - uvy ) );
		renderData.UV.Add( new Vector2( rect.xMax - uvx, rect.y + uvy ) );
		renderData.UV.Add( new Vector2( rect.x + uvx, rect.y + uvy ) );

	}

	private void renderCursor( int startIndex, int cursorIndex, float[] charWidths, float leftOffset )
	{

		// Cannot render the cursor without a blank texture
		if( string.IsNullOrEmpty( SelectionSprite ) || Atlas == null )
			return;

		var cursorPos = 0f;
		for( int i = startIndex; i < cursorIndex; i++ )
		{
			cursorPos += charWidths[ i ];
		}

		var designedPixelSize = PixelsToUnits();
		var xofs = ( cursorPos + leftOffset + padding.left * designedPixelSize ).Quantize( designedPixelSize );
		var yofs = -padding.top * designedPixelSize;
		var width = designedPixelSize * cursorWidth;
		var height = ( size.y - padding.vertical ) * designedPixelSize;

		var v0 = new Vector3( xofs, yofs );
		var v1 = new Vector3( xofs + width, yofs );
		var v2 = new Vector3( xofs + width, yofs - height );
		var v3 = new Vector3( xofs, yofs - height );

		var verts = renderData.Vertices;
		var triangles = renderData.Triangles;
		var uvs = renderData.UV;
		var colors = renderData.Colors;

		var pivotOffset = pivot.TransformToUpperLeft( size ) * designedPixelSize;
		addQuadIndices( verts, triangles );
		verts.Add( v0 + pivotOffset );
		verts.Add( v1 + pivotOffset );
		verts.Add( v2 + pivotOffset );
		verts.Add( v3 + pivotOffset );

		var glyphColor = ApplyOpacity( TextColor );
		colors.Add( glyphColor );
		colors.Add( glyphColor );
		colors.Add( glyphColor );
		colors.Add( glyphColor );

		var blankTexture = Atlas[ SelectionSprite ];
		var rect = blankTexture.region;
		uvs.Add( new Vector2( rect.x, rect.yMax ) );
		uvs.Add( new Vector2( rect.xMax, rect.yMax ) );
		uvs.Add( new Vector2( rect.xMax, rect.y ) );
		uvs.Add( new Vector2( rect.x, rect.y ) );

	}

	private void addQuadIndices( dfList<Vector3> verts, dfList<int> triangles )
	{

		var vcount = verts.Count;
		var indices = new int[] { 0, 1, 3, 3, 1, 2 };

		for( int ii = 0; ii < indices.Length; ii++ )
		{
			triangles.Add( vcount + indices[ ii ] );
		}

	}

	private int getCharIndexOfMouse( dfMouseEventArgs args )
	{

		var mousePos = GetHitPosition( args );

		var p2u = PixelsToUnits();
		var index = scrollIndex;
		var accum = leftOffset / p2u;
		for( int i = scrollIndex; i < charWidths.Length; i++ )
		{
			accum += charWidths[ i ] / p2u;
			if( accum < mousePos.x )
				index++;
		}

		return index;

	}

	#endregion

}
