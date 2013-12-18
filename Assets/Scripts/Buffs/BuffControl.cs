using UnityEngine;
using System.Collections;

public class BuffControl : MonoBehaviour {


	public void DestroyThis()
	{
		networkView.RPC("Die", RPCMode.AllBuffered);
	}

	[RPC]
	public void Die()
	{
		Destroy(gameObject);
	}
}
