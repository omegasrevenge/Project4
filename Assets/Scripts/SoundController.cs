  using System.Collections.Generic;
  using UnityEngine;
using System.Collections;

public class SoundController : MonoBehaviour
{
    public const string ChannelSFX          = "channelSFX";
    public const string SFXlocation         = "SFX/Oc_Audio_SFX_";

    private const string Vengea              = "Vengea_";
    private const string NCE                 = "NCE_";

    // depends on faction
    public const string SoundFacChoose          = "Choose";
    public const string SoundFacClick           = "Click";
    public const string SoundFacError           = "Error";

    // fight --> BattleSounds ??
    public const string SoundFacFightCatch      = "Fight_Catch";
    public const string SoundFacFightDriod      = "Fight_Driode_"; //+number
    public const string SoundFacFightDeselect   = "Fight_Driode_Deselect"; 
    public const string SoundFacFightLose       = "Fight_Lose";
    public const string SoundFacFightVictory    = "Fight_Victory";
    
    public const string SoundFacLocate          = "Locate";
    public const string SoundFacMapClick        = "MapClick";
    public const string SoundFacMessageIRIS     = "Message_IRIS";
    public const string SoundFacMessageEnemy    = "Message_Enemy";
    public const string SoundFacUpgrade         = "Upgrade";

    // without faction
    public const string SoundCraftCombine = "Craft_Combine";
    public const string SoundCraftExchange = "Craft_Exchange";
    public const string SoundCraftPlace = "Craft_Place";
    public const string SoundCraftSplit= "Craft_Split";
    public const string SoundCraftTake = "Craft_Take";
    public const string SoundReboot = "Reboot";
    public const string SoundLogo = "Logo";

    private const float LoadTimeOut = 2f;
    private static SoundController _instance;

    private string _faction;

    [SerializeField]
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

    public static string Faction
    {
        get
        {
            if (GameManager.Singleton.Player.CurrentFaction == Player.Faction.NCE)
                Singleton._faction = NCE;
            else
                Singleton._faction = Vengea;
            return Singleton._faction;
        }
    }

    public static bool Enabled
    {
        get
        {
            return AudioListener.volume >= 0.5f;
        }
        set
        {
            if (value)
            {
                AudioListener.volume = 1f;
                PlayerPrefs.SetFloat("Sound",1f);
            }
            else
            {
                AudioListener.volume = 0f;
                PlayerPrefs.SetFloat("Sound", 0f);
            }
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
        AudioListener.volume = PlayerPrefs.GetFloat("Sound", 1f);

        AudioSource source;
        foreach (string sound in preloadedSounds)
        {
            source = _audio[sound] = gameObject.AddComponent<AudioSource>();
            source.clip = Resources.Load<AudioClip>(sound);
        }
    }

    public static AudioSource PlaySound(string filename, string sharedAudioChannel = "none", bool stream = false, float delay=0)
    {
        if(!Singleton)
            return null;

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
                source.clip = Resources.Load<AudioClip>("Sounds/" + filename);
            if(delay>0)
                source.PlayDelayed(delay);
            else
                source.Play();
        }
        return source;
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
	        Singleton._audio.Remove(audioChannel);
           Destroy(source);
        }
    }

    private IEnumerator StreamSound(string filename, AudioSource source)
    {

        string url = Application.streamingAssetsPath + "Sounds/" + filename + ".mp3";
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
