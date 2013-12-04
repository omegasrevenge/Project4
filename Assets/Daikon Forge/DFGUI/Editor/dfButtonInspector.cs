﻿/* Copyright 2013 Daikon Forge */
using UnityEngine;
using UnityEditor;

using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

[CanEditMultipleObjects]
[CustomEditor( typeof( dfButton ), true )]
public class dfButtonInspector : dfControlInspector
{

	private static Dictionary<int, bool> foldouts = new Dictionary<int, bool>();

	protected override bool OnCustomInspector()
	{

		var control = target as dfButton;
		if( control == null )
			return false;

		dfEditorUtil.DrawSeparator();

		if( !isFoldoutExpanded( foldouts, "Button Properties", true ) )
			return false;

		EditorGUIUtility.LookLikeControls( 100f );
		EditorGUI.indentLevel += 1;

		GUILayout.Label( "Data", "HeaderLabel" );
		{

			var text = EditorGUILayout.TextField( "Text", control.Text );
			if( text != control.Text )
			{
				dfEditorUtil.MarkUndo( control, "Change Text" );
				control.Text = text;
			}

		}

		GUILayout.Label( "Behavior", "HeaderLabel" );
		{

			var autoSize = EditorGUILayout.Toggle( "Auto size", control.AutoSize );
			if( autoSize != control.AutoSize )
			{

				dfEditorUtil.MarkUndo( control, "Change Auto-size property" );
				control.AutoSize = autoSize;

				var setDefaultPadding =
					autoSize &&
					control.Padding.horizontal == 0 &&
					control.Padding.vertical == 0 &&
					control.Atlas != null &&
					!string.IsNullOrEmpty( control.BackgroundSprite );

				if( setDefaultPadding )
				{
					var sprite = control.Atlas[ control.BackgroundSprite ];
					control.Padding = new RectOffset
					(
						sprite.border.left,
						sprite.border.right,
						sprite.border.top,
						sprite.border.bottom
					);
				}

			}

			var group = EditorGUILayout.ObjectField( "Group", control.ButtonGroup, typeof( dfControl ), true ) as dfControl;
			if( group != control.ButtonGroup )
			{
				dfEditorUtil.MarkUndo( control, "Assign Button Group" );
				control.ButtonGroup = group;
			}

		}

		GUILayout.Label( "Images", "HeaderLabel" );
		{

			SelectTextureAtlas( "Atlas", control, "Atlas", false, true );
			if( control.GUIManager != null && !dfAtlas.Equals( control.Atlas, control.GUIManager.DefaultAtlas ) )
			{
				EditorGUILayout.HelpBox( "This control does not use the same Texture Atlas as the View, which will result in an additional draw call.", MessageType.Info );
			}

			var buttonState = (dfButton.ButtonState)EditorGUILayout.EnumPopup( "Button State", control.State );
			if( buttonState != control.State )
			{
				dfEditorUtil.MarkUndo( control, "Change Button State" );
				control.State = buttonState;
			}

			SelectSprite( "Normal", control.Atlas, control, "BackgroundSprite" );
			SelectSprite( "Focus", control.Atlas, control, "FocusSprite", false );
			SelectSprite( "Hover", control.Atlas, control, "HoverSprite", false );
			SelectSprite( "Pressed", control.Atlas, control, "PressedSprite", false );
			SelectSprite( "Disabled", control.Atlas, control, "DisabledSprite", false );

		}

		GUILayout.Label( "Text Colors", "HeaderLabel" );
		{

			var textColor = EditorGUILayout.ColorField( "Normal", control.TextColor );
			if( textColor != control.TextColor )
			{
				dfEditorUtil.MarkUndo( control, "Change Text Color" );
				control.TextColor = textColor;
			}

			textColor = EditorGUILayout.ColorField( "Hover", control.HoverTextColor );
			if( textColor != control.HoverTextColor )
			{
				dfEditorUtil.MarkUndo( control, "Change Text Color" );
				control.HoverTextColor = textColor;
			}

			textColor = EditorGUILayout.ColorField( "Pressed", control.PressedTextColor );
			if( textColor != control.PressedTextColor )
			{
				dfEditorUtil.MarkUndo( control, "Change Text Color" );
				control.PressedTextColor = textColor;
			}

			textColor = EditorGUILayout.ColorField( "Focused", control.FocusTextColor );
			if( textColor != control.FocusTextColor )
			{
				dfEditorUtil.MarkUndo( control, "Change Text Color" );
				control.FocusTextColor = textColor;
			}

			textColor = EditorGUILayout.ColorField( "Disabled", control.DisabledTextColor );
			if( textColor != control.DisabledTextColor )
			{
				dfEditorUtil.MarkUndo( control, "Change Text Color" );
				control.DisabledTextColor = textColor;
			}

		}

		GUILayout.Label( "Text Appearance", "HeaderLabel" );
		{

			SelectFontDefinition( "Font", control.Atlas, control, "Font", true );

			if( control.Font == null )
				return false;

			var align = (TextAlignment)EditorGUILayout.EnumPopup( "Text Align", control.TextAlignment );
			if( align != control.TextAlignment )
			{
				dfEditorUtil.MarkUndo( control, "Change control Text Alignment" );
				control.TextAlignment = align;
			}

			var vertAlign = (dfVerticalAlignment)EditorGUILayout.EnumPopup( "Vert Align", control.VerticalAlignment );
			if( vertAlign != control.VerticalAlignment )
			{
				dfEditorUtil.MarkUndo( control, "Change Vertical Alignment" );
				control.VerticalAlignment = vertAlign;
			}

			var textScale = EditorGUILayout.FloatField( "Text Scale", control.TextScale );
			if( textScale != control.TextScale )
			{
				dfEditorUtil.MarkUndo( control, "Change Text Scale" );
				control.TextScale = textScale;
			}

			var scaleMode = (dfTextScaleMode)EditorGUILayout.EnumPopup( "Auto Scale", control.TextScaleMode );
			if( scaleMode != control.TextScaleMode )
			{
				dfEditorUtil.MarkUndo( control, "Change Text Scale Mode" );
				control.TextScaleMode = scaleMode;
			}

			var wordwrap = EditorGUILayout.Toggle( "Word Wrap", control.WordWrap );
			if( wordwrap != control.WordWrap )
			{
				dfEditorUtil.MarkUndo( control, "Toggle Word Wrap" );
				control.WordWrap = wordwrap;
			}

			var padding = EditPadding( "Padding", control.Padding );
			if( padding != control.Padding )
			{
				dfEditorUtil.MarkUndo( control, "Change Textbox Padding" );
				control.Padding = padding;
			}

			var shadow = EditorGUILayout.Toggle( "Shadow Effect", control.Shadow );
			if( shadow != control.Shadow )
			{
				dfEditorUtil.MarkUndo( control, "Change Shadow Effect" );
				control.Shadow = shadow;
			}

			if( shadow )
			{

				var shadowColor = EditorGUILayout.ColorField( "Shadow Color", control.ShadowColor );
				if( shadowColor != control.ShadowColor )
				{
					dfEditorUtil.MarkUndo( control, "Change Shadow Color" );
					control.ShadowColor = shadowColor;
				}

				var shadowOffset = EditInt2( "Shadow Offset", "X", "Y", control.ShadowOffset );
				if( shadowOffset != control.ShadowOffset )
				{
					dfEditorUtil.MarkUndo( control, "Change Shadow Color" );
					control.ShadowOffset = shadowOffset;
				}

				EditorGUIUtility.LookLikeControls( 120f );

			}

		}

		return true;

	}

	protected override bool OnControlDoubleClick( dfControl control, Event evt )
	{

		// HACK: Horribly hacky way to add better workflow for Tab controls
		// Double-click will select the tab corresponding to the double-clicked
		// button and make it visible so that the user can edit the tab page.
		if( control.Parent is dfTabstrip )
		{

			var tabStrip = control.Parent as dfTabstrip;
			tabStrip.SelectedIndex = control.ZOrder;

			SceneView.lastActiveSceneView.Repaint();
			dfGUIManager.RefreshAll();

			return true;

		}

		return base.OnControlDoubleClick( control, evt );
		
	}

}

