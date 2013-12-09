using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {

	public GameObject Player;
	
	// Update is called once per frame
	void Update () 
	{
		if(Player == null && GameObject.FindGameObjectsWithTag("Player").Length > 0)
		{
			foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player"))
			{
				if(player.networkView.isMine)
				{
					Player = player;
				}
			}
		}
	}
	
	public void MoveRight()
	{
		if(Player.GetComponent<ConstantForce>().force.z > 0) return;
		Player.transform.localEulerAngles = Vector3.zero;
		Player.GetComponent<ConstantForce>().force = Vector3.forward*Player.GetComponent<PlayerController>().MovementSpeed;
	}
	
	public void MoveLeft()
	{
		if(Player.GetComponent<ConstantForce>().force.z < 0) return;
		Player.transform.localEulerAngles = Vector3.up*180;
		Player.GetComponent<ConstantForce>().force = Vector3.back*Player.GetComponent<PlayerController>().MovementSpeed;
	}
}
