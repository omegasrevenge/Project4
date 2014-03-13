using System;
using UnityEngine;
using System.Collections;

public class GUIObjectNameInput : MonoBehaviour 
{
    private const string Prefab = "GUI/panel_nameinput";
    private const string ButtonStr = "button_submit";
    private const string TextboxStr = "textbox_name";

    [SerializeField]
    private string _textkeyDefault = "blindtext";
    [SerializeField]
    private string _textkeyButton = "blindtext";

    [SerializeField]
    private dfButton _button;

    [SerializeField] public string Text;

    public string Default
    {
        get { return _textkeyDefault; }
        set
        {
            _textkeyDefault = value;
            Text = Localization.GetText(value);
        }
    }

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

    void Awake()
    {
        GameObject obj = transform.FindChild(TextboxStr).gameObject;
        if (obj)
        {
            dfTextbox box = obj.GetComponent<dfTextbox>();
            box.Click +=
                (control, @event) =>
                {
                    SoundController.PlaySound(SoundController.SoundFacClick, SoundController.ChannelSFX);
                };
        }

        if (_button == null)
        {
            obj = transform.FindChild(ButtonStr).gameObject;
            if (obj)
            {
                _button = obj.GetComponent<dfButton>();
            }
        }
        if (_button != null)
        {
            _button.Click +=
                (control, @event) =>
                {
                    SoundController.PlaySound(SoundController.SoundFacClick, SoundController.ChannelSFX);
                };
        }
    }

    public static GameObject Create(string textKeyTitle, string textKeyText, string textKeyButton, string textKeyUsername, Action<string> callback)
    {
        GameObject go = Instantiate(Resources.Load<GameObject>(Prefab)) as GameObject;
        GUIObjectNameInput input = go.GetComponent<GUIObjectNameInput>();
        input.Button = textKeyButton;
        if(callback != null)
            input._button.Click += (control, @event) => { if(callback != null) callback(input.Text); };
        input.Default = textKeyUsername;

        GUIObjectTextPanel panel = go.GetComponent<GUIObjectTextPanel>();
        panel.Title = textKeyTitle;
        panel.Text = textKeyText;

        return go;
    }
}
