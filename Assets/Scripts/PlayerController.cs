using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class PlayerController : MonoBehaviour {

	public int MovementSpeed = 10;
	public int JumpForce = 3;
	public int HealthPoints = 100;

	private bool _isDead
	{
		get
		{
			return HealthPoints <= 0;
		}
	}
	
	private Color _color
	{
		get
		{
			return GetComponent<MeshRenderer>().material.color;
		}
		set
		{
			GetComponent<MeshRenderer>().material.color = value;
		}
	}

	private ConstantForce _push;

	// Use this for initialization
	void Start () 
	{
		_push = GetComponent<ConstantForce>();
		if(networkView.isMine)
		{
			_color = new Color(_color.r, _color.g, _color.b+200, _color.a);
		}
		else
		{
			_color = new Color(_color.r+200, _color.g, _color.b, _color.a);
		}
	}

	void OnCollisionStay(Collision collision)
	{

		if(!networkView.isMine) return;

		if(collision.gameObject.layer == 8 && Input.GetKeyDown(KeyCode.Space))
		{
			Jump();
		}
	}

	// Update is called once per frame
	void Update () 
	{
		
		if(_isDead && _color.a > 0)
		{
			_color = new Color(_color.r, _color.g, _color.b, _color.a-0.005f);
		}

		if(!networkView.isMine) return;

		HandleInput();

	}

	public void HandleInput()
	{
		if(_isDead) return;

		if(Input.GetKey(KeyCode.D) && _push.force.z <= 0)
		{
			MoveRight();
		}
		
		if(Input.GetKey(KeyCode.A) && _push.force.z >= 0)
		{
			MoveLeft();
		}
		
		if(!Input.anyKey)
		{
			_push.force = Vector3.zero;
		}
	}

	private void MoveRight()
	{
		transform.localEulerAngles = Vector3.zero;
		_push.force = Vector3.forward*MovementSpeed;
	}

	private void MoveLeft()
	{
		transform.localEulerAngles = Vector3.up*180;
		_push.force = Vector3.back*MovementSpeed;
	}

	private void Jump()
	{
		GetComponent<Rigidbody>().AddForce(Vector3.up*JumpForce - new Vector3(0, GetComponent<Rigidbody>().velocity.y,0));
	}
}
