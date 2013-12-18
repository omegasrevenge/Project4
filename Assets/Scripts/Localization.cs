using System;
using UnityEngine;

public class Localization
{
    private static Localization _instance;
    private JSONObject _strings;
    private JSONObject _sounds;
    private int _textID;
    private int _soundID;

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
            string language = (string)languages[i];
            if (language == Application.systemLanguage.ToString())
            {
                _textID = i;
                break;
            }
        }

        asset = Resources.Load<TextAsset>("sounds");
        _sounds = JSONParser.parse(asset.text);
        languages = _sounds["lang"];
        for (int i = 0; i < languages.Count; i++)
        {
            string language = (string)languages[i];
            if (language == Application.systemLanguage.ToString())
            {
                _soundID = i;
                break;
            }
        }

    }

    public static string GetText(string textKey)
    {
        if (Singleton._strings[textKey] == null)
        {
            Debug.LogError("Missing textkey for "+textKey+".");
            return textKey;
        }
        if (Singleton._textID >= Singleton._strings[textKey].Count)
        {
            Debug.LogError("Missing translation for " + textKey + ".");
            if (Singleton._strings[textKey].Count == 0)
                return textKey;
            return (string)Singleton._strings[textKey][0];
        }
        return (string)Singleton._strings[textKey][Singleton._textID];
    }

    public static AudioClip GetSound(string soundKey)
    {
        if (Singleton._sounds[soundKey] == null)
        {
            Debug.LogError("Missing sound for " + soundKey + ".");
            return null;
        }
        if (Singleton._soundID >= Singleton._sounds[soundKey].Count)
        {
            if (Singleton._sounds[soundKey].Count == 0)
            {
                Debug.LogError("Missing sound for " + soundKey + ".");
                return null;
            }      
            return Resources.Load<AudioClip>("Sounds/" + (string)Singleton._sounds[soundKey][0]);

        }
        Debug.Log(Resources.Load<AudioClip>("Sounds/hello-de"));
        Debug.Log("Sounds/" + (string)Singleton._sounds[soundKey][Singleton._soundID]);
        return Resources.Load<AudioClip>("Sounds/" + (string)Singleton._sounds[soundKey][Singleton._soundID]);
    }
}
