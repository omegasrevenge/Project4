#pragma strict
var rate = 0.1;
var smooth = true;
var spread = 7.1;
var spark : Transform;
var runWhileOutOfSight = false;
var color : Color = Color.white;
var width = 1.2;
var dis = 0.2;
var jump = 6.2;
private var baseVertices : Vector3[];
private var mesh : Mesh;
private var run = false;

function Start (){
	var filter = (GetComponent(MeshFilter) as MeshFilter);
	mesh = (filter.mesh as Mesh);
	if (baseVertices == null)
		baseVertices = mesh.vertices;
	EmitLightning();
}

function EmitLightning() {
	while(run == true){
		transform.BroadcastMessage("DestroySpark", SendMessageOptions.DontRequireReceiver);
		for (var i=0;i<baseVertices.Length;i++){
			if(Random.Range(-1.0, spread) > 0.0){
				var sub : Transform = Instantiate(spark, transform.position, Random.rotation) as Transform;
				var script : BendyLightning = sub.GetComponent(BendyLightning) as BendyLightning;
				sub.parent = transform;
				sub.transform.localPosition = baseVertices[i];
				script.SetStats(color, width, dis, jump); 
			}
		}
		yield WaitForSeconds(rate);
	}
}

//get to work the boss is looking
function OnBecameVisible() {
	run = true;
	EmitLightning();
}
//why go though the hassel if we cant be seen.  Inless we want to that is.
function OnBecameInvisible () {
	if(runWhileOutOfSight == false){
		run = false;
		EmitLightning();
	}
}