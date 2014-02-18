using System;
using UnityEngine;
using System.Collections;

public class GUIObjectResourceResult : MonoBehaviour, IPopupContent
{
    private const string Prefab = "GUI/panel_rsc_result";
    private const string ButtonStr = "button_ok";
    private const string TextboxStr = "textbox_name";
    private const string ResourceStr = "sprite_rsc";
    private const string Prefix = "element_";

    public event Action ClosePopup;

    private dfControl _root;

    [SerializeField] private string _textkeyButton = "blindtext";

    [SerializeField] private dfButton _button;

    [SerializeField] private dfSprite[] _rsc;

    [SerializeField] public string Text;

    public string Button
    {
        get { return _textkeyButton; }
        set
        {
            if (_button == null)
            {
                GameObject obj = transform.FindChild(ButtonStr).gameObject;
                if (obj)
                    _button = obj.GetComponent<dfButton>();
            }
            _textkeyButton = value;
            if (_button != null)
                _button.Text = Localization.GetText(value);
        }
    }

    public static GUIObjectResourceResult Create(dfControl root, string textKeyText, string textKeyButton, string[] count, string[] level, string[] element)
    {
        SoundController.PlaySound(SoundController.SoundChoose, SoundController.ChannelSFX);
        dfControl cntrl = root.AddPrefab(Resources.Load<GameObject>(Prefab));
        cntrl.Size = cntrl.Parent.Size;
        cntrl.RelativePosition = Vector2.zero;
        GUIObjectResourceResult obj = cntrl.GetComponent<GUIObjectResourceResult>();
        obj._root = root;

        obj.Button = textKeyButton;
        GUIObjectTextPanel[] panels = obj.GetComponentsInChildren<GUIObjectTextPanel>();
        for (int i = 0; i < count.Length; i++)
        {          
            panels[i].Text = textKeyText + "#" + count[i] + "#" + Localization.GetText(level[i]) + "#" + Localization.GetText(element[i]);
            obj._rsc[i].SpriteName = Prefix + element[i].ToLower();
        }
        for (int i = count.Length; i < panels.Length; i++)
        {
            Destroy(obj._rsc[i].gameObject);
            Destroy(panels[i].gameObject);
        }
        return obj;
    }
}