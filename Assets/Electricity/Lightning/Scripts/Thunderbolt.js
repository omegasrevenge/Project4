var color : Color = Color.white;
var width = 6.0;
var lineRenderer : LineRenderer;
var jump = 5.0;
var dis = 20;
var maxDistance = 120;
var sparks = true;
var spread = 0.1;
var timeOut = 0.5;
var spark : Transform;
var glow = true;
var createLight = false;
var lightningLight : Transform;
var thunderSound : Transform;
private var curPos : Vector3[];

function Start() {
	var tempColor = color;
	tempColor.a = 255;
	lineRenderer = (GetComponent(LineRenderer) as LineRenderer);
	lineRenderer.SetColors(tempColor, tempColor);
	lineRenderer.SetWidth(width,width);
	UpdateBolt();
}

function SetStats(a : Color, b : float, c : float, d : int, e : boolean, f : float, g : float, h : boolean, i : boolean){
	color = a;
	width = b;
	jump = c;
	maxDistance = d;
	sparks = e;
	spread = f;
	timeOut = g;
	glow = h;
	createLight = i;
}

function UpdateBolt() {
	var hit : RaycastHit;
	var i = 0;
	var totalY : float = 0.0;
	var lastPos : Vector3;
	var lightY : float = 0.0;
	var arr = new Array ();
	if (Physics.Raycast(transform.position, -Vector3.up, hit, maxDistance)){
		dis = Mathf.Round(hit.distance + 2);
	}
	else{
		dis = maxDistance;
	}
//lineRenderer.SetVertexCount(dis);

	while(totalY > -dis){
		lineRenderer.SetVertexCount(i + 1);
		var pos : Vector3 = Vector3(Random.Range(-jump + lastPos.x, jump + lastPos.x), Random.Range(-3.5, -1.5), Random.Range(-jump + lastPos.z, jump + lastPos.z));
		lightY += pos.y;
		pos.y += totalY;
		totalY = pos.y;
		lineRenderer.SetPosition(i, pos);
		
		if(sparks == true){
			if(Random.Range(-1.0, spread) > 0.0){
				var sub : Transform = Instantiate(spark, transform.position, transform.rotation) as Transform;
				var script : SubLightning = sub.GetComponent(SubLightning) as SubLightning;
				sub.parent = transform;
				sub.transform.localPosition = pos;
				script.color = color;
				script.frequency = timeOut;
//				sub.GetComponent("subLightning").color = color;
//				sub.GetComponent("subLightning").frequency = timeOut;
			}
		}
		
		if(lightY < -20.0 && createLight == true){
			var lighta : Transform = Instantiate(lightningLight, transform.position, transform.rotation) as Transform;
			lighta.parent = transform;
			lighta.transform.localPosition = pos;
			lighta.light.color = color;
			lightY = 0.0;
		}
		
		i++;
		lastPos = pos;
		arr.Add(lastPos);
	}
	curPos = arr;
	if(glow == true)
		Glow();
	Instantiate(thunderSound, transform.position, transform.rotation);
	Destroy(gameObject, timeOut);
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