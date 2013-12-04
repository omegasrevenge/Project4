using UnityEngine;
using System.Collections;

public class AxeControl : MonoBehaviour {

	public float RotationSpeed = 500f;
	public float SwingTime = 0.75f;
	public float LifeTime = 4f;

	private float _counter = 0f;
	private bool _axeLookingRight;

	private Transform _emitters
	{
		get
		{
			return transform.FindChild("Axe").FindChild("Head").FindChild("Sparkle Rising");
		}
	}

	private bool _playerLookingRight
	{
		get
		{
			return (GameObject.Find("Player(Clone)").transform.localEulerAngles.y < 50f && GameObject.Find("Player(Clone)").transform.localEulerAngles.y > -50f);
		}
	}
	
	void Start()
	{
		_axeLookingRight = _playerLookingRight;
		if(!_playerLookingRight)
		{
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 180f, transform.localEulerAngles.z);
		}
	}

	void Update () 
	{
		if(networkView.isMine) transform.Rotate(Vector3.right*(RotationSpeed*Time.deltaTime), Space.Self);
		HandleTime();
	}

	private void HandleTime()
	{
		_counter += Time.deltaTime;

		if(_counter >= SwingTime)
		{
			StopActing();
		}

		if(_counter >= LifeTime)
		{
			Destroy(gameObject);
		}
	}

	private void StopActing()
	{
		transform.FindChild("Axe").GetComponent<MeshRenderer>().material.color = new Color(0f,0f,0f,0f);
		transform.FindChild("Axe").FindChild("Head").GetComponent<MeshRenderer>().material.color = new Color(0f,0f,0f,0f);
		_emitters.FindChild("SparkleParticles").GetComponent<ParticleEmitter>().emit = false;
		_emitters.FindChild("SparkleParticlesSecondary").GetComponent<ParticleEmitter>().emit = false;
	}
}
