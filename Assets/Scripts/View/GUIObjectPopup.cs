using System;
using UnityEngine;

public class GUIObjectPopup : MonoBehaviour
{
    private const string Prefab = "GUI/panel_popup"; // should be implemented, if does not exist

    private const string PopupStr = "sprite_popup";
    private const string MessageSound = "Oc_Audio_SFX_Vengea_Message_IRIS_LAYOUT";

    private GameObject _content;
    private dfControl _control;
    private dfControl _root;
    private float _delay = 1f;
    private bool _startedPlaying = false;

    private GUIObjectPopup _nextPopup;
    private bool _playMessageSound = true;

    public event Action ShowPopup;
    public event Action HidePopup;
    public event Action ShowButtons;
    public event Action HideButtons;

    public Action Callback;
    public Action StartCallback;

    private float _startTime;

    public static GUIObjectPopup Create(dfControl root)
    {
        dfControl cntrl = root.AddPrefab(Resources.Load<GameObject>(Prefab));
        cntrl.Size = cntrl.Parent.Size;
        cntrl.RelativePosition = Vector2.zero;
        GUIObjectPopup obj = cntrl.GetComponent<GUIObjectPopup>();
        obj._root = root;
        obj._control = cntrl;
        return obj;
    }

    public GUIObjectPopup AddToPopup(GameObject content)
    {
        if (_content != null)
            Destroy(_content);
        if (content == null)
            return this;

        if (content.GetComponent<dfControl>() == null)
        {
            throw new InvalidCastException();
        }
        content.transform.parent = transform;
        content.layer = gameObject.layer;

        var child = content.GetComponent<dfControl>();
        _control.AddControl(child);
        child.Size = _control.Size;
        child.RelativePosition = Vector2.zero;

        child.BringToFront();

        _content = content;

        return this;
    }

    public GUIObjectPopup AddPopup()
    {
        _nextPopup = Create(_root);
        _nextPopup._playMessageSound = false;
        _nextPopup._delay = 0.1f;
        return _nextPopup;
    }

    private void Awake()
    {
        Transform popUpTrnsf = transform.FindChild(PopupStr);
    }

    public GUIObjectPopup Show()
    {
        if (_playMessageSound)
            SoundController.PlaySound(MessageSound, SoundController.ChannelSFX);
        if (ShowPopup != null)
            ShowPopup();
        return this;
    }

    void Update()
    {
        if (!_startedPlaying)
        {
            _startedPlaying = true;
        }

        else if (_startedPlaying)
        {
            //OnShowButtons();
        }
    }

    public void OnPopupStart()
    {
        _root.Click += (control, @event) =>
        {
            SoundController.PlaySound(SoundController.SoundClick, SoundController.ChannelSFX);
            if (HidePopup != null) HidePopup();
        };
        _startTime = Time.time;      
        if (_content != null) 
            _content.GetComponent<dfControl>().Show();
        if (StartCallback != null)
            StartCallback();
    }

    public void OnPopupEnd()
    {
        GameManager.Singleton.lastPlayerRequest = "none";
        Destroy(gameObject);
        if (_nextPopup != null)
            _nextPopup.Show();
        if (Callback != null)
            Callback();
    }
}
