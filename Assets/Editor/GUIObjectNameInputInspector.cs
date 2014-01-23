using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(GUIObjectNameInput))]
public class GUIObjectNameInputInspector : Editor
{

    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();
        GUIObjectNameInput maxScreen = (GUIObjectNameInput)target;
        maxScreen.Button = EditorGUILayout.TextField("Textkey Button", maxScreen.Button);
        maxScreen.Default = EditorGUILayout.TextField("Textkey Default", maxScreen.Default);
        maxScreen.Text = EditorGUILayout.TextField("Text", maxScreen.Text);


    }

}
