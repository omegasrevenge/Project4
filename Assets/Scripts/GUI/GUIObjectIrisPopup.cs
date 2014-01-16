using System;
using UnityEngine;
using System.Collections;

public class GUIObjectIrisPopup : MonoBehaviour 
{
    private const string PopupStr = "sprite_popup";
    private const string VisualizerStr = "panel_visualizer";
    private const string RepeatButtonStr = "button_repeat";
    private const string TextLabelStr = "label_text";

    private const float Delay = 1f;

    private string _textkeyText = "blindtext";

    public string Audio
    {
        get 
        { 
            if (_audio && _audio.clip) return _audio.clip.name;
            return "null";
        }
        set
        {
            _audio.clip = Localization.GetSound(value);
            Debug.Log(_audio.clip);
        }
    }

    private dfLabel _textLabel;
    private dfButton _repeat;
    private AudioSource _messageSound;
    private AudioSource _audio;
    private bool _repeatEnabled = false;

    public event Action ShowRepeat;
    public event Action HideRepeat;

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
        _messageSound.Play();

        _audio = gameObject.AddComponent<AudioSource>();

        Transform popUpTrnsf = transform.FindChild(PopupStr);

        if (popUpTrnsf != null)
        {
            GameObject obj = popUpTrnsf.FindChild(TextLabelStr).gameObject;
            if (obj)
                _textLabel = obj.GetComponent<dfLabel>();

            obj = popUpTrnsf.FindChild(VisualizerStr).gameObject;
            if (obj)
                obj.GetComponent<GUIObjectVisualizer>().Audio = _audio;


            obj = popUpTrnsf.FindChild(RepeatButtonStr).gameObject;
            if (obj)
            {
                _repeat = obj.GetComponent<dfButton>();
                _repeat.Disable();
                _repeat.Click += (dfControl control, dfMouseEventArgs mouseEvent) =>
                {
                    if (mouseEvent.Used) return;
                    mouseEvent.Use();
                    PlaySound();
                    _repeat.Disable();
                    if (HideRepeat != null)
                        HideRepeat();
                };
            }
        }
        

    }

    void Update()
    {
        if (_audio == null || _audio.clip == null)
            return;
        if (!_repeatEnabled && _audio.isPlaying)
            _repeatEnabled = true;
        
        else if (_repeatEnabled && !_audio.isPlaying && !_repeat.IsEnabled)
        {
            _repeat.Enable();
            if (ShowRepeat != null)
                ShowRepeat();
        }
    }

    public void PlaySound()
    {
        
        _audio.PlayDelayed(Delay);
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
