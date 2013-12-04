using UnityEngine;
using System.Collections;

public class CylinderRotation : MonoBehaviour {
	
	public float RotationSpeed = 100f;
	
	void Update () 
	{
		transform.Rotate(Vector3.up*RotationSpeed*Time.deltaTime, Space.Self);
	}
}
