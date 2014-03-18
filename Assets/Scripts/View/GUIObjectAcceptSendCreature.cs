using System;
using UnityEngine;
using System.Collections;

public class GUIObjectAcceptSendCreature : MonoBehaviour, IPopupContent
{
    private const string Prefab = "GUI/panel_sendcrtoven";
    private const string ButtonStr = "button_accept";
    private const string ButtonStrCncl = "button_decline";

    public event Action ClosePopup;

    [SerializeField] private string _textkeyButtonA = "blindtext";
    [SerializeField] private string _textkeyButtonD = "blindtext";

    private Creature creature;

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

    public static GameObject Create(string textKeyText, string textKeyTitle, Creature cr, string cancel, string ok)
    {
        GameObject go = Instantiate(Resources.Load<GameObject>(Prefab)) as GameObject;
        GUIObjectAcceptSendCreature content = go.GetComponent<GUIObjectAcceptSendCreature>();

        content.ButtonAccept = ok;
        content.ButtonDecline = cancel;

        if (ok == "") Destroy(content._buttonAccept);
        if (cancel == "") Destroy(content._buttonDecline);

        GUIObjectTextPanel panel = content.GetComponent<GUIObjectTextPanel>();
        content.creature = cr;
        panel.Text = textKeyText +"#"+cr.Name;
        panel.Title = textKeyTitle;

        return go;
    }

    public void Accept(dfControl control, dfMouseEventArgs args)
    {
        GameManager.Singleton.SendCreatureToVengea(creature.CreatureID);
    }

    public void Decline(dfControl control, dfMouseEventArgs args)
    {
        
    }
}