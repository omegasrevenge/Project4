using System;
using UnityEngine;

public class GUIObjectMenuRebootButton : GUIObjectMenuButton
{

    private bool _initialized;

    void Update()
    {
        if (!_initialized && GameManager.Singleton.Player != null && GameManager.Singleton.Player.InitSteps >0)
        {
            if(GameManager.Singleton.Player.CurrentFaction == Player.Faction.VENGEA)
            {
                GetComponent<dfControl>().Hide();
            }
            _initialized = true;
        }
    }

    void OnClick(dfControl control, dfMouseEventArgs args)
    {
        if (args.Used) return;
        args.Use();
        GameManager.Singleton.SetFaction(Player.Faction.VENGEA);
    }

}

