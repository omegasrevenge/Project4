using UnityEngine;
using System.Collections;

public class SkillController : MonoBehaviour {

	public float FireballCooldown = 5f;
	public float AxeCooldown = 1f;
	public float LaserCooldown = 10f;
	public float NoCooldownsLength = 3f;
	
	private float _fireballCooldownTimer = 0f;
	private float _axeCooldownTimer = 0f;
	private float _laserCooldownTimer = 0f;
	private float _noCooldownTimer = 0f;
	
	private dfRadialSprite _cooldownFireball;
	private dfRadialSprite _cooldownAxe;
	private dfRadialSprite _cooldownLaser;
	
	private dfLabel _cooldownTimerFireball;
	private dfLabel _cooldownTimerAxe;
	private dfLabel _cooldownTimerLaser;

	public bool NoCooldowns = false;
	public Transform Pos;

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
		if(Pos == null) return;

		if(NoCooldowns)
		{
			NoCooldowns = false;
			_noCooldownTimer = NoCooldownsLength;
		}
		HandleInput();
		HandleSkillCooldowns ();
	}

	private void HandleInput()
	{
		if(!Input.anyKeyDown) return;
		if(Input.GetKeyDown(KeyCode.Keypad1)) CreateFireball();
		if(Input.GetKeyDown(KeyCode.Keypad2)) CreateLaser();
		if(Input.GetKeyDown(KeyCode.Keypad3)) CreateAxe();
	}

	private void HandleSkillCooldowns()
	{
		if(_noCooldownTimer > 0f)
		{
			_noCooldownTimer -= Time.deltaTime;
			_fireballCooldownTimer = 0f;
			_cooldownFireball.FillAmount = _fireballCooldownTimer / FireballCooldown;
			_axeCooldownTimer = 0f;
			_cooldownAxe.FillAmount = _axeCooldownTimer / AxeCooldown;
			_laserCooldownTimer = 0f;
			_cooldownLaser.FillAmount = _laserCooldownTimer / LaserCooldown;
		}

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
		if (_fireballCooldownTimer > 0f || Pos == null) return;
		_fireballCooldownTimer = FireballCooldown;

		Object FireballPrefab = Resources.Load("Fireball");
		GameObject Fireball = (GameObject)Network.Instantiate(FireballPrefab, Pos.localPosition, Pos.localRotation, 1);
	}
	
	public void CreateAxe()
	{
		if (_axeCooldownTimer > 0f || Pos == null) return;
		_axeCooldownTimer = AxeCooldown;

		Object AxePrefab = Resources.Load("SeverReality");
		GameObject Axe = (GameObject)Network.Instantiate(AxePrefab, Pos.localPosition, Pos.localRotation, 1);
	}
	
	public void CreateLaser()
	{
		if (_laserCooldownTimer > 0f || Pos == null) return;
		_laserCooldownTimer = LaserCooldown;

		Object LaserPrefab = Resources.Load("Laser");
		GameObject Laser = (GameObject)Network.Instantiate(LaserPrefab, Pos.localPosition, Pos.localRotation, 1);
	}

	public void Jump()
	{
		if(Pos == null) return;
		Pos.GetComponent<PlayerController>().Jump();
	}
}
