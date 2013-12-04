using UnityEngine;
using System.Collections;

public class FireballControl : MonoBehaviour {

	public float Acceleration = 0.2f;
	public int RotationSpeed = 720;
	public float LifeTimeSeconds = 10;

	private float counter = 0f;
	private Vector3 _direction;

	void Start()
	{
		if(GameObject.Find("Player(Clone)").transform.localEulerAngles.y < 50f 
		   && GameObject.Find("Player(Clone)").transform.localEulerAngles.y > -50f)
		{
			_direction = Vector3.forward*Acceleration;
		}
		else
		{
			_direction = Vector3.back*Acceleration;
		}
	}

	void Update () 
	{
		counter += Time.deltaTime;
		if(counter >= LifeTimeSeconds)
		{
			Destroy(gameObject);
		}

		if(!networkView.isMine) return;
		GetComponent<Rigidbody>().AddForce(_direction);
		transform.FindChild("Content").Rotate(new Vector3(RotationSpeed*Time.deltaTime,0,0), Space.Self);
	}
}
