@CustomEditor (BendyLightning)
class bendyLightningEditor extends Editor {
    function OnInspectorGUI () {
    	target.color = EditorGUILayout.ColorField("Color", target.color);
    	EditorGUILayout.LabelField("Width", " ");
        target.width = EditorGUILayout.Slider(target.width,0.05, 9);
        EditorGUILayout.LabelField("Spread", " ");
        target.jump = EditorGUILayout.Slider(target.jump,0.1, 4.0);
		EditorGUILayout.LabelField("Distance", " ");
		target.dis = EditorGUILayout.IntSlider(target.dis,1, 10);
        target.pointDown = EditorGUILayout.Toggle("Point down", target.pointDown);
        if (GUI.changed)
            EditorUtility.SetDirty (target);
    }
}