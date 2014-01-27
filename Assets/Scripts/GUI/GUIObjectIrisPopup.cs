using System;
using UnityEngine;
using System.Collections;

public class GUIObjectIrisPopup : MonoBehaviour
{
    private const string Prefab = "GUI/panel_irispopup";

    private const string PopupStr = "sprite_popup";
    private const string VisualizerStr = "panel_visualizer";
    private const string RepeatButtonStr = "button_repeat";
    private const string OKButtonStr = "button_ok";
    private const string TextLabelStr = "label_text";
    private const string ContinueTextkey = "continue";
    private const string MessageSound = "Oc_Audio_SFX_Vengea_Message_IRIS_LAYOUT";



    private string _textkeyText = "blindtext";
    private string _textkeyButton = "ok";

    private dfLabel _textLabel;
    private dfButton _repeat;
    private dfButton _ok;
    private AudioSource _audio;
    private GUIObjectVisualizer _visualizer;
    private dfControl _root;
    private float _delay = 1f;
    private bool _startedPlaying = false;

    private GUIObjectIrisPopup _nextPopup;
    private bool _playMessageSound = true;

    public event Action ShowPopup;
    public event Action HidePopup;
    public event Action ShowButtons;
    public event Action HideButtons;

    public Action Callback;
    public Action StartCallback;

    private float _startTime;

    public string Text
    {
        get { return _textkeyText; }
        set
        {
            _textkeyText = value;
            _textLabel.Text = Localization.GetText(value);
        }
    }

    public string Audio
    {
        get
        {
            if (_audio && _audio.clip) return _audio.clip.name;
            return "null";
        }
        set
        {
            _audio = Localization.GetSound(value);
            _visualizer.Audio = _audio;
        }
    }

    public string ButtonText
    {
        get { return _textkeyButton; }
        set
        {
            _textkeyButton = value;
            _ok.Text = Localization.GetText(value);
        }
    }


    public static GUIObjectIrisPopup Create(dfControl root, string textKeyText, string audioKey, string textKeyButton ="ok")
    {
        dfControl cntrl = root.AddPrefab(Resources.Load<GameObject>(Prefab));
        cntrl.Size = cntrl.Parent.Size;
        cntrl.RelativePosition = Vector2.zero;
        GUIObjectIrisPopup obj = cntrl.GetComponent<GUIObjectIrisPopup>();
        obj._root = root;
        obj.Text = textKeyText;
        obj.Audio = audioKey;
        return obj;
    }

    public GUIObjectIrisPopup AddIrisPopup(string textKeyText, string audioKey, string textKeyButton = "ok")
    {
        _nextPopup = Create(_root, textKeyText, audioKey, textKeyButton);
        _nextPopup._playMessageSound = false;
        _nextPopup._delay = 0.1f;
        ButtonText = ContinueTextkey;
        return _nextPopup;
    }

    void Awake()
    {
        Transform popUpTrnsf = transform.FindChild(PopupStr);

        if (popUpTrnsf != null)
        {
            GameObject obj = popUpTrnsf.FindChild(TextLabelStr).gameObject;
            if (obj)
            {
                _textLabel = obj.GetComponent<dfLabel>();
                Text = _textkeyText;
            }

            obj = popUpTrnsf.FindChild(VisualizerStr).gameObject;
            if (obj)
                _visualizer = obj.GetComponent<GUIObjectVisualizer>();


            obj = popUpTrnsf.FindChild(RepeatButtonStr).gameObject;
            if (obj)
            {
                _repeat = obj.GetComponent<dfButton>();
                _repeat.Click += (dfControl control, dfMouseEventArgs mouseEvent) =>
                {
                    if (mouseEvent.Used) return;
                    mouseEvent.Use();
                   
                    PlaySound();
                    
                    OnHideButtons();

                };
            }    

            obj = popUpTrnsf.FindChild(OKButtonStr).gameObject;
            if (obj)
            {
                _ok = obj.GetComponent<dfButton>();
                ButtonText = _textkeyButton;
                _ok.Click += (dfControl control, dfMouseEventArgs mouseEvent) =>
                {
                    if (mouseEvent.Used) return;
                    mouseEvent.Use();

                    if (HidePopup != null)
                        HidePopup();

                };
            }            
            
        }
        

    }

    public GUIObjectIrisPopup Show()
    {
        if (_playMessageSound)
            SoundController.PlaySound(MessageSound, false, SoundController.ChannelSFX);
        if (ShowPopup != null)
            ShowPopup();
        return this;
    }

    void Update()
    {
        if (_audio == null || _audio.clip == null)
            return;
        if (!_startedPlaying && _audio.isPlaying)
        {
            _startedPlaying = true;
        }
        
        else if (_startedPlaying && !_audio.isPlaying && !_repeat.IsEnabled)
        {
            OnShowButtons();

        }
    }

    public void OnPopupStart()
    {
        _startTime = Time.time;
        if (StartCallback != null)
            StartCallback();
        PlaySound();
    }

    public void OnPopupEnd()
    {
        Destroy(gameObject);
        if(_nextPopup != null)
            _nextPopup.Show();
        if (Callback != null)
            Callback();
        SoundController.RemoveChannel(Audio);
    }

    private void PlaySound()
    {       
        _audio.PlayDelayed(_delay-(Time.time-_startTime));
        //StartCoroutine(LoadAndPlay());
    }

    private void OnHideButtons()
    {
        _repeat.Disable();
        _ok.Disable();
        if (HideButtons != null)
            HideButtons();
    }

    private void OnShowButtons()
    {
        _repeat.Enable();
        _ok.Enable();
        if (ShowButtons != null)
        {
            ShowButtons();
        }
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
