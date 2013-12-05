using UnityEngine;
using System.Collections;

public class LaserCylinderControl : MonoBehaviour {
	
	public float RotationSpeed = 100f;
	
	void Update () 
	{
		transform.Rotate(Vector3.up*RotationSpeed*Time.deltaTime, Space.Self);
	}

	void OnTriggerEnter(Collider target)
	{
		transform.parent.GetComponent<LaserControl>().Collided(target);
	}
}
