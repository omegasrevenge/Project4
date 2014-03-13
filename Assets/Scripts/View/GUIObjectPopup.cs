using System;
using System.Collections.Generic;
using UnityEngine;

public class GUIObjectPopup : MonoBehaviour
{
    private const string Prefab = "GUI/panel_popup";
    private const string PopupStr = "sprite_popup";

    private static readonly Vector3 HiddenScale = new Vector3(0.2f,0.2f,0.2f);
    private static readonly Vector3 DefaultScale = new Vector3(1f,1f,1f);
    private const float AnimationTime = 0.3f;

    private dfSprite _popupRoot;
    
    private List<GameObject> _content;
    private Dictionary<GameObject, Action> _immediateCallbacks;
    private List<Action> _callbacks;
    private GameObject _currentContent;
  

    public event Action ShowPopup;
    public event Action HidePopup;
    public event Action ShowButtons;
    public event Action HideButtons;

    private bool _visible;
    private bool _active;

    public static GUIObjectPopup Create(dfControl root, GameObject content, bool stack = false, Action callback = null, bool startCallbackImmediatly = false)
    {
        dfControl cntrl = root.AddPrefab(Resources.Load<GameObject>(Prefab));
        cntrl.Size = cntrl.Parent.Size;
        cntrl.RelativePosition = Vector3.zero;
        GUIObjectPopup obj = cntrl.GetComponent<GUIObjectPopup>();
        obj.Init();
        obj.AddContent(content, stack, callback, startCallbackImmediatly);
        return obj;
    }

    private void Init()
    {
        _content = new List<GameObject>();
        _immediateCallbacks = new Dictionary<GameObject, Action>();
        _callbacks = new List<Action>();

        _popupRoot = transform.Find(PopupStr).GetComponent<dfSprite>();
        if (GameManager.Singleton.Player.CurrentFaction == Player.Faction.NCE)
            _popupRoot.Color = GameManager.NCERed;
    }

    public void AddContent(GameObject content, bool stack = false, Action callback = null, bool startCallbackImmediatly = false)
    {
        if(content == null) return;
        if(stack)
            _content.Insert(0,content);
        else
            _content.Add(content);

        if (callback != null)
        {
            if(startCallbackImmediatly)
                _immediateCallbacks.Add(content, callback);
            else
                _callbacks.Add(callback);
        }

        if (_content.Count > 0)
        {
            if(stack && _content.Count>1)
                PlayContentHideAnimation(false);
            else
            {
                ShowContent(_content[0]);
            }
        }
    }

    private void ShowContent(GameObject content)
    {
        if (content == null || content == _currentContent)
            return;

        dfControl child = content.GetComponent<dfControl>();
        if (child == null)
        {
            throw new InvalidCastException();
        }
        content.transform.parent = _popupRoot.transform;
        content.layer = _popupRoot.gameObject.layer;

        _popupRoot.AddControl(child);
        child.Size = _popupRoot.Size;
        child.RelativePosition = Vector2.zero;

        child.BringToFront();
        _currentContent = content;
        if (!_visible)
        {
            _visible = true;
            Show();
        }
        else
        {
            PlayContentShowAnimation();
        }
    }

    void OnClick(dfControl control, dfMouseEventArgs args)
    {
        if (!_active || args.Used) return;
        args.Use();

        SoundController.PlaySound(SoundController.SoundFacClick, SoundController.ChannelSFX);

        if (_content.Count > 1)
        {
            PlayContentHideAnimation();
        }
        else
        {
            Hide();
        }
    }

    /// <summary>
    /// Shows popup
    /// </summary>
    private void Show()
    {
        if (ShowPopup != null)
        {
            GameManager.Singleton.GUIDisableMenu();
            ShowPopup();
        }
    }

    private void Hide()
    {
        _active = false;
        if (HidePopup != null)
            HidePopup();
    }

    /// <summary>
    /// Executed when popup is shown in full size and opacity
    /// </summary>
    public void OnPopupStart()
    {
        _active = true;
    }

    /// <summary>
    /// Executed when popup is completly hidden
    /// </summary>
    public void OnPopupEnd()
    {
        GameManager.Singleton.GUIEnableMenu();
        Action callback;
        IrisMessage();
        if (_immediateCallbacks.TryGetValue(_currentContent, out callback))
        {
            if (callback != null)
                callback(); 
            _immediateCallbacks.Remove(_currentContent);
        }
        foreach (Action action in _callbacks)
        {
            if (action != null)
                action();
        }
        Destroy(gameObject);
    }

    private void IrisMessage()
    {
        if (_currentContent.GetComponent<GUIObjectResourceResult>() != null &&
            GameManager.Singleton.Player.InitSteps == 4)
        {
            GameManager.Singleton.GUICollectBiods_2();
            return;
        }
    }

    private void PlayContentShowAnimation()
    {
        if (!_currentContent)
            return;

        dfTweenVector3 tween = _currentContent.GetComponent<dfTweenVector3>();
        if (tween)
        {
            tween.Stop();
            Destroy(tween);
        }
        tween = _currentContent.AddComponent<dfTweenVector3>();
        tween.Target = new dfComponentMemberInfo()
        {
            Component = _currentContent.transform,
            MemberName = "localScale"
        };
        tween.StartValue = HiddenScale;
        tween.EndValue = DefaultScale;
        tween.Length = AnimationTime;
        tween.AutoRun = true;
        tween.TweenCompleted += OnContentShown;
    }


    private void PlayContentHideAnimation(bool remove = true)
    {
        if (!_currentContent)
            return;
        _active = false;
        dfTweenVector3 tween = _currentContent.GetComponent<dfTweenVector3>();
        if (tween)
        {
            tween.Stop();
            Destroy(tween);
        }
        tween = _currentContent.AddComponent<dfTweenVector3>();
        tween.Target = new dfComponentMemberInfo()
        {
            Component = _currentContent.transform,
            MemberName = "localScale"
        };
        tween.StartValue = DefaultScale;
        tween.EndValue = HiddenScale;
        tween.Length = AnimationTime;
        tween.AutoRun = true;
        if(remove)
            tween.TweenCompleted += OnRemoveContent;
        else
        {
            tween.TweenCompleted += OnHideContent;
        }
    }
    private void OnContentShown(dfTweenPlayableBase sender)
    {
        _active = true;
    }

    private void OnRemoveContent(dfTweenPlayableBase sender)
    {
        _content.Remove(_currentContent);
        Action callback;
        if (_immediateCallbacks.TryGetValue(_currentContent,out callback))
        {
            if (callback != null)
                callback();
            _immediateCallbacks.Remove(_currentContent);
        }
        Destroy(_currentContent);
        ShowContent(_content[0]);
    }

    private void OnHideContent(dfTweenPlayableBase sender)
    {
        ShowContent(_content[0]);
    }
}
