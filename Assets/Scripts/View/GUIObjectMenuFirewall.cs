using UnityEngine;

public class GUIObjectMenuFirewall : MonoBehaviour
{
    private bool _initialized;
    private void Update()
    {
        if (GameManager.Singleton.Player == null || GameManager.Singleton.Player.InitSteps == 0) return;
        if (!_initialized)
        {
            if(GameManager.Singleton.Player.CurrentFaction == Player.Faction.NCE)
                GetComponent<dfControl>().Parent.Hide();
            GUIObjectSwitch guiSwitch = GetComponent<GUIObjectSwitch>();
            guiSwitch.Switch += OnSwitch;
            guiSwitch.Active = GameManager.Singleton.Player.Firewall;

        }


    }

    private void OnSwitch(bool b)
    {
        if (_initialized)
        {
            GameManager.Singleton.SetFirewall(b);
            SoundController.PlaySound(SoundController.SoundFacClick, SoundController.ChannelSFX);
            if(!b)
                GameManager.Singleton.GUIFirewallWarning();
        }
        else
            _initialized = true;
    }
}
