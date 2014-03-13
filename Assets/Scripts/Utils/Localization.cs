using System;
using UnityEngine;

public class Localization
{
    public const string Fallback = "de";

    private static Localization _instance;
    private string _language = "de";
    private string _location = "Iris/";
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
            return "Missing textkey for " + textKey + ".";
        }
        return (string)Singleton._strings[textKey];
    }

    public static AudioSource GetSound(string soundKey, string channel = "none")
    {
        AudioSource source = SoundController.LoadAudioClip(Singleton._location + soundKey + "_" + Singleton._language, channel);
        if (source != null)
            return source;
        source = SoundController.LoadAudioClip(Singleton._location + soundKey + "_en", channel);
        if (source != null)
            return source;
        source = SoundController.LoadAudioClip(Singleton._location + soundKey + "_de", channel);
        if (source != null)
            return source;
        return SoundController.LoadAudioClip(Singleton._location + soundKey, channel);
    }
}
