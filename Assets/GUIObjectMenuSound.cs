using System;
using UnityEngine;

public class GUIObjectMenuSound : MonoBehaviour 
{
    private void Awake()
    {
        GUIObjectSwitch guiSwitch = GetComponent<GUIObjectSwitch>();
        guiSwitch.Active = PlayerPrefs.GetFloat("Sound", 1f) >= 0.5f;
        guiSwitch.Switch += OnSwitch;
    }

    private void OnSwitch(bool b)
    {
        SoundController.Enabled = b;
    }
}
