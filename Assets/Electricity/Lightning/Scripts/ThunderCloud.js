#pragma strict
var frequency = 10.0;
var range = 100.0;
var thunderbolt : Transform;
var color : Color = Color.white;
var width = 6.0;
var jump = 5.0;
var maxDistance = 120;
var sparks = true;
var spread = 0.1;
var timeOut = 0.5;
var glow = true;
var createLight = true;
private var delay = 3.0;

function Start(){
	MakeThunder();
}

function MakeThunder(){
    var a = true;
    while(a == true){
    	delay = Random.Range(0.0, frequency);
    	var pos : Vector3 = Vector3(Random.Range(-range, range), transform.position.y, Random.Range(-range, range));
    	var bolt : Transform = Instantiate(thunderbolt, pos, transform.rotation) as Transform;
    	var script : Thunderbolt = bolt.GetComponent(Thunderbolt) as Thunderbolt;
    	script.SetStats(color, width, jump, maxDistance, sparks, spread, timeOut, glow, createLight);
    	yield WaitForSeconds(delay);
    }
}