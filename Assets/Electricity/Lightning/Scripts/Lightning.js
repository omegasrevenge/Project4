var color : Color = Color.white;
var width = 1.5;
var lineRenderer : LineRenderer;
var speed = 30.0;
var jump = 4.0;
var dis = 20;
var updateDistance = false;
var maxDistance = 500;
var sparks = false;
var spread = 0.001;
var spark : Transform;
var clampStart = true;
var clampEnd = true;
var glow = true;
var createLight = false;
var lightObj : Transform;
var runWhileOutOfSight = false;
private var oldPos : Vector3[];
private var curPos : Vector3[];
private var newPos : Vector3[];
private var myTrans : Transform;
var lights : Array;
private var i = 0;
private var curTime = 0.0;
private var delay = 1.0;

function Start() {
	var tempColor = color;
	tempColor.a = 255;
	myTrans = transform;
	lineRenderer = (GetComponent(LineRenderer) as LineRenderer);
	lineRenderer.SetColors(tempColor, tempColor);
	lineRenderer.SetWidth(width,width);
	lineRenderer.SetVertexCount(dis);
	oldPos = new Vector3[dis];
	curPos = new Vector3[dis];
	newPos = new Vector3[dis];
	lights = new Transform[0];
	UpdateVectors();
}
function FixedUpdate(){
	transform.BroadcastMessage("DestroySpark", SendMessageOptions.DontRequireReceiver);
	if(updateDistance == true)
		UpdateDis();
	curTime += speed * Time.deltaTime;
	if(curTime >= delay){
		i = 0;
		UpdateVectors();
		curTime = 0.0;
	}
	else{
		while(i < dis){
			curPos[i] = Vector3.Slerp(oldPos[i], newPos[i], curTime / delay);
			lineRenderer.SetPosition(i, curPos[i]);
			if(sparks && spark){
				if(Random.Range(-1.0, spread) > 0.0){
					var sub : Transform = Instantiate(spark, myTrans.position, Random.rotation) as Transform;
					var script : SubLightning = sub.GetComponent(SubLightning) as SubLightning;
					sub.parent = myTrans;
					sub.transform.localPosition = curPos[i];
					script.color = color;
				}
			}
			i++;
		}
		i = 0;
	}
	if(createLight == true)
		MakeLights();
	else if(lights.length > 0){
		for(var oldLight : int = 0; oldLight < lights.length; oldLight++){
			Destroy((lights[oldLight] as GameObject).gameObject);
		}
		lights = new Transform[0];
	}
	particleEmitter.ClearParticles();
	if(glow == true)
		Glow();
}
function UpdateVectors(){	
	for(var b : int = 0; b < dis; b++){
		newPos[b] = Vector3(Random.Range(-jump, jump), Random.Range(-jump, jump), b);
		curPos[b].z = newPos[b].z;
		oldPos[b] = curPos[b];
	}
	newPos[0] = Vector3.zero;
	if(dis > 2)
		newPos[dis - 1] = Vector3(0,0,dis - 1);
	curPos[0].z = newPos[0].z;
	curPos[dis - 1].z = newPos[dis - 1].z;
	oldPos[0] = curPos[0];
	oldPos[dis - 1] = curPos[dis - 1];
}
function UpdateDis(){
	var hit : RaycastHit;
	var fwd = transform.TransformDirection (Vector3.forward);
	var temp1 = oldPos;
	var temp2 = curPos;
	var temp3 = newPos;
	if (Physics.Raycast (myTrans.position, fwd, hit, maxDistance)){
		dis = Mathf.Round(hit.distance + 1.5);
		oldPos = new Vector3[dis];
		curPos = new Vector3[dis];
		newPos = new Vector3[dis];
		lineRenderer.SetVertexCount(dis);
	}
	else{
		dis = maxDistance;
		oldPos = new Vector3[dis];
		curPos = new Vector3[dis];
		newPos = new Vector3[dis];
		lineRenderer.SetVertexCount(dis);
	}
	for(var a : int = 0; a < temp3.length && a < newPos.length; a++){
		oldPos[a] = temp1[a];
		curPos[a] = temp2[a];
		newPos[a] = temp3[a];
	}
	if(temp3.length < newPos.length)
		FixNewVectors(temp3.length);
}
function FixNewVectors(end : int){
	for(var n : int = end; n < newPos.length; n++){
		newPos[n] = Vector3(Random.Range(-jump, jump), Random.Range(-jump, jump), n);
		curPos[n].z = newPos[n].z;
		oldPos[n] = curPos[n];
	}
}
function MakeLights(){
	if(!lightObj) return;
//	for(var oldLight : int = 1; oldLight < lights.length; oldLight++){
//		Destroy(lights[oldLight - 1]);
//	}
//	lights = new GameObject[dis / 5];
	if(lights.length > (dis / 5)){
		while(lights.length > (dis / 5)){
			Destroy((lights.Pop() as GameObject).gameObject);
		}
	}
	for(var l : int = 1; l < (dis / 5); l++){
		if((l) > lights.length){
			var aLight : Transform = Instantiate(lightObj, myTrans.position, myTrans.rotation) as Transform;
			aLight.parent = myTrans;
			aLight.light.color = color;
			lights.Add(aLight);
		}
		(lights[l - 1] as GameObject).transform.localPosition = curPos[l * 5];
	}
}
function Glow(){
	particleEmitter.Emit(dis);
	var particles = particleEmitter.particles;
	for (var i = 0; i < particles.Length; i++) {
		particles[i].position = curPos[i];
		particles[i].color = color;
		particles[i].size = width * 8;
	}
	particleEmitter.particles = particles;
}
//get to work the boss is looking
function OnBecameVisible() {
	enabled = true;
}
//why go though the hassel if we cant be seen.  Inless we want to that is.
function OnBecameInvisible () {
	if(runWhileOutOfSight == false)
		enabled = false;
}