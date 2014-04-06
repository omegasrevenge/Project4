using UnityEngine;

public class GUIObjectReboot : MonoBehaviour
{

    private const float SecondScreen = 3f;
    private const float ThirdScreen = 6f;
    private const float Reboot = 9f;

    private int screen = 0;

    private float _startTime;
	// Use this for initialization
	void Start ()
	{
	    _startTime = Time.time;
        GameManager.Singleton.GUIShowLoadingScreen("loadingscreen_reboot");
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (screen == 0 && Time.time - _startTime > SecondScreen)
	    {
            GameManager.Singleton.GUIShowLoadingScreen("loadingscreen_restart");
	        screen++;
	    }
        else if (screen == 1 && Time.time - _startTime > ThirdScreen)
        {
            GameManager.Singleton.GUIShowLoadingScreen("loadingscreen_close");
            screen++;
        }
        else if (screen == 2 && Time.time - _startTime > Reboot)
        {
            Application.LoadLevel(1);
        }
	}
}
