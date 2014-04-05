using UnityEngine;
using System.Collections;

public class GUIObjectMenuSpectre : GUIObjectMenuButton
{
    public dfLabel Name;
    public dfSprite SpectreSymbol;

    private const string Fenris = "symbol_fenris";
    private const string Giant = "symbol_giant";

    
    void Update () 
    {

        if (GameManager.Singleton.CurrentGameMode == GameManager.GameMode.Map && GameManager.Singleton.Player != null && GameManager.Singleton.Player.InitSteps >= 3)
        {
            Name.Text = GameManager.Singleton.Player.CurCreature.Name;
            if (GameManager.Singleton.Player.CurCreature.ModelID == 0)
                SpectreSymbol.SpriteName = Fenris;
            else
                SpectreSymbol.SpriteName = Giant;

        }
	}

    void OnClick(dfControl control, dfMouseEventArgs args)
    {
        GameManager.Singleton.GUIShowSpectreName();
    }
}
