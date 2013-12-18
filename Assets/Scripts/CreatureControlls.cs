using UnityEngine;
using System.Collections;

public class CreatureControlls : MonoBehaviour {

	public enum Phase{None, Attack, Wait, Back}

	public int Health = 100;
	public bool Hit = false;
	public Transform StartPosition;
	public int AttackSpeed = 5;
	public bool Projectile = false;
	public BattleEngine Owner;

	public Phase CurrentPhase{get;}
	private Transform _target;

	void Start()
	{
		CurrentPhase = Phase.None;
	}

	public void Attack(Transform target)
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
		if(CurrentPhase == Phase.Attack)
		{
			transform.position = Vector3.Lerp(transform.position, _target.position, AttackSpeed/100f);
			if(HaveReached(_target)) CurrentPhase = Phase.Wait;
		}
		if(CurrentPhase == Phase.Wait)
		{
			if(Projectile)
			{
				Destroy(gameObject);
			}
			//if not projectile, wait for result
		}
		if(CurrentPhase == Phase.Back)
		{
			transform.position = Vector3.Lerp(transform.position, StartPosition.position, AttackSpeed/100f);
			if(HaveReached(StartPosition)) CurrentPhase = Phase.None;
		}
	}

	private bool HaveReached(Transform target)
	{
		return Mathf.Abs((transform.position-target.position).magnitude)<0.1f ? true : false;
	}

}