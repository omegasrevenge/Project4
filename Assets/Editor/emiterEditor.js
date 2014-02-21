@CustomEditor (Emitter)
class emiterEditor extends Editor {
    function OnInspectorGUI () {
    	EditorGUILayout.LabelField("Rate", " ");
        target.rate = EditorGUILayout.Slider(target.rate,0.05, 10);
        EditorGUILayout.LabelField("Spread", " ");
        target.spread = EditorGUILayout.Slider(target.spread,0.001, 1);
        target.runWhileOutOfSight = EditorGUILayout.Toggle("Run out of sight", target.runWhileOutOfSight);
    	
    	EditorGUILayout.LabelField("", " ");
    	EditorGUILayout.LabelField("", " ");
    	EditorGUILayout.LabelField("Spark variables", " ");
    	target.color = EditorGUILayout.ColorField("Color", target.color);
    	EditorGUILayout.LabelField("Width", " ");
        target.width = EditorGUILayout.Slider(target.width,0.05, 9);
        EditorGUILayout.LabelField("Spread", " ");
        target.jump = EditorGUILayout.Slider(target.jump,0.1, 4.0);
		EditorGUILayout.LabelField("Distance", " ");
		target.dis = EditorGUILayout.IntSlider(target.dis,1, 500);
        if (GUI.changed)
            EditorUtility.SetDirty (target);
    }
}