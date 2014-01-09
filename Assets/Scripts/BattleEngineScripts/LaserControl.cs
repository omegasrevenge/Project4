using UnityEngine;
using System.Collections;

public class LaserControl : ActorControlls {

	public float FinalHeadScale = 3f;
	public float HeadScalingSpeed = 3f;
	public float FadeOutTime = 2f;
	public float LifeTime = 5f;

	private float _counter = 0f;
	private Transform _head;
	private Transform _beam;

	// Use this for initialization
	void Start () 
	{
		AnimationFinished = false;
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
				CanShowDamage = true;
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
		if(_counter >= LifeTime)
		{
			Owner.Actor = null;
			Destroy(gameObject);
		}
	}
}
