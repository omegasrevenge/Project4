using System;
using UnityEngine;
using System.Collections;

public class GUIObjectPopup : MonoBehaviour
{
    private const string Prefab = "GUI/panel_popup"; // should be implemented, if does not exist

    private const string PopupStr = "sprite_popup";
    private const string MessageSound = "Oc_Audio_SFX_Vengea_Message_IRIS_LAYOUT";

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
        return obj;
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
        _startTime = Time.time;
        if (StartCallback != null)
            StartCallback();
    }

    public void OnPopupEnd()
    {
        Destroy(gameObject);
        if (_nextPopup != null)
            _nextPopup.Show();
        if (Callback != null)
            Callback();
    }
}
