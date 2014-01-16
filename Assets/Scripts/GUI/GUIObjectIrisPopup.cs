using UnityEngine;
using System.Collections;

public class GUIObjectIrisPopup : MonoBehaviour 
{
    private const string PopupStr = "sprite_popup";
    private const string VisualizerStr = "panel_visualizer";
    private const string TextLabelStr = "label_text";

    private string _textkeyText = "blindtext";
    public string Audio = "";

    private dfLabel _textLabel;
    private AudioSource _messageSound;
    private AudioSource _audio;

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


    void Awake()
    {
        _messageSound = GetComponent<AudioSource>();
        _audio = gameObject.AddComponent<AudioSource>();

        GameObject obj = transform.FindChild(PopupStr).FindChild(TextLabelStr).gameObject;
        if (obj)
            _textLabel = obj.GetComponent<dfLabel>();

        obj = transform.FindChild(PopupStr).FindChild(VisualizerStr).gameObject;
        if (obj)
            obj.GetComponent<GUIObjectVisualizer>().Audio = _audio;

    }

    void Update()
    {

    }

    public void PlaySound()
    {
        _messageSound.Play();
        _audio.clip = Localization.GetSound(Audio);
        _audio.PlayDelayed(1f);
        //StartCoroutine(LoadAndPlay());
    }

    //private IEnumerator LoadAndPlay()
    //{
    //    string url = Application.streamingAssetsPath + "/Sounds/" + Audio + "-de.mp3";
    //    Debug.Log(url);
    //    WWW file = new WWW(url);
    //    yield return file;
    //    Debug.Log("FEHLER: "+file.error);
    //    AudioClip audio = file.GetAudioClip(false,true);
    //    while (!audio.isReadyToPlay)
    //    {
    //        yield return new WaitForEndOfFrame();
    //        //Debug.Log("Wait for it ...");
    //    }
    //    _audio.clip = audio;
    //    _audio.Play();
    //}
}
