#pragma strict
var delaySound = true;
var timeOut = 6.0;
function Start(){
	//if true we delay the sound by the distance from audio listener devided by the speed of sound
	if(delaySound == true)
		yield WaitForSeconds(Vector3.Distance(GameObject.FindWithTag("MainCamera").transform.position, transform.position) / 340.29);
	audio.Play();
	Destroy(gameObject, timeOut);
}