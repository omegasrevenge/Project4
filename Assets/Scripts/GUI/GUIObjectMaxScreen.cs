﻿using UnityEngine;
using System.Collections;

public class GUIObjectMaxScreen : MonoBehaviour
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
            _textkeyText = value;
            _textLabel.Text = Localization.GetText(value);
        }
    }

    public string Title
    {
        get { return _textkeyTitle; }
        set
        {
            if (_titleLabel == null)
            {
                GameObject obj = transform.FindChild(TitleLabelStr).gameObject;
                if (obj)
                    _titleLabel = obj.GetComponent<dfLabel>();
            }
            _textkeyTitle = value;
            _titleLabel.Text = Localization.GetText(value);
        }
    }
	
	void Update () {
	
	}
}