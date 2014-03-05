using System;
using UnityEngine;
using System.Collections;

public class GUIObjectChallenge : MonoBehaviour, IPopupContent
{
    private const string Prefab = "GUI/panel_challengepanel";
    private const string ButtonStr = "button_accept";
    private const string ButtonStrCncl = "button_decline";

    public event Action ClosePopup;

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

            if (GameManager.Singleton.Player.CurrentFaction == Player.Faction.NCE)
                _buttonAccept.TextColor = GameManager.Black;
        }
    }

    public string ButtonDecline
    {
        get { return _textkeyButtonD; }
        set
        {
            if (_buttonDecline == null)
            {
                GameObject obj = transform.FindChild(ButtonStrCncl).gameObject;
                if (obj)
                    _buttonDecline = obj.GetComponent<dfButton>();
            }
            _textkeyButtonD = value;
            if (_buttonDecline != null)
                _buttonDecline.Text = Localization.GetText(value);

            if (GameManager.Singleton.Player.CurrentFaction == Player.Faction.NCE)
                _buttonDecline.TextColor = GameManager.Black;
        }
    }

    public static GameObject Create(string textKeyText, string textKeyTitle, string name, string cancel, string ok)
    {
        GameObject go = Instantiate(Resources.Load<GameObject>(Prefab)) as GameObject;
        GUIObjectChallenge challengeContent = go.GetComponent<GUIObjectChallenge>();

        challengeContent.ButtonAccept = ok;
        challengeContent.ButtonDecline = cancel;

        if (ok == "") Destroy(challengeContent._buttonAccept);
        if (cancel == "") Destroy(challengeContent._buttonDecline);

        GUIObjectTextPanel panel = challengeContent.GetComponent<GUIObjectTextPanel>();
        panel.Text = textKeyText +"#"+name;
        panel.Title = textKeyTitle;

        return go;
    }

    public void Accept(dfControl control, dfMouseEventArgs args)
    {
        GameManager.Singleton.lastPlayerRequest = "none";
        GameManager.Singleton.Accept();
    }

    public void Decline(dfControl control, dfMouseEventArgs args)
    {
        GameManager.Singleton.lastPlayerRequest = "none";
        GameManager.Singleton.Decline();
    }
}