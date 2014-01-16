using System;
using UnityEngine;

public class Localization
{
    public const string Fallback = "de";

    private static Localization _instance;
    private string _language = "de";
    private JSONObject _strings;

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
# if !UNITY_EDITOR
        AndroidJavaObject locale = new AndroidJavaClass("java/util/Locale").CallStatic<AndroidJavaObject>("getDefault");
        _language = locale.Call<string>("getLanguage");
#endif
        string loadedLang = _language;

        TextAsset asset = Resources.Load<TextAsset>("Localization/strings-" + _language);
        if (asset == null)
        {
            asset = Resources.Load<TextAsset>("Localization/strings-"+Fallback);
            if (asset == null)
            {
                Debug.LogError("No localization file.");
                return;
            }
            loadedLang = "en";
        }


        _strings = JSONParser.parse(asset.text);
        Debug.Log("System Language: "+_language+", Loaded Language: "+loadedLang);
    }

    public static string GetText(string textKey)
    {
        if (Singleton._strings[textKey] == null)
        {
            Debug.LogError("Missing textkey for " + textKey + ".");
            return textKey;
        }
        return (string)Singleton._strings[textKey];
    }

    public static AudioClip GetSound(string soundKey)
    {
        AudioClip clip =  Resources.Load<AudioClip>("Sounds/" + soundKey +"-"+ Singleton._language);
        if (clip != null)
            return clip;
        clip = Resources.Load<AudioClip>("Sounds/" + soundKey + "-en");
        if (clip != null)
            return clip;
        clip = Resources.Load<AudioClip>("Sounds/" + soundKey + "-de");
        if (clip != null)
            return clip;
        return Resources.Load<AudioClip>("Sounds/" + soundKey);
    }
}
