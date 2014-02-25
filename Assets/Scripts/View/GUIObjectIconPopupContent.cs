using UnityEngine;

public class GUIObjectIconPopupContent : GUIObjectDefaultPopupContent 
{
    private const string Prefab = "GUI/panel_popupcontent_icon";
    private const string IconStr = "panel_icon";
    private const string SpriteStr = "sprite";

    public static GameObject Create(string textKeyText, string textKeyTitle, string ok, string icon)
    {
        GameObject go = Instantiate(Resources.Load<GameObject>(Prefab)) as GameObject;
        GUIObjectIconPopupContent content = go.GetComponent<GUIObjectIconPopupContent>();
        content.Init(textKeyText, textKeyTitle, ok);
        content.AddIcon(icon);
        return go;
    }

    private void AddIcon(string icon)
    {
        dfControl control = transform.Find(IconStr).GetComponent<dfControl>();
        dfSprite sprite = control.transform.Find(SpriteStr).GetComponent<dfSprite>();
        sprite.SpriteName = icon;

        float scale = control.Size.y/sprite.SpriteInfo.sizeInPixels.y;
        Vector2 size = sprite.SpriteInfo.sizeInPixels;
        size *= scale;
        sprite.Size = size;
        sprite.RelativePosition = new Vector3((control.Size.x - sprite.Size.x)/2f,0f,0f);

    }
}
