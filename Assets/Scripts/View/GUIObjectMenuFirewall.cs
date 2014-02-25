using UnityEngine;

public class GUIObjectMenuFirewall : MonoBehaviour
{
    private bool _initialized;
    private void Update()
    {
        if (_initialized || GameManager.Singleton.Player == null) return;
        GUIObjectSwitch guiSwitch = GetComponent<GUIObjectSwitch>();
        guiSwitch.Switch += OnSwitch;
        guiSwitch.Active = GameManager.Singleton.Player.Firewall;
    }

    private void OnSwitch(bool b)
    {
        if (_initialized)
        {
            GameManager.Singleton.SetFirewall(b);
            SoundController.PlaySound(SoundController.SoundClick, SoundController.ChannelSFX);
            if(!b)
                GameManager.Singleton.GUIFirewallWarning();
        }
        else
            _initialized = true;
    }
}
