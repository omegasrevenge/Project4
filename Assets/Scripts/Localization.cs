using System;
using UnityEngine;

public class Localization
{
    private static Localization _instance;
    private JSONObject _strings;
    private int _langID;

    public static Localization Singleton
    {
        get
        {
            if (_instance == null)
                return new Localization();
            else
                return _instance;
        }
    }

    private Localization()
    {
        if (_instance == null)
        {
            _instance = this;
            Init();
        }
        else
            Debug.LogError("Second Instance of Localization.");
    }

    private void Init()
    {
        TextAsset asset = Resources.Load<TextAsset>("strings");
        _strings = JSONParser.parse(asset.text);
        JSONObject languages =  _strings["lang"];
        for (int i = 0; i < languages.Count; i++)
        {
            string language = (string) languages[i];
            if (language == Application.systemLanguage.ToString())
            {
                _langID = i;
                break;
            }
        }
    }

    public static string Get(string textKey)
    {
        return (string)Singleton._strings[textKey][Singleton._langID];
    }
}
