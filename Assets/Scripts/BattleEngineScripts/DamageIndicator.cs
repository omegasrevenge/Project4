using UnityEngine;
using System.Collections;

public class DamageIndicator : MonoBehaviour 
{
	private float _counter = 0f;
	void Update () 
	{
		_counter += Time.deltaTime;
		if(_counter >= 3f)
		{
			Destroy(gameObject);
		}
		transform.position = new Vector3(transform.position.x, transform.position.y+Time.deltaTime, transform.position.z);
	}

}
