using UnityEngine;

public class Button : TouchObject 
{

    void OnDestroy()
    {
        base.OnDestroy();
    }
	

    void Start () 
    {
        base.Start();
    }

    override public void OnTap(TouchInput.Touch2D touch2D)
    {
        GetComponent<Animator>().Play("blink");    
    }
}

