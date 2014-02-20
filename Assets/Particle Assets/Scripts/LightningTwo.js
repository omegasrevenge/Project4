#pragma strict

var minTime = 0.5;
var threshold = 0.5;
var myLight : Light;

private var lastTime = 0;



function Update () {

if ((Time.time - lastTime) > minTime)
if (Random.value > threshold)
light.enabled = true;
else
light.enabled = false;
lastTime = Time.time;
}