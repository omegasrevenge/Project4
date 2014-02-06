using UnityEngine;
using System.Collections;

public class MonsterController : ActorControlls 
{
	[SerializeField]
	public int AttackSpeed = 5;
	[SerializeField]
	public int Health;
	
	[HideInInspector]
	public Transform BgHealthbar;
	[HideInInspector]
	public Transform Healthbar;
	[HideInInspector]
	public Vector3 StartPosition;

	private Vector3 _target;
	private Transform _battleCam;
	private MonsterStats _monsterStats;

	void Awake()
	{
		_monsterStats = GetComponent<MonsterStats>();
	}

	public void Attack(Vector3 target)
	{
		AnimationFinished = false;
		_target = target;
	}

	void Update () 
	{
        if (_battleCam == null)
            _battleCam = BattleEngine.Current.Camera.transform;

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
		return Mathf.Abs((transform.position-target).magnitude)<0.1f ? true : false;
	}
}
