using System;
using UnityEngine;
using System.Collections;

public class GUIObjectMessage : MonoBehaviour 
{
    private const string Prefab = "GUI/panel_message"; // should be implemented, if does not exist
    private const string MessageStrSpr = "message_sprite_counter";
    //private const string MessageSound = "Oc_Audio_SFX_Vengea_Message_IRIS_LAYOUT";

    private GameObject _content;
    private dfControl _control;
    private dfControl _root;
    private int count = 1;
    private float _delay = 1f;
    private bool _startedPlaying = false;

    private bool _playMessageSound = true;

    public event Action ShowPopup;
    public event Action HidePopup;
    public event Action ShowButtons;
    public event Action HideButtons;

    public Action Callback;
    public Action StartCallback;

    public dfSprite messageCountSprite;
    public dfLabel messageCounter;
    public dfButton button;

    private float _startTime;

    public static GUIObjectMessage Create(dfControl root)
    {
        GUIObjectMessage message = root.transform.GetComponentInChildren<GUIObjectMessage>();
        if (message != null)
        {
            message.IncreaseMessageCount();
            return message;
        }
        dfControl cntrl = root.AddPrefab(Resources.Load<GameObject>(Prefab));
        cntrl.Size = cntrl.Parent.Size;
        cntrl.RelativePosition = Vector2.zero;
        GUIObjectMessage obj = cntrl.GetComponent<GUIObjectMessage>();
        obj._root = root;
        obj._control = cntrl;
        obj.button.Click += (control, @event) =>
        {
            SoundController.PlaySound(SoundController.SoundClick, SoundController.ChannelSFX);
            if (obj.HidePopup != null) obj.HidePopup();
            else Destroy(obj.gameObject);
            GameManager.Singleton.ClickMessage();
        };

        return obj;
    }

    public GUIObjectMessage Show()
    {
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
        if (_content != null)
            _content.GetComponent<dfControl>().Show();
        if (StartCallback != null)
            StartCallback();
    }

    public void OnPopupEnd()
    {
        Destroy(gameObject);
    }

    public void IncreaseMessageCount()
    {
        count++;
        if (messageCountSprite == null || messageCounter == null)
           return;
        messageCountSprite.Show();
        messageCounter.Show();
        messageCounter.Text = count.ToString();
    }
}
