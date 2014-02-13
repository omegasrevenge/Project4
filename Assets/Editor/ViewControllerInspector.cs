using UnityEditor;

[CustomEditor(typeof(ViewController))]
public class ViewControllerInspector : Editor {
    public override void OnInspectorGUI()
    {
        ViewController viewController = (ViewController)target;
        viewController.ViewportScrollState = EditorGUILayout.Slider("Menu",viewController.ViewportScrollState, 0f, 1f);
    }
}
