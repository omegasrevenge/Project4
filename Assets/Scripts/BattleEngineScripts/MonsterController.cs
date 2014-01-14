using UnityEngine;
using System.Collections;

public class MonsterController : ActorControlls 
{
	[SerializeField]
	public int AttackSpeed = 5;
	[SerializeField]
	public int Health;

	[HideInInspector]
	public Vector3 StartPosition;

	private Vector3 _target;
	private Transform _bgHealthbar;
	private Transform _healthbar;
	private Transform _battleCam;
	private MonsterStats _monsterStats;

	void Awake()
	{
		_monsterStats = GetComponent<MonsterStats>();
		_battleCam = GameObject.Find("BattleCamera").transform;
		_bgHealthbar = transform.FindChild("Healthbar");
		_healthbar = transform.FindChild("Health");
	}

	public void Attack(Vector3 target)
	{
		AnimationFinished = false;
		_target = target;
	}

	void Update () 
	{
		UpdateHealthBar();

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

	private void UpdateHealthBar()
	{
		_bgHealthbar.LookAt(_battleCam);
		_healthbar.LookAt(_battleCam);
		float a = Health;
		float b = _monsterStats.HP;
		_healthbar.localScale = new Vector3(a/b, _healthbar.localScale.y, _healthbar.localScale.z);
	}
}
