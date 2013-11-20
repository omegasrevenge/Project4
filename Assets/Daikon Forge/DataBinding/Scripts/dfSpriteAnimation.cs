using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

/// <summary>
/// Allows the animation of any component which renders sprites,
/// such as dfSprite, dfTextureSprite, dfButton, dfPanel, etc.
/// </summary>
[Serializable]
[RequireComponent( typeof( BoxCollider ) )]
[AddComponentMenu( "Daikon Forge/Tweens/Sprite Animator" )]
public class dfSpriteAnimation : dfTweenPlayableBase
{

	#region Public enumerations 

	/// <summary>
	/// Indicates the direction that the animation should play in 
	/// </summary>
	public enum PlayDirection : int
	{
		Forward = 0,
		Reverse = 1
	}

	#endregion

	#region Events

#pragma warning disable 0067

	/// <summary>
	/// Raised when the tween animation has started playing 
	/// </summary>
	public event TweenNotification AnimationStarted;

	/// <summary>
	/// Raised when the tween animation has stopped playing before completion
	/// </summary>
	public event TweenNotification AnimationStopped;

	/// <summary>
	/// Raised when the tween animation has been paused
	/// </summary>
	public event TweenNotification AnimationPaused;

	/// <summary>
	/// Raised when the tween animation has been resumed after having been paused
	/// </summary>
	public event TweenNotification AnimationResumed;

	/// <summary>
	/// Raised when the tween animation has been reset
	/// </summary>
	public event TweenNotification AnimationReset;

	/// <summary>
	/// Raised when the tween animation has successfully completed
	/// </summary>
	public event TweenNotification AnimationCompleted;

#pragma warning restore 0067

	#endregion

	#region Private serialized fields 

	[SerializeField]
	private string animationName = "ANIMATION";

	[SerializeField]
	private dfAnimationClip clip;

	[SerializeField]
	private dfComponentMemberInfo memberInfo = new dfComponentMemberInfo(); 

	[SerializeField]
	private dfTweenLoopType loopType = dfTweenLoopType.Loop;

	[SerializeField]
	private float length = 1f;

	[SerializeField]
	private bool autoStart = false;

	[SerializeField]
	private bool skipToEndOnStop = false;

	[SerializeField]
	private PlayDirection playDirection = PlayDirection.Forward;

	#endregion

	#region Private runtime variables 

	private bool autoRunStarted = false;
	private bool isRunning = false;
	private dfObservableProperty target = null;

	#endregion

	#region Public properties

	public dfAnimationClip Clip
	{
		get { return this.clip; }
		set
		{
			this.clip = value;
		}
	}

	public dfComponentMemberInfo Target
	{
		get { return this.memberInfo; }
		set
		{
			this.memberInfo = value;
		}
	}

	public bool AutoRun
	{
		get { return this.autoStart; }
		set { this.autoStart = value; }
	}

	public float Length
	{
		get { return this.length; }
		set { this.length = Mathf.Max( value, 0.03f ); }
	}

	public dfTweenLoopType LoopType
	{
		get { return this.loopType; }
		set { this.loopType = value; }
	}

	public PlayDirection Direction
	{
		get { return this.playDirection; }
		set { this.playDirection = value; if( this.IsPlaying ) this.Play(); }
	}

	#endregion

	#region Unity events 
 
	public void Awake() { }
	public void Start() { }

	public void LateUpdate()
	{

		if( this.AutoRun && !this.IsPlaying && !this.autoRunStarted )
		{
			this.autoRunStarted = true;
			this.Play();
		}

	}

	#endregion

	#region Public methods 

	/// <summary>
	/// Event-bindable wrapper around Direction and Play members to 
	/// start playing the animation in the forward direction
	/// </summary>
	public void PlayForward()
	{
		this.playDirection = PlayDirection.Forward;
		this.Play();
	}

	/// <summary>
	/// Event-bindable wrapper around Direction and Play members to 
	/// start playing the animation in the reverse direction
	/// </summary>
	public void PlayReverse()
	{
		this.playDirection = PlayDirection.Reverse;
		this.Play();
	}

	#endregion

	#region dfTweenPlayableBase implementation

	public override bool IsPlaying
	{
		get { return this.isRunning; }
	}

	public override void Play()
	{

		if( this.IsPlaying )
		{
			this.Stop();
		}

		if( !enabled || !gameObject.activeSelf || !gameObject.activeInHierarchy )
			return;

		if( this.memberInfo == null )
			throw new NullReferenceException( "Animation target is NULL" );

		if( !this.memberInfo.IsValid )
			throw new InvalidOperationException( "Invalid property binding configuration on " + getPath( gameObject.transform ) + " - " + target );

		this.target = this.memberInfo.GetProperty();
		StartCoroutine( Execute() );

	}

