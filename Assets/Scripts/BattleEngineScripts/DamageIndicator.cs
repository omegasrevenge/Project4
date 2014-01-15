using UnityEngine;
using System.Collections;

public class DamageIndicator : MonoBehaviour 
{
	private GameObject _battleCam;

	void Awake()
	{
		_battleCam = GameObject.Find("BattleCamera");
	}

	void Update () 
	{
		transform.position = new Vector3(transform.position.x, transform.position.y+Time.deltaTime, transform.position.z);
		if(_battleCam != null) transform.LookAt(_battleCam.transform);
	}
}