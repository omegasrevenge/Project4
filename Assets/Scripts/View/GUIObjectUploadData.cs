using System;
using UnityEngine;
using System.Collections;

public class GUIObjectUploadData : MonoBehaviour, IPopupContent
{
    private const string Prefab = "GUI/panel_uploaddata";
    private const string ButtonStrCncl = "button_decline";
    private const float DestructTime = 4;
    
    public event Action ClosePopup;

    [SerializeField] private string _textkeyButton = "blindtext";
    [SerializeField] private dfButton _buttonDecline;

    private float destructTimer;

    public string ButtonDecline
    {
        get { return _textkeyButton; }
        set
        {
            if (_buttonDecline == null)
            {
                GameObject obj = transform.FindChild(ButtonStrCncl).gameObject;
                if (obj)
                    _buttonDecline = obj.GetComponent<dfButton>();
            }
            _textkeyButton = value;
            if (_buttonDecline != null)
                _buttonDecline.Text = Localization.GetText(value);

            if (GameManager.Singleton.Player.CurrentFaction == Player.Faction.NCE)
                _buttonDecline.TextColor = GameManager.Black;
        }
    }

    public static GameObject Create(string textKeyText, string textKeyTitle, string cancel)
    {
        GameObject go = Instantiate(Resources.Load<GameObject>(Prefab)) as GameObject;
        GUIObjectUploadData challengeContent = go.GetComponent<GUIObjectUploadData>();

        challengeContent.ButtonDecline = cancel;

        if (cancel == "") Destroy(challengeContent._buttonDecline);

        GUIObjectTextPanel panel = challengeContent.GetComponent<GUIObjectTextPanel>();
        panel.Text = textKeyText;
        panel.Title = textKeyTitle;

        return go;
    }

    public void Decline(dfControl control, dfMouseEventArgs args)
    {
        GameManager.Singleton.SetFaction(Player.Faction.NCE);
    }

    void Update()
    {
        destructTimer += Time.deltaTime;
        if (destructTimer >= DestructTime)
            transform.parent.parent.GetComponent<GUIObjectPopup>().OnPopupEnd();
    }
}
