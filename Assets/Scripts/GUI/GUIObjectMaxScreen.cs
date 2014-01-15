using UnityEngine;
using System.Collections;

public class GUIObjectMaxScreen : MonoBehaviour
{

    private const string TextLabelStr = "label_text";
    private const string TitleLabelStr = "label_title";

    private string _textkeyText = "blindtext";
    private string _textkeyTitle = "blindtext";

    private dfLabel _textLabel;
    private dfLabel _titleLabel;

    public string Text
    {
        get { return _textkeyText; }
        set
        {
            Debug.Log(value);
            _textkeyText = value;
            _textLabel.Text = Localization.GetText(value);
        }
    }

    public string Title
    {
        get { return _textkeyTitle; }
        set
        {
            Debug.Log(value);
            _textkeyTitle = value;
            _titleLabel.Text = Localization.GetText(value);
        }
    }

	void Awake ()
	{
	    GameObject obj = transform.FindChild(TextLabelStr).gameObject;
	    if (obj)
	        _textLabel = obj.GetComponent<dfLabel>();

        obj = transform.FindChild(TitleLabelStr).gameObject;
        if (obj)
            _titleLabel = obj.GetComponent<dfLabel>();
	}
	
	void Update () {
	
	}
}
