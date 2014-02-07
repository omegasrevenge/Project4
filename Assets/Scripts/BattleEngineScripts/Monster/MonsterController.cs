using UnityEngine;
using System.Collections;

public class MonsterController : ActorControlls 
{
	[SerializeField]
	public int AttackSpeed = 5;

	[HideInInspector]
	public Vector3 StartPosition;

	private Vector3 _target;

	public void Attack(Vector3 target)
	{
		AnimationFinished = false;
		_target = target;
	}

	void Update () 
	{
		if(!AnimationFinished)
		{
			transform.position = Vector3.Lerp(transform.position, _target, AttackSpeed/100f);
			if(HaveReached(_target)) CanShowDamage = true;
		}
		if(CanShowDamage)
		{
			transform.position = Vector3.Lerp(transform.position, StartPosition, AttackSpeed/100f);
			if(HaveReached(StartPosition)) 
			{
				CanShowDamage     = false;
				AnimationFinished = true;
			}
		}
	}
	
	private bool HaveReached(Vector3 target)
	{
		return Mathf.Abs((transform.position-target).magnitude)<0.1f;
	}
}
