using UnityEngine;
using System.Collections;

public class GUIObjectTextPanel : MonoBehaviour
{
    private const string TextLabelStr = "label_text";
    private const string TitleLabelStr = "label_title";

    [SerializeField]
    private string _textkeyText = "blindtext";
    [SerializeField]
    private string _textkeyTitle = "blindtext";

    [SerializeField]
    private dfLabel _textLabel;
    [SerializeField]
    private dfLabel _titleLabel;

    public string Text
    {
        get { return _textkeyText; }
        set
        {
            if (_textLabel == null)
            {
                GameObject obj = transform.FindChild(TextLabelStr).gameObject;
                if (obj)
                    _textLabel = obj.GetComponent<dfLabel>();
            }
            string[] text = new string[] {value};
           
            if (value.Contains("#"))
                text = value.Split('#');
            _textkeyText = text[0];
            if (_textLabel != null)
            {              
                string textinfo = Localization.GetText(text[0]);
                _textLabel.Text = textinfo;
                string[] newText = new string[text.Length-1];
                if (text.Length > 1)
                {
                    for (int i = 0; i < newText.Length; i++)
                        newText[i] = text[i + 1];
                    _textLabel.Text = string.Format(textinfo, newText);
                }        
            }
        }
    }

    public string Title
    {
        get { return _textkeyTitle; }
        set
        {
            if (_titleLabel == null)
            {
                Transform trans = transform.FindChild(TitleLabelStr);
                GameObject obj;
                if (trans)
                {
                    obj = trans.gameObject;
                    if (obj)
                    _titleLabel = obj.GetComponent<dfLabel>();
                }
                
            }
    
            _textkeyTitle = value;
            if (_titleLabel != null)
                _titleLabel.Text = Localization.GetText(value);
        }
    }
}
