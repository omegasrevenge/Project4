using UnityEngine;
using System.Collections;

public class ActorControlls : MonoBehaviour {

	public enum Phase{None, Attack, Wait, Back}

	public int Health = 100;
	public bool Hit = false;
	public Vector3 StartPosition;
	public int AttackSpeed = 5;
	public bool Projectile = false;
	public BattleEngine Owner;

	public Phase CurrentPhase;
	private Vector3 _target;

	void Start()
	{
		CurrentPhase = Phase.None;
	}

	public void Attack(Vector3 target)
	{
		CurrentPhase = Phase.Attack;
		_target = target;
	}

	// Update is called once per frame
	void Update () 
	{
		if(Hit)
		{
			//you are hit. react somehow.
			Hit = false;
		}
		if(!Projectile && CurrentPhase == Phase.None)
		{
			//do idle animation or smth
		}
		if(CurrentPhase == Phase.Attack)
		{
			transform.position = Vector3.Lerp(transform.position, _target, AttackSpeed/100f);
			if(HaveReached(_target)) CurrentPhase = Phase.Wait;
		}
		if(CurrentPhase == Phase.Wait)
		{
			if(Projectile)
			{
				Owner.Actors.Remove(this);
				Destroy(gameObject);
			}
			else
			{
				if(Owner.Result == null) //server did not send result yet
				{
					//just do nothing and wait
				}
				else
				{
					CurrentPhase = Phase.Back;
				}
			}
		}
		if(CurrentPhase == Phase.Back)
		{
			transform.position = Vector3.Lerp(transform.position, StartPosition, AttackSpeed/100f);
			if(HaveReached(StartPosition)) CurrentPhase = Phase.None;
		}
	}

	private bool HaveReached(Vector3 target)
	{
		return Mathf.Abs((transform.position-target).magnitude)<0.1f ? true : false;
	}

}