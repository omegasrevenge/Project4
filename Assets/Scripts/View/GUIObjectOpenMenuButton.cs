using UnityEngine;
using System.Collections;

public class GUIObjectOpenMenuButton : MonoBehaviour
{
    private dfControl _control;
	// Use this for initialization
	void Start ()
	{
	    _control = GetComponent<dfControl>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(GameManager.Singleton.CurrentGameMode != GameManager.GameMode.Map)
	        _control.Hide();
        else
        {
            _control.Show();
        }
	}
}
