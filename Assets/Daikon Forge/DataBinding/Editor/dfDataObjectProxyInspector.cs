/* Copyright 2013 Daikon Forge */
using UnityEngine;
using UnityEditor;

using System;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

[CustomEditor( typeof( dfDataObjectProxy ) )]
public class dfDataObjectProxyInspector : Editor
{

	public override void OnInspectorGUI()
	{

		dfEditorUtil.ComponentCopyButton( target );

		var proxy = target as dfDataObjectProxy;

		var assignedScript = getMatchingScript( proxy.TypeName );
		var selectedScript = EditorGUILayout.ObjectField( "Data Type", assignedScript, typeof( MonoScript ), false ) as MonoScript;
		if( selectedScript != assignedScript )
		{
			dfEditorUtil.MarkUndo( proxy, "Change Proxy Data Type" );
			proxy.TypeName = selectedScript.GetClass().Name;
		}

		if( Application.isPlaying || proxy.Data == null )
			return;

		var serialized = new SerializedObject( target );
		var property = serialized.FindProperty( "data" );
		if( property == null )
			return;

		EditorGUIUtility.LookLikeControls( 100f );
		EditorGUI.indentLevel += 1;

		GUILayout.Label( "Data", "HeaderLabel" );
		{
			EditorGUILayout.PropertyField( property, true );
		}

	}

	private MonoScript getMatchingScript( string targetType )
	{

		if( targetType == null )
			return null;

		MonoScript[] scripts = (MonoScript[])Resources.FindObjectsOfTypeAll( typeof( MonoScript ) );
		for( int i = 0; i < scripts.Length; i++ )
		{

			var scriptClass = scripts[ i ].GetClass();
			if( scriptClass == null )
				continue;

			if( scriptClass.Name == targetType )
			{
				return scripts[ i ];
			}

		}

		return null;

	}

}
