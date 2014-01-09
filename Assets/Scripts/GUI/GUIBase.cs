using System.Net.NetworkInformation;
using UnityEngine;
using System.Collections;

public class GUIBase : MonoBehaviour {

	void Start () {
	
	}

	void OnGUI()
	{
		if (GameManager.Singleton.LoggedIn)
		{
			if (GUI.Button(new Rect(10, 180, 120, 50), "Choose as Base!"))
			{
				GameManager.Singleton.SendBasePosition();
				Debug.Log("Current Base Poition: " + LocationManager.GetCurrentPosition());
			}
		}
	}

	void Update () {
	
	}
}
