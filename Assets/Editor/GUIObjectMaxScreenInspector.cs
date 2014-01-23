using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(GUIObjectMaxScreen))]
public class GUIObjectMaxScreenInspector : Editor
{

    public override void OnInspectorGUI()
    {
        GUIObjectMaxScreen maxScreen = (GUIObjectMaxScreen)target;
        maxScreen.Title = EditorGUILayout.TextField("Textkey Title", maxScreen.Title);
        maxScreen.Text = EditorGUILayout.TextField("Textkey Text", maxScreen.Text);


    }

}
