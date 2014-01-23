using UnityEngine;
using System.Collections;

public class DamageIndicator : MonoBehaviour 
{
	private Camera _battleCam;
	private Vector3   _proxyPos;

	void Awake()
	{
		_battleCam = BattleEngine.Current.BattleCam.GetComponent<Camera>();
		_proxyPos = transform.position;
	}

	void Update () 
	{
		_proxyPos.y += Time.deltaTime;
		transform.position = _battleCam.WorldToViewportPoint (_proxyPos); 
	}
}