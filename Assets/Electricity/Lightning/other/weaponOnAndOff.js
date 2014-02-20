var lightnings : GameObject[];
private var alreadyOn = true;
function OnGUI(){
	GUI.Label(Rect(0,0,300,200), "Click to turn lightning gun on and off");
}
function Update () {
	if(Input.GetButtonDown("Fire1")){
		for(var lightning : GameObject in lightnings){
			lightning.active = !lightning.active;
		}
		if(alreadyOn == false){
			alreadyOn = true;
		}
		else
			alreadyOn = false;
	}
}