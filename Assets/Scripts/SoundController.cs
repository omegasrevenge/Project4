  using System.Collections.Generic;
  using UnityEngine;
using System.Collections;

public class SoundController : MonoBehaviour
{
    public const string ChannelSFX = "channelSFX";

    private const float LoadTimeOut = 2f;
    private static SoundController _instance;

    private Dictionary<string, AudioSource> _audio; 

    public static SoundController Singleton
    {
        get
        {
            if (_instance != null)
                return _instance;
            return null;
        }
    }

    public static SoundController Create()
    {
        if (_instance != null)
            return _instance;
        GameObject obj = new GameObject("controller_soundcontroller")
        {
            hideFlags = HideFlags.DontSave,
            tag = GameManager.DontSaveTag
        };
        _instance = obj.AddComponent<SoundController>();
        return _instance;
    }

    public void Init(params string[] preloadedSounds)
    {
        _audio = new Dictionary<string, AudioSource>();
        gameObject.AddComponent<AudioListener>();

        AudioSource source;
        foreach (string sound in preloadedSounds)
        {
            source = _audio[sound] = gameObject.AddComponent<AudioSource>();
            source.clip = Resources.Load<AudioClip>("Sounds/" + sound);
        }
    }

    public static void PlaySound(string filename, bool stream = false, string sharedAudioChannel = "none", float delay=0)
    {
        if(!Singleton)
            return;

        bool loadClip = true;
        if (sharedAudioChannel == "none")
        {
            sharedAudioChannel = filename;
            loadClip = false;
        }

        AudioSource source = GetChannel(sharedAudioChannel);

        if (stream)
            Singleton.StartCoroutine(Singleton.StreamSound(filename, source));
        else
        {
            if (loadClip || source.clip == null)
                source.clip = Resources.Load<AudioClip>("Sounds/"+filename);
            if(delay>0)
                source.PlayDelayed(delay);
            else
                source.Play();
        }


    }

    public static AudioSource LoadAudioClip(string filename, string sharedAudioChannel = "none")
    {
        if (!Singleton)
            return null;

        bool loadClip = true;
        if (sharedAudioChannel == "none")
        {
            sharedAudioChannel = filename;
            loadClip = false;
        }

        AudioSource source = GetChannel(sharedAudioChannel);


        if (loadClip || source.clip == null)
            source.clip = Resources.Load<AudioClip>("Sounds/" + filename);
        return source;

    }

    public static AudioSource GetChannel(string audioChannel)
    {
        AudioSource source;
        if (!Singleton._audio.TryGetValue(audioChannel, out source))
            source = Singleton._audio[audioChannel] = Singleton.gameObject.AddComponent<AudioSource>();
        return source;
    }

    public static void RemoveChannel(string audioChannel)
    {
        AudioSource source;
        if (Singleton._audio.TryGetValue(audioChannel, out source))
        {
           Destroy(source);
        }
    }

    private IEnumerator StreamSound(string filename, AudioSource source)
    {

        string url = Application.streamingAssetsPath + "/Sounds/" + filename + ".mp3";
        WWW file = new WWW(url);
        yield return file;
        AudioClip audio = file.GetAudioClip(false, true);
        float time = Time.time;
        while (!audio.isReadyToPlay && (Time.time-time)<=LoadTimeOut)
        {
            yield return new WaitForEndOfFrame();
        }
        source.clip = audio;
        source.Play();
    }
}
