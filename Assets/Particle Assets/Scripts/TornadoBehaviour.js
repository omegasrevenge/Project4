#pragma strict

var lastChange : float = 0;
var changeInt : float = 1;

function Update () {
	//for flat terrain, seems to work better
	transform.Translate(Vector3.forward * Time.deltaTime * 10, Space.Self);
//	transform.Translate(Vector3(1,0,1) * Time.deltaTime * 2, Space.World);
	
	if (Time.time > lastChange + changeInt)
		{
			ChangeDirection();
			lastChange = Time.time;
			changeInt = Random.Range(0,2);
				
		}	

}

function ChangeDirection()
{
	transform.eulerAngles.y = Random.Range(0.0, 360.0);
}