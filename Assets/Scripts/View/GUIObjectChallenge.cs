using System;
using UnityEngine;
using System.Collections;

public class GUIObjectChallenge : MonoBehaviour, IPopupContent
{
    private const string Prefab = "GUI/panel_challengepanel";
    private const string ButtonStr = "button_accept";
    private const string ButtonStrCncl = "button_decline";
    private const string TextboxStr = "textbox_name";

    public event Action ClosePopup;

    private dfControl _root;

    [SerializeField] private string _textkeyButtonA = "blindtext";
    [SerializeField] private string _textkeyButtonD = "blindtext";

    [SerializeField]
    private dfButton _buttonAccept;
    [SerializeField]
    private dfButton _buttonDecline;

    public string ButtonAccept
    {
        get { return _textkeyButtonA; }
        set
        {
            if (_buttonAccept == null)
            {
                GameObject obj = transform.FindChild(ButtonStr).gameObject;
                if (obj)
                    _buttonAccept = obj.GetComponent<dfButton>();
            }
            _textkeyButtonA = value;
            if (_buttonAccept != null)
                _buttonAccept.Text = Localization.GetText(value);
        }
    }

    public string ButtonDecline
    {
        get { return _textkeyButtonD; }
        set
        {
            if (_buttonDecline == null)
            {
                GameObject obj = transform.FindChild(ButtonStr).gameObject;
                if (obj)
                    _buttonDecline = obj.GetComponent<dfButton>();
            }
            _textkeyButtonD = value;
            if (_buttonDecline != null)
                _buttonDecline.Text = Localization.GetText(value);
        }
    }

    public static GUIObjectChallenge Create(dfControl root, string textKeyText, string textKeyTitle, string name, string cancel, string ok)
    {
        dfControl cntrl = root.AddPrefab(Resources.Load<GameObject>(Prefab));
        cntrl.Size = cntrl.Parent.Size;
        cntrl.RelativePosition = Vector2.zero;
        GUIObjectChallenge obj = cntrl.GetComponent<GUIObjectChallenge>();
        obj._root = root;
        obj.ButtonAccept = ok;
        obj.ButtonDecline = cancel;

        if (ok == "") Destroy(obj._buttonAccept);
        if (cancel == "") Destroy(obj._buttonDecline);

        GUIObjectTextPanel panel = obj.GetComponent<GUIObjectTextPanel>();
        panel.Text = textKeyText +"#"+name;
        panel.Title = textKeyTitle;

        return obj;
    }

    public void Accept()
    {
        Debug.Log("accepted");
        GameManager.Singleton.Accept();
    }

    public void Decline()
    {
        Debug.Log("declined");
        GameManager.Singleton.Decline();
    }
}