var pointDown = false;
var color : Color = Color.white;
var width = 0.25;
var lineRenderer : LineRenderer;
var frequency = 1.5;
var jump = 1.5;
var dis = 3.0;
var glow = true;
private var curPos : Vector3[];
function Start() {
	var tempColor = color;
	tempColor.a = 255;
	lineRenderer = (GetComponent(LineRenderer) as LineRenderer);
	lineRenderer.SetColors(tempColor, tempColor);
	lineRenderer.SetWidth(width,width);
    UpdateChains();
}

function UpdateChains(){
    var i = 1;
    var lastPos : Vector3;
    var totalZ = 0.0;
    var arr = new Array ();
    if(pointDown == true)
    	transform.eulerAngles = Vector3(Random.Range(60, 120), Random.Range(-180, 180), Random.Range(-90, 90));
    	
    while(totalZ < dis){
		lineRenderer.SetVertexCount(i + 1);
		var pos : Vector3 = Vector3(Random.Range(-jump + lastPos.x, jump + lastPos.x), Random.Range(-jump + lastPos.y, jump + lastPos.y),Random.Range(0.0 + lastPos.z, 1.0 + lastPos.z));
		totalZ = pos.z;
		lineRenderer.SetPosition(i, pos);
		i++;
		lastPos = pos;
		arr.Add(lastPos);
	}
	curPos = arr;
	if(glow == true)
		Glow();
}
function Glow(){
	particleEmitter.ClearParticles();
	particleEmitter.Emit(curPos.length);
	var particles = particleEmitter.particles;
	for (var i = 0; i < particles.Length; i++) {
		particles[i].position = curPos[i];
		particles[i].color = color;
		particles[i].size = width * 8;
	}
	particleEmitter.particles = particles;
}
function DestroySpark(){
	Destroy(gameObject);
}