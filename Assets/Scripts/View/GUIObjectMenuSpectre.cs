using UnityEngine;
using System.Collections;

public class GUIObjectMenuSpectre : GUIObjectMenuButton
{
    public dfLabel Name;
    public dfSprite SpectreSymbol;

    private const string Fenris = "symbol_fenris";
    private const string Giant = "symbol_giant";

    private bool _initialized = false;
    
    void Update () 
    {
        if (_initialized) return;
        if (GameManager.Singleton.Player != null && GameManager.Singleton.Player.InitSteps > 0)
        {
            Name.Text = GameManager.Singleton.Player.CurCreature.Name;
            if (GameManager.Singleton.Player.CurCreature.ModelID == 0)
                SpectreSymbol.SpriteName = Fenris;
            else
                SpectreSymbol.SpriteName = Giant;

            _initialized = true;
        }
	}

    void OnClick(dfControl control, dfMouseEventArgs args)
    {
        //TODO Hide Menu
        GameManager.Singleton.GUIShowSpectreName();
    }
}
