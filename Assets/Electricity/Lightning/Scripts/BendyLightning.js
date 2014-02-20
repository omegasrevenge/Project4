#pragma strict
var pointDown = false;
var lineRenderer : LineRenderer;
var color : Color = Color.white;
var width = 0.5;
var dis = 5.0;
var jump = 0.2;
private var oldPos : Array;
 
function Start() {
	lineRenderer = (GetComponent(LineRenderer) as LineRenderer);
	lineRenderer.SetColors(color, color);
	lineRenderer.SetWidth(width,width);
	oldPos = new Vector3[Mathf.Round(dis / 0.5)];
}

function FixedUpdate() {
    var i = 1;
    var lastPos : Vector3;
    var totalZ = 0.0;
    if(pointDown == true)
    	transform.eulerAngles = Vector3(Random.Range(60, 120), Random.Range(-180, 180), Random.Range(-90, 90));
    	
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

function SetStats(a : Color, b : float, c : float, d : float){
	color = a;
	width = b;
	dis = c;
	jump = d;
}

function DestroySpark(){
	Destroy(gameObject);
}