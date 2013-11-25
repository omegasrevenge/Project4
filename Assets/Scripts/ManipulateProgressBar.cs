using UnityEngine;
using System.Collections;

public class ManipulateProgressBar : MonoBehaviour {

    public GameObject content;
	private dfProgressBar bar;

    void Awake()
    {
		if(content == null) content = gameObject;
        bar = content.GetComponent<dfProgressBar>();
    }

    public void IncProgressBar()
    {
        bar.Value += 0.2f;
    }

    public void DecProgressBar()
    {
        bar.Value -= 0.2f;
    }
}
