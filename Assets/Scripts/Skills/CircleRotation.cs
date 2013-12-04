using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Rigidbody))]
public class CircleRotation : MonoBehaviour {

	public float RotationSpeed = 100f;

	private Rigidbody _physics;

	void Start()
	{
		_physics = GetComponent<Rigidbody>();
	}

	void Update () 
	{
		_physics.AddTorque(new Vector3(RotationSpeed, RotationSpeed, RotationSpeed)*Time.deltaTime);
	}
}
