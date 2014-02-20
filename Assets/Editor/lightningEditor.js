@CustomEditor (Lightning)
class lightningEditor extends Editor {
    function OnInspectorGUI () {
    	if(GUILayout.Button("Reset")){
    		target.color = Color.white;
    		target.width = 1.5;
    		target.speed = 30.0;
    		target.jump = 4.0;
    		target.updateDistance = false;
    		target.maxDistance = 100;
    		target.dis = 20;
    		target.sparks = false;
    		target.spread = 0.001;
    		target.clampStart = true;
	 		target.clampEnd = true;
	 		target.glow = false;
	 		target.createLight = false;
	 		target.runWhileOutOfSight = false;
    	}
    	
    	target.color = EditorGUILayout.ColorField("Color", target.color);
    	EditorGUILayout.LabelField("Width", " ");
        target.width = EditorGUILayout.Slider(target.width,0.1, 5);
	   	EditorGUILayout.LabelField("Speed", " ");
		target.speed = EditorGUILayout.Slider(target.speed,1, 40);
        EditorGUILayout.LabelField("Jump", " ");
        target.jump = EditorGUILayout.Slider(target.jump,0.1, 6.0);
        target.updateDistance = EditorGUILayout.Toggle("Update distance", target.updateDistance);
        if(target.updateDistance == true){
        	EditorGUILayout.LabelField("Max distance", " ");
			target.maxDistance = EditorGUILayout.IntSlider(target.maxDistance,1, 500);
		}
		else{
			EditorGUILayout.LabelField("Distance", " ");
			target.dis = EditorGUILayout.IntSlider(target.dis,1, 500);
        }
        target.clampStart = EditorGUILayout.Toggle("Clamp start", target.clampStart);
        target.clampEnd = EditorGUILayout.Toggle("Clamp end", target.clampEnd);
        target.createLight = EditorGUILayout.Toggle("Create light?", target.createLight);
        target.glow = EditorGUILayout.Toggle("Glow?", target.glow);
        target.runWhileOutOfSight = EditorGUILayout.Toggle("Run out of sight", target.runWhileOutOfSight);
        target.sparks = EditorGUILayout.Toggle("Sparks?", target.sparks);
        if(target.sparks == true){
        	EditorGUILayout.LabelField("Sparkyness", " ");
			target.spread = EditorGUILayout.Slider(target.spread,0.001, 0.1);
		}
        if (GUI.changed)
            EditorUtility.SetDirty (target);
    }
}