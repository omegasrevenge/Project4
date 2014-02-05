using UnityEngine;

public class MarkerBiod: MonoBehaviour
{
    private const string SymbolStr = "sprite_symbol";
    private const string Prefix = "element_";

    public void SetElementSymbol(string element)
    {
        dfSprite sprite = transform.Find(SymbolStr).GetComponent<dfSprite>();
        sprite.SpriteName = Prefix + element.ToLower();
    }
}
