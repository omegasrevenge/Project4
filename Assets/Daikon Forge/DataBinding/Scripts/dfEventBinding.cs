/* Copyright 2013 Daikon Forge */

using UnityEngine;

using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Provides Editor support for binding any event on a source Component
/// to a compatible event handler on a target Component
/// </summary>
[AddComponentMenu( "Daikon Forge/Data Binding/Event Binding" )]
[Serializable]
public class dfEventBinding : MonoBehaviour, IDataBindingComponent
{

	#region Public fields

	/// <summary>
	/// Specifies which event on the source component to bind to
	/// </summary>
	public dfComponentMemberInfo DataSource;

	/// <summary>
	/// Specifies which method on the target component to invoke when 
	/// the source event is triggered
	/// </summary>
	public dfComponentMemberInfo DataTarget;

	#endregion

	#region Private fields

	private bool isBound = false;

	private Component sourceComponent;
	private Component targetComponent;

	private FieldInfo eventField;
	private Delegate eventDelegate;

	#endregion

	#region Unity events

	public void OnEnable()
	{
		if( !isBound && DataSource.IsValid && DataTarget.IsValid )
		{
			Bind();
		}
	}

	public void Start()
	{
		if( !isBound && DataSource.IsValid && DataTarget.IsValid )
		{
			Bind();
		}
	}

	public void OnDisable()
	{
		Unbind();
	}

	#endregion

	#region Public methods 

	/// <summary>
	/// Bind the source event to the target event handler
	/// </summary>
	public void Bind()
	{

		if( isBound )
			return;

		if( !DataSource.IsValid || !DataTarget.IsValid )
		{
			Debug.LogError( string.Format( "Invalid event binding configuration - Source:{0}, Target:{1}", DataSource, DataTarget ) );
			return;
		}

		sourceComponent = DataSource.Component;
		targetComponent = DataTarget.Component;

		var eventHandler = DataTarget.GetMethod();
		if( eventHandler == null )
		{
			Debug.LogError( "Event handler not found: " + targetComponent.GetType().Name + "." + DataTarget.MemberName );
			return;
		}

		eventField = getField( sourceComponent, DataSource.MemberName );
		if( eventField == null )
		{
			Debug.LogError( "Event definition not found: " + sourceComponent.GetType().Name + "." + DataSource.MemberName );
			return;
		}

		try
		{

			var invokeMethod = eventField.FieldType.GetMethod( "Invoke" );
			var invokeParams = invokeMethod.GetParameters();
			var handlerParams = eventHandler.GetParameters();

			if( invokeParams.Length == handlerParams.Length )
			{
				eventDelegate = Delegate.CreateDelegate( eventField.FieldType, targetComponent, eventHandler, true );
			}
			else if( invokeParams.Length > 0 && handlerParams.Length == 0 )
			{
#if !UNITY_IPHONE
				eventDelegate = createDynamicWrapper( targetComponent, eventField.FieldType, invokeParams, eventHandler );
#else	
				var message = string.Format( 
					"Dynamic code generation is not supported on the target platform, the {0}.{1} method must exactly match the event signature for {2}.{3}", 
					DataTarget.Component.GetType().Name, 
					DataTarget.MemberName, 
					DataSource.Component.GetType().Name, 
					DataSource.MemberName 
					);

				throw new InvalidOperationException( message );
#endif
			}
			else
			{
				throw new InvalidCastException( "Event signature mismatch: " + eventHandler );
			}

		}
		catch( Exception err )
		{
			Debug.LogError( "Event binding failed - Failed to create event handler: " + err.ToString() );
			return;
		}

		var combinedDelegate = Delegate.Combine( eventDelegate, (Delegate)eventField.GetValue( sourceComponent ) );
		eventField.SetValue( sourceComponent, combinedDelegate );

		isBound = true;

	}

	/// <summary>
	/// Unbind the source event and target event handler
	/// </summary>
	public void Unbind()
	{

		if( !isBound )
			return;

		isBound = false;

		var currentDelegate = (Delegate)eventField.GetValue( sourceComponent );
		var newDelegate = Delegate.Remove( currentDelegate, eventDelegate );
		eventField.SetValue( sourceComponent, newDelegate );

		eventField = null;
		eventDelegate = null;

		sourceComponent = null;
		targetComponent = null;

	}

	#endregion

	#region System.Object overrides 

	/// <summary>
	/// Returns a formatted string summarizing this object's state
	/// </summary>
	public override string ToString()
	{

		string sourceType = DataSource != null && DataSource.Component != null ? DataSource.Component.GetType().Name : "[null]";
		string sourceMember = DataSource != null && !string.IsNullOrEmpty( DataSource.MemberName ) ? DataSource.MemberName : "[null]";

		string targetType = DataTarget != null && DataTarget.Component != null ? DataTarget.Component.GetType().Name : "[null]";
		string targetMember = DataTarget != null && !string.IsNullOrEmpty( DataTarget.MemberName ) ? DataTarget.MemberName : "[null]";

		return string.Format( "Bind {0}.{1} -> {2}.{3}", sourceType, sourceMember, targetType, targetMember );

	}

	#endregion

	#region Private utility methods

	private FieldInfo getField( Component sourceComponent, string fieldName )
	{

		return
			sourceComponent.GetType()
			.GetAllFields()
			.Where( f => f.Name == fieldName )
			.FirstOrDefault();

	}

	/// <summary>
	/// Creates a Delegate wrapper that allows a parameterless method to be used as 
	/// an event handler for an event that defines parameters. This enables the use
	/// of "notification" event handlers - Methods which either cannot make use of
	/// or don't care about event parameters. 
	/// </summary>
	private Delegate createDynamicWrapper( object target, Type delegateType, ParameterInfo[] eventParams, MethodInfo eventHandler )
	{

#if UNITY_IPHONE
		throw new InvalidOperationException( "Dynamic code generation is not supported on the target platform" );
#else
		var paramTypes =
			new Type[] { target.GetType() }
			.Concat( eventParams.Select( p => p.ParameterType ) )
			.ToArray();

		var handler = new DynamicMethod(
			"DynamicEventWrapper_" + eventHandler.Name,
			typeof( void ),
			paramTypes
			);

		var il = handler.GetILGenerator();

		il.Emit( OpCodes.Ldarg_0 );

		// Changed from il.EmitCall() to il.Emit() for WP8 compatibility
		il.Emit( OpCodes.Callvirt, eventHandler );

		il.Emit( OpCodes.Ret );

		return handler.CreateDelegate( delegateType, target );

#endif

	}

	#endregion

}
