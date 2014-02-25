using System;
using UnityEngine;

public class GUIObjectDefaultPopupContent : MonoBehaviour, IPopupContent
{
    private const string Prefab = "GUI/panel_popupcontent_default";
    private const string ButtonStr = "button_accept";
    public event Action ClosePopup;

    [SerializeField]
    private string _textkeyButton = "blindtext";
    private dfButton _buttonAccept;

    public string ButtonAccept
    {
        get { return _textkeyButton; }
        set
        {
            if (_buttonAccept == null)
            {
                GameObject obj = transform.Find(ButtonStr).gameObject;
                if (obj)
                    _buttonAccept = obj.GetComponent<dfButton>();
            }
            _textkeyButton = value;
            if (_buttonAccept != null)
                _buttonAccept.Text = Localization.GetText(value);
        }
    }

    public static GameObject Create(string textKeyText, string textKeyTitle, string ok)
    {
        GameObject go = Instantiate(Resources.Load<GameObject>(Prefab)) as GameObject;
        GUIObjectDefaultPopupContent challengeContent = go.GetComponent<GUIObjectDefaultPopupContent>();
        challengeContent.Init(textKeyText, textKeyTitle, ok);
        return go;
    }

    protected void Init(string textKeyText, string textKeyTitle, string ok)
    {
        GUIObjectTextPanel textPanel = GetComponent<GUIObjectTextPanel>();
        textPanel.Text = textKeyText;
        textPanel.Title = textKeyTitle;
        ButtonAccept = ok;

    }
}
