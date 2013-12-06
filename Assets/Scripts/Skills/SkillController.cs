using UnityEngine;
using System.Collections;

public class SkillController : MonoBehaviour {

	public float FireballCooldown = 5f;
	public float AxeCooldown = 1f;
	public float LaserCooldown = 10f;
	
	private float _fireballCooldownTimer = 0f;
	private float _axeCooldownTimer = 0f;
	private float _laserCooldownTimer = 0f;
	
	private dfRadialSprite _cooldownFireball;
	private dfRadialSprite _cooldownAxe;
	private dfRadialSprite _cooldownLaser;
	
	private dfLabel _cooldownTimerFireball;
	private dfLabel _cooldownTimerAxe;
	private dfLabel _cooldownTimerLaser;


	private Transform _pos;

	void Start()
	{
		_cooldownFireball = GameObject.Find ("Fireball").transform.FindChild ("Cooldown").GetComponent<dfRadialSprite> ();
		_cooldownAxe = GameObject.Find ("SeverReality").transform.FindChild ("Cooldown").GetComponent<dfRadialSprite> ();
		_cooldownLaser = GameObject.Find ("Laser").transform.FindChild ("Cooldown").GetComponent<dfRadialSprite> ();
		
		_cooldownTimerFireball = _cooldownFireball.transform.FindChild ("CooldownText").GetComponent<dfLabel> ();
		_cooldownTimerAxe = _cooldownAxe.transform.FindChild ("CooldownText").GetComponent<dfLabel> ();
		_cooldownTimerLaser = _cooldownLaser.transform.FindChild ("CooldownText").GetComponent<dfLabel> ();
	}

	void Update () 
	{
		HandleSkillCooldowns ();

		if(_pos != null) return;
		InitPos ();
	}

	private void InitPos()
	{
		if(GameObject.Find("Player(Clone)") != null)
		{
			foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player"))
			{
				if(player.networkView.isMine)
				{
					_pos = GameObject.Find("Player(Clone)").transform;
					return;
				}
			}
		}
	}

	private void HandleSkillCooldowns()
	{
		if (_fireballCooldownTimer > 0f) 
		{
			_fireballCooldownTimer -= Time.deltaTime;
			_cooldownFireball.FillAmount = _fireballCooldownTimer / FireballCooldown;
			_cooldownTimerFireball.Text = Mathf.Round(_fireballCooldownTimer).ToString();
		} else _cooldownTimerFireball.Text = "";


		if (_axeCooldownTimer > 0f) 
		{
			_axeCooldownTimer -= Time.deltaTime;
			_cooldownAxe.FillAmount = _axeCooldownTimer / AxeCooldown;
			_cooldownTimerAxe.Text = Mathf.Round(_axeCooldownTimer).ToString();
		} else _cooldownTimerAxe.Text = "";


		if (_laserCooldownTimer > 0f) 
		{
			_laserCooldownTimer -= Time.deltaTime;
			_cooldownLaser.FillAmount = _laserCooldownTimer / LaserCooldown;
			_cooldownTimerLaser.Text = Mathf.Round(_laserCooldownTimer).ToString();
		} else _cooldownTimerLaser.Text = "";
	}

	public void CreateFireball()
	{
		if (_fireballCooldownTimer > 0f) return;
		_fireballCooldownTimer = FireballCooldown;

		Object FireballPrefab = Resources.Load("Fireball");
		GameObject Fireball = (GameObject)Network.Instantiate(FireballPrefab, _pos.localPosition, _pos.localRotation, 1);
	}
	
	public void CreateAxe()
	{
		if (_axeCooldownTimer > 0f) return;
		_axeCooldownTimer = AxeCooldown;

		Object AxePrefab = Resources.Load("SeverReality");
		GameObject Axe = (GameObject)Network.Instantiate(AxePrefab, _pos.localPosition, _pos.localRotation, 1);
	}
	
	public void CreateLaser()
	{
		if (_laserCooldownTimer > 0f) return;
		_laserCooldownTimer = LaserCooldown;

		Object LaserPrefab = Resources.Load("Laser");
		GameObject Laser = (GameObject)Network.Instantiate(LaserPrefab, _pos.localPosition, _pos.localRotation, 1);
	}
}
