@CustomEditor (SubLightning)
class subLightningEditor extends Editor {
    function OnInspectorGUI (){
    	target.color = EditorGUILayout.ColorField("Color", target.color);
    	EditorGUILayout.LabelField("Width", " ");
        target.width = EditorGUILayout.Slider(target.width,0.05, 9);
        EditorGUILayout.LabelField("Spread", " ");
        target.jump = EditorGUILayout.Slider(target.jump,0.1, 4.0);
		EditorGUILayout.LabelField("Distance", " ");
		target.dis = EditorGUILayout.IntSlider(target.dis,1, 500);
        target.pointDown = EditorGUILayout.Toggle("Point down", target.pointDown);
        target.glow = EditorGUILayout.Toggle("Glow?", target.glow);
        if (GUI.changed)
            EditorUtility.SetDirty (target);
    }
}