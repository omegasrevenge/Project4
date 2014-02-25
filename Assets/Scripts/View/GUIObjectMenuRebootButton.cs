using System;
using UnityEngine;

public class GUIObjectMenuRebootButton : GUIObjectMenuButton
{
    void OnClick(dfControl control, dfMouseEventArgs args)
    {
        if (args.Used) return;
        args.Use();
        GameManager.Singleton.SetFaction(Player.Faction.VENGEA);
    }

}

