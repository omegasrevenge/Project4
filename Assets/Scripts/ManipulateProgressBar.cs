using UnityEngine;
using System.Collections;

public class ManipulateProgressBar : MonoBehaviour {

    public dfProgressBar content;

    void Awake()
    {
        content = GetComponent<dfProgressBar>();
    }

    public void IncProgressBar()
    {
        content.Value += 0.2f;
    }

    public void DecProgressBar()
    {
        content.Value -= 0.2f;
    }
}
