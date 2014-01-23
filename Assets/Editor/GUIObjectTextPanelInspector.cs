using UnityEditor;

[CustomEditor(typeof(GUIObjectTextPanel))]
public class GUIObjectTextPanelInspector : Editor
{

    public override void OnInspectorGUI()
    {
        GUIObjectTextPanel maxScreen = (GUIObjectTextPanel)target;
        maxScreen.Title = EditorGUILayout.TextField("Textkey Title", maxScreen.Title);
        maxScreen.Text = EditorGUILayout.TextField("Textkey Text", maxScreen.Text);


    }

}
