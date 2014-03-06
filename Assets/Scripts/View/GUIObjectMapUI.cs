using System;
using UnityEngine;

public class GUIObjectMapUI : MonoBehaviour 
{
    private const string Prefab = "GUI/panel_mapui";
    private const string ContentPanelStr    = "panel_content";
    private const string MenuPanelStr       = "panel_menu";
    private const string MessagePanelStr    = "panel_message";
    private const string MenuButtonStr      = "panel_menu_button";
    public const float MaxViewportScroll = 0.78f;
    public const float MenuSpeed = 1f;

    private dfControl _guiRoot;
    //_guiRoot contains _contentRoot and _menuRoot
    private dfControl       _contentRoot;
    private MovableGUIPanel _contentViewport;
    private dfControl       _menuRoot;
    private MovableGUIPanel _menuViewport;

    //_contentRoot contains _menuButtonPanel and _menuButtonPanel contains _menuButton
    private dfControl _menuButtonPanel;
    private dfControl _menuButton;
    //_contentRoot also contains _creatureInfo and _fightInvation
    private GUIObjectCreatureInfo _creatureInfo;
    private GUIObjectMessage _fightInvation;

    private Map _root3D;

    [SerializeField]
    private float _viewportScrollState;


    public static GUIObjectMapUI Create(dfControl root)
    {
        dfControl cntrl = root.AddPrefab(Resources.Load<GameObject>(Prefab));
        cntrl.Size = cntrl.Parent.Size;
        cntrl.RelativePosition = Vector2.zero;
        GUIObjectMapUI obj = cntrl.GetComponent<GUIObjectMapUI>();
        obj._guiRoot = cntrl;
        return obj;
    }

    public bool MenuControlsEnabled 
    {
        get
        {
            if (!_menuRoot) return false;
            return _menuRoot.IsEnabled;
        }
        set
        {
            if (_menuRoot)
            {
                Debug.Log("Menu-Panel: " + value);
                _menuRoot.IsEnabled = value;
            }
        }
    }

    public void Init(Map root)
    {
        _root3D = root;

        _contentRoot    = transform.Find(ContentPanelStr).GetComponent<dfControl>();
        _menuRoot       = transform.Find(MenuPanelStr).GetComponent<dfControl>();

        _contentViewport    = _contentRoot.GetComponent<MovableGUIPanel>();
        _menuViewport       = _menuRoot.GetComponent<MovableGUIPanel>();
        ViewportScrollState = 0f;

        _fightInvation = _contentRoot.transform.Find(MessagePanelStr).GetComponent<GUIObjectMessage>();
        _menuButtonPanel = _contentRoot.transform.Find(MenuButtonStr).GetComponent<dfControl>();
        _menuButtonPanel.BringToFront();

        _menuButton = _menuButtonPanel.transform.Find(MenuButtonStr).GetComponent<dfControl>();
        _menuButton.Click += OnOpen;
    }

    private void OnOpen(dfControl control, dfMouseEventArgs args)
    {
        if (args.Used) return;
        args.Use();
        OpenMenu();
    }

    private void OnClose(dfControl control, dfMouseEventArgs args)
    {
        if (args.Used) return;
        args.Use();
        CloseMenu();
    }

    public float ViewportScrollState
    {
        get
        {
            return _viewportScrollState;
        }

        set
        {
            value = Mathf.Clamp(value, 0f, MaxViewportScroll);
            _viewportScrollState = _menuViewport.Phase = _contentViewport.Phase = value;
            if (_root3D)
                _root3D.ViewportPhase = _viewportScrollState;
        }
    }

    public void OpenMenu()
    {   
        TouchInput.OnClearAll();
        _menuButton.Click -= OnOpen;
        _menuButtonPanel.Click += OnClose;
        _menuButtonPanel.BringToFront();
        TouchInput.DisableBy(this);

        dfTweenFloat tween = GetComponent<dfTweenFloat>();
        if (tween)
        {
            tween.Stop();
            Destroy(tween);
        }
        tween = gameObject.AddComponent<dfTweenFloat>();
        tween.Target = new dfComponentMemberInfo()
        {
            Component = this,
            MemberName = "ViewportScrollState"
        };
        tween.StartValue = ViewportScrollState;
        tween.EndValue = 1f;
        tween.Function = dfEasingType.ExpoEaseOut;
        tween.Length = (1f - ViewportScrollState) * MenuSpeed;
        tween.AutoRun = true;
    }

    public void CloseMenu()
    {
        _menuButton.Click += OnOpen;
        _menuButtonPanel.Click -= OnClose;
        TouchInput.EnableBy(this);

        dfTweenFloat tween = GetComponent<dfTweenFloat>();
        if (tween)
        {
            tween.Stop();
            Destroy(tween);
        }
        tween = gameObject.AddComponent<dfTweenFloat>();
        tween.Target = new dfComponentMemberInfo()
        {
            Component = this,
            MemberName = "ViewportScrollState"
        };
        tween.StartValue = ViewportScrollState;
        tween.EndValue = 0f;
        tween.Function = dfEasingType.BackEaseOut;
        tween.Length = ViewportScrollState * MenuSpeed;
        tween.AutoRun = true;
    }

    public void CloseMenuImmediate()
    {
        dfTweenFloat tween = GetComponent<dfTweenFloat>();
        if (tween)
        {
            tween.Stop();
            Destroy(tween);
        }
        ViewportScrollState = 0;

        _menuButton.Click += OnOpen;
        _menuButtonPanel.Click -= OnClose;
        TouchInput.EnableBy(this);

        Debug.Log("ScrollState: "+ViewportScrollState);
    }

    public void AddMarker(TouchObject[] touches)
    {
        ObjectOnMap[] objects = Array.ConvertAll(touches, item => (ObjectOnMap)item);
        GUIObjectMarker.Create(_contentRoot,objects);
    }

    public void SetCreatureInfo(Creature creature)
    {
        if (!_creatureInfo)
            _creatureInfo = GUIObjectCreatureInfo.Create(_contentRoot, creature);
        else
            _creatureInfo.SetCreature(creature);

    }

    public void ShowFightInvation()
    {
        if (_fightInvation)
            _fightInvation.Show();
    }

    public void HideFightInvation()
    {
        if (_fightInvation)
            _fightInvation.Hide();
    }
}
