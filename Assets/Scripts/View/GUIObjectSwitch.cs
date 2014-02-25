using System;
using UnityEngine;
using System.Collections;

public class GUIObjectSwitch : MonoBehaviour 
{
    private const string SwitchStr = "switch";

    public event Action<bool> Switch; 
    private dfSlider _slider;

    private dfSlider Slider
    {
        get { return _slider ?? (_slider = transform.Find(SwitchStr).GetComponent<dfSlider>()); }
    }
    public bool Active
    {
        get
        {
            return Slider.Value == 1f;
        }
        set
        {
            if (value)
                Slider.Value = 1f;
            else
            {
                Slider.Value = 0f;
            }
        }
    }

    private void Awake()
    {
        Slider.ValueChanged += SliderOnValueChanged;
    }

    private void SliderOnValueChanged(dfControl control, float value)
    {
        if (Switch != null)
            Switch(Active);
    }

    private void OnClick(dfControl control, dfMouseEventArgs args)
    {
        if(args.Used) return;
        args.Use();
        Active = !Active;
    }
}
