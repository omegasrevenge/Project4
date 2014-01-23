using UnityEngine;
using System.Collections;

public class GUIObjectNameInput : MonoBehaviour 
{
    private const string ButtonStr = "button_submit";

    [SerializeField]
    private string _textkeyDefault = "blindtext";
    [SerializeField]
    private string _textkeyButton = "blindtext";

    [SerializeField]
    private dfTextbox _textBox;
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
}
