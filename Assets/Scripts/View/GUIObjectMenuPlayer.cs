using UnityEngine;
using System.Collections;

public class GUIObjectMenuPlayer : MonoBehaviour
{
    public dfLabel Name;
    public dfSprite UserPicture;

    private const string Vengea = "profile_vengea";
    private const string NCE = "profile_nce";

    private bool _initialized = false;
    
    void Update () 
    {
        if (_initialized) return;
        if (GameManager.Singleton.Player != null && GameManager.Singleton.Player.InitSteps > 0)
        {
            Name.Text = GameManager.Singleton.Player.Name;
            if (GameManager.Singleton.Player.CurrentFaction == Player.Faction.VENGEA)
                UserPicture.SpriteName = Vengea;
            else
                UserPicture.SpriteName = NCE;

            _initialized = true;
        }
	}
}
