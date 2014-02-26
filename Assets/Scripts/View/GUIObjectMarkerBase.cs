using UnityEngine;
using System.Collections;

public class GUIObjectMarkerBase : MonoBehaviour
{
    private const string SpriteStr = "sprite";
    private dfSprite sprite;
    private bool _initialized = false;
    
    void Update()
    {
        if (_initialized) return;
        if (GameManager.Singleton.Player.CurrentFaction == Player.Faction.NCE)
        {
            sprite = transform.Find(SpriteStr).GetComponent<dfSprite>();
            sprite.Color = GameManager.Black;
            _initialized = true;
        }
        
    }
}
