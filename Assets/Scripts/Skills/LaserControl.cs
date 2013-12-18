using UnityEngine;
using System.Collections;

public class LaserControl : MonoBehaviour {

	public float FinalHeadScale = 3f;
	public float HeadScalingSpeed = 3f;
	public float FadeOutTime = 2f;
	public float LifeTime = 5f;
	public int Damage = 80;

	private float _counter;
	private Transform _head;
	private Transform _beam;
	private bool _canDamage = true;

	// Use this for initialization
	void Start () 
	{
		_head = transform.FindChild("Head");
		_beam = transform.FindChild("Beam");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(_head.localScale.x < FinalHeadScale*0.95)
		{
			_head.localScale = Vector3.Lerp(_head.localScale, Vector3.one*FinalHeadScale, HeadScalingSpeed*Time.deltaTime);
		}
		else
		{
			if(!_beam.gameObject.activeSelf)
			{
				_beam.gameObject.SetActive(true);
				//damageinstance happens now
			}
		}

		if(_beam.gameObject.activeSelf)
		{
			_head.GetComponent<MeshRenderer>().material.color = new Color(_head.GetComponent<MeshRenderer>().material.color.r,
			                                                              _head.GetComponent<MeshRenderer>().material.color.g,
			                                                              _head.GetComponent<MeshRenderer>().material.color.b,
			                                                              _head.GetComponent<MeshRenderer>().material.color.a-(Time.deltaTime/FadeOutTime));
			
			_beam.GetComponent<MeshRenderer>().material.color = new Color(_beam.GetComponent<MeshRenderer>().material.color.r,
			                                                              _beam.GetComponent<MeshRenderer>().material.color.g,
			                                                              _beam.GetComponent<MeshRenderer>().material.color.b,
			                                                              _beam.GetComponent<MeshRenderer>().material.color.a-(Time.deltaTime/FadeOutTime));
		}

		_counter += Time.deltaTime;
		if(networkView.isMine && _counter >= LifeTime)
		{
			networkView.RPC("Die", RPCMode.AllBuffered);
		}
	}

	[RPC]
	public void Die()
	{
		Destroy(gameObject);
	}

	public void Collided(Collider collider)
	{
		if(!networkView.isMine) return;
		if(collider.tag == "Player" && collider.networkView.isMine != networkView.isMine && _canDamage)
		{
			_canDamage = false;
			collider.GetComponent<PlayerController>().HealthPoints -= Damage;
		}
	}
}