	public override void Reset()
	{

		var sprites = ( clip != null ) ? clip.Sprites : null;
		if( memberInfo.IsValid && sprites != null && sprites.Count > 0 )
		{
			memberInfo.Component.SetProperty( memberInfo.MemberName, sprites[ 0 ] );
		}

		if( !isRunning )
			return;

		StopAllCoroutines();
		isRunning = false;

		onReset();

		this.target = null;

	}

	public override void Stop()
	{

		if( !isRunning )
			return;

		var sprites = ( clip != null ) ? clip.Sprites : null;
		if( skipToEndOnStop && sprites != null )
		{
			setFrame( Mathf.Max( sprites.Count - 1, 0 ) );
		}

		StopAllCoroutines();
		isRunning = false;

		onStopped();

		this.target = null;

	}

	public override string TweenName
	{
		get { return this.animationName; }
		set { this.animationName = value; }
	}

	#endregion

	#region Event signalers

	protected void onPaused()
	{
		SendMessage( "AnimationPaused", this, SendMessageOptions.DontRequireReceiver );
		if( AnimationPaused != null ) AnimationPaused();
	}

	protected void onResumed()
	{
		SendMessage( "AnimationResumed", this, SendMessageOptions.DontRequireReceiver );
		if( AnimationResumed != null ) AnimationResumed();
	}

	protected void onStarted()
	{
		SendMessage( "AnimationStarted", this, SendMessageOptions.DontRequireReceiver );
		if( AnimationStarted != null ) AnimationStarted();
	}

	protected void onStopped()
	{
		SendMessage( "AnimationStopped", this, SendMessageOptions.DontRequireReceiver );
		if( AnimationStopped != null ) AnimationStopped();
	}

	protected void onReset()
	{
		SendMessage( "AnimationReset", this, SendMessageOptions.DontRequireReceiver );
		if( AnimationReset != null ) AnimationReset();
	}

	protected void onCompleted()
	{
		SendMessage( "AnimationCompleted", this, SendMessageOptions.DontRequireReceiver );
		if( AnimationCompleted != null ) AnimationCompleted();
	}

	#endregion

	#region Private utility methods 

	private IEnumerator Execute()
	{

		if( clip == null || clip.Sprites == null || clip.Sprites.Count == 0 )
			yield break;


		this.isRunning = true;

		var startTime = Time.realtimeSinceStartup;
		var direction = ( this.playDirection == PlayDirection.Forward ) ? 1 : -1;
		var lastFrameIndex = (direction == 1) ? 0 : clip.Sprites.Count - 1;

		setFrame( lastFrameIndex );

		while( true )
		{

			yield return null;

			// Rereference these values each frame in case base AnimationClip
			// has changed (should probably on happen at design time in editor)
			var sprites = clip.Sprites;
			var maxFrameIndex = sprites.Count - 1;

			// Calculate the amount of time that has passed since the animation
			// started, looped, or reversed
			var timeNow = Time.realtimeSinceStartup;
			var elapsed = timeNow - startTime;

			// Determine the index of the current animation frame
			var frameIndex = Mathf.RoundToInt( Mathf.Clamp01( elapsed / this.length ) * maxFrameIndex );

			// Determine what to do if the animation has reached the 
			// last frame.
			if( elapsed >= this.length )
			{

				switch( this.loopType )
				{
					case dfTweenLoopType.Once:
						yield break;
					case dfTweenLoopType.Loop:
						startTime = timeNow;
						frameIndex = 0;
						break;
					case dfTweenLoopType.PingPong:
						startTime = timeNow;
						direction *= -1;
						frameIndex = 0;
						break;
				}

			}

			if( direction == -1 )
			{
				frameIndex = maxFrameIndex - frameIndex;
			}
			
			// Set the current animation frame on the sprite
			if( lastFrameIndex != frameIndex )
			{
				lastFrameIndex = frameIndex;
				setFrame( frameIndex );
			}

		}

	}

	private string getPath( Transform obj )
	{

		System.Text.StringBuilder path = new System.Text.StringBuilder();

		while( obj != null )
		{
			if( path.Length > 0 )
			{
				path.Insert( 0, "\\" );
				path.Insert( 0, obj.name );
			}
			else
			{
				path.Append( obj.name );
			}
			obj = obj.parent;
		}

		return path.ToString();

	}

	private void setFrame( int frameIndex )
	{

		var sprites = clip.Sprites;
		if( sprites.Count == 0 )
			return;

		// Clamp the frame index
		frameIndex = Mathf.Max( 0, Mathf.Min( frameIndex, sprites.Count - 1 ) );

		if( this.target != null )
		{
			// Sprites and other DFGUI controls will re-render themselves when
			// the property associated with their background image is changed.
			this.target.Value = sprites[ frameIndex ];
		}

	}

	#endregion

}
