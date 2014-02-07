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

    [SerializeField] private dfSprite _rsc;

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

    public static GUIObjectResourceResult Create(dfControl root, string textKeyText, string textKeyButton, string rsc)
    {
        dfControl cntrl = root.AddPrefab(Resources.Load<GameObject>(Prefab));
        cntrl.Size = cntrl.Parent.Size;
        cntrl.RelativePosition = Vector2.zero;
        GUIObjectResourceResult obj = cntrl.GetComponent<GUIObjectResourceResult>();
        obj._root = root;

        obj.Button = textKeyButton;
        GUIObjectTextPanel panel = obj.GetComponent<GUIObjectTextPanel>();
        panel.Text = textKeyText;

        obj._rsc.SpriteName = Prefix + rsc.ToLower();
        return obj;
    }
}