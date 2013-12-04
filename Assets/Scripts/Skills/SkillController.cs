using UnityEngine;
using System.Collections;

public class SkillController : MonoBehaviour {

	private Transform _pos;

	void Update () 
	{
		if(_pos == null && GameObject.Find("Player(Clone)") != null)
		{
			_pos = GameObject.Find("Player(Clone)").transform;
		}
	}

	public void CreateFireball()
	{
		Object FireballPrefab = Resources.Load("Fireball");
		GameObject Fireball = (GameObject)Network.Instantiate(FireballPrefab, _pos.localPosition, _pos.localRotation, 1);
	}
	
	public void CreateAxe()
	{
		Object AxePrefab = Resources.Load("SeverReality");
		GameObject Axe = (GameObject)Network.Instantiate(AxePrefab, _pos.localPosition, _pos.localRotation, 1);
	}
	
	public void CreateLaser()
	{
		Object LaserPrefab = Resources.Load("Laser");
		GameObject Laser = (GameObject)Network.Instantiate(LaserPrefab, _pos.localPosition, _pos.localRotation, 1);
	}
}
