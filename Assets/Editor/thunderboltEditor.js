@CustomEditor (Thunderbolt)
class thunderboltEditor extends Editor {
    function OnInspectorGUI () {
    	if(GUILayout.Button("Reset")){
    		target.color = Color(255,255,255,10);
    		target.width = 6.0;
    		target.jump = 5.0;
    		target.sparks = true;
    		target.spread = 0.1;
    		target.createLight = true;
    		target.maxDistance = 120;
    		target.timeOut = 0.5;
    		target.glow = true;
    	}
    	
    	target.color = EditorGUILayout.ColorField("Color", target.color);
    	EditorGUILayout.LabelField("Width", " ");
        target.width = EditorGUILayout.Slider(target.width,0.1, 10.0);
        EditorGUILayout.LabelField("Spread", " ");
        target.jump = EditorGUILayout.Slider(target.jump,0.1, 10.0);
        target.sparks = EditorGUILayout.Toggle("Sparks?", target.sparks);
        if(target.sparks == true){
        	EditorGUILayout.LabelField("Sparkyness", " ");
			target.spread = EditorGUILayout.Slider(target.spread,0.001, 0.25);
		}
		
		EditorGUILayout.LabelField("Max distance", " ");
		target.maxDistance = EditorGUILayout.IntSlider(target.maxDistance,25, 500);
		EditorGUILayout.LabelField("Bolt stay time", " ");
        target.timeOut = EditorGUILayout.Slider(target.timeOut,0.05, 10.0);
        target.glow = EditorGUILayout.Toggle("Glow?", target.glow);
        target.createLight = EditorGUILayout.Toggle("Create light?", target.createLight);
        
        if (GUI.changed)
            EditorUtility.SetDirty (target);
    }
}