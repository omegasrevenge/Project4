using UnityEngine;
using System.Collections;

public class SelfDestruct : MonoBehaviour {

	public float MaxAge = 3f;

	private float _counter = 0f;
	
	void Update () 
	{
		_counter += Time.deltaTime;
		if(_counter >= MaxAge)
		{
			Destroy(gameObject);
		}
	}
}
