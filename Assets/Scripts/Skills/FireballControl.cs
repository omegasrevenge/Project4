using UnityEngine;
using System.Collections;

public class FireballControl : MonoBehaviour {

	public float Acceleration = 0.2f;
	public int RotationSpeed = 720;
	public float LifeTimeSeconds = 10;
	public int Damage = 50;

	private float counter = 0f;
	private Vector3 _direction;
	private bool _canDamage = true;
	
	private bool _playerLookingRight
	{
		get
		{
			foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player") )
			{
				if(player.networkView.isMine == networkView.isMine)
				{
					return (player.transform.localEulerAngles.y < 50f && player.transform.localEulerAngles.y > -50f);
				}
			}
			return (GameObject.Find("Player(Clone)").transform.localEulerAngles.y < 50f && GameObject.Find("Player(Clone)").transform.localEulerAngles.y > -50f);
		}
	}

	void Start()
	{
		if(_playerLookingRight)
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
		if(networkView.isMine && counter >= LifeTimeSeconds)
		{
			networkView.RPC("Die", RPCMode.AllBuffered);
		}

		if(!networkView.isMine) return;
		GetComponent<Rigidbody>().AddForce(_direction);
		transform.FindChild("Content").Rotate(new Vector3(RotationSpeed*Time.deltaTime,0,0), Space.Self);
	}

	[RPC]
	public void Die()
	{
		Destroy(gameObject);
	}
	
	void OnTriggerEnter(Collider collider)
	{
		if(!networkView.isMine) return;
		if(collider.tag == "Player" && collider.networkView.isMine != networkView.isMine && _canDamage)
		{
			_canDamage = false;
			collider.GetComponent<PlayerController>().HealthPoints -= Damage;
		}
	}
}
