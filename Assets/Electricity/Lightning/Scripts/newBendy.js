/*
#pragma strict
var pointDown = false;
var lineRenderer : LineRenderer;
var color : Color = Color.white;
var width = 0.5;
var dis = 5.0;
var jump = 0.2;
var smooth = false;
private var oldPos : Array;
private var delay = 0.0;
private var curTime = 0.0;

function Start() {
	lineRenderer = (GetComponent(LineRenderer) as LineRenderer);
	lineRenderer.SetColors(color, color);
	lineRenderer.SetWidth(width,width);
	oldPos = new Vector3[Mathf.Round(dis / 0.5)];
	if(pointDown == true)
    	transform.eulerAngles = Vector3(Random.Range(60, 120), Random.Range(-180, 180), Random.Range(-90, 90));
}

function FixedUpdate() {
	if(smooth == false){
		SnapVectors();
		return;
	}
	curTime += speed * Time.deltaTime;
	while(i < dis){
		curPos[i] = Vector3.Slerp(oldPos[i], newPos[i], curTime / delay);
		lineRenderer.SetPosition(i, curPos[i]);
		if(sparks == true){
			if(Random.Range(-1.0, spread) > 0.0){
				var sub : Transform = Instantiate(spark, transform.position, Random.rotation) as Transform;
				var script : SubLightning = sub.GetComponent(SubLightning) as SubLightning;
				sub.parent = transform;
				sub.transform.localPosition = curPos[i];
				script.color = color;
			}
		}
		i++;
	}
	i = 0;
}

function SnapVectors(){
	var i = 1;
    var lastPos : Vector3;
    var totalZ = 0.0;    	
    while(totalZ < dis){
		lineRenderer.SetVertexCount(i + 1);
		var pos = lastPos;
		pos.z += 0.5;
		pos = Quaternion(Random.Range(-jump, jump), Random.Range(-jump, jump), 0, 1) * pos;
		totalZ += 0.5;
		lineRenderer.SetPosition(i, pos);
		i++;
		lastPos = pos;
		oldPos[i - 2] = pos;
	}
}

function SetStats(a : Color, b : float, c : float, d : float, e : boolean){
	color = a;
	width = b;
	dis = c;
	jump = d;
	smooth = e;
}

function DestroySpark(){
	Destroy(gameObject);
}
*/