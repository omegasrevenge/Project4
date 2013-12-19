using UnityEngine;
using System.Collections;

public class ScrollingRenderer : MonoBehaviour
{
    private MovableViewport _gameCam;
    private MovableViewport _guiCam;
	// Use this for initialization
	void Awake ()
	{
        _gameCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MovableViewport>();
        _guiCam = GameObject.FindGameObjectWithTag("GUICamera").GetComponent<MovableViewport>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	    float phase = (Mathf.Sin(Time.time) + 1f)/2f;
	    _gameCam.phase = phase;
	    _guiCam.phase = phase - 1;
	}
}
