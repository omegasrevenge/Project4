using UnityEngine;
using System.Collections;

public class BuffEmitter : MonoBehaviour {

	public float TimeBetweenBuffs = 10;

	private float _timeCounter = 0f;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!networkView.isMine) return;

		_timeCounter += Time.deltaTime;
		if(_timeCounter >= TimeBetweenBuffs)
		{
			_timeCounter = 0f;
			SpawnBuff();
		}
	}


	private void SpawnBuff()
	{
		Vector3 spawnPos = new Vector3(GameObject.Find("Player(Clone)").transform.localPosition.x, 
		                               30f, 
		                               Random.Range(GameObject.Find("Base1").transform.localPosition.z, 
		             								GameObject.Find("Base2").transform.localPosition.z));

		GameObject Buff = (GameObject)Network.Instantiate(Resources.Load("NoCdBuff"), spawnPos, Quaternion.identity, 1);
	}
}
