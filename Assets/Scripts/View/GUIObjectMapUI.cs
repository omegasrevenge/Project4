using System;
using UnityEngine;

public class GUIObjectMapUI : MonoBehaviour 
{
    private const string Prefab = "GUI/panel_mapui";
    private const string MenuPanelStr = "panel_menu";
    private const string MessagePanelStr = "panel_message";
    private const string RootPanelStr = "panel_root";
    private const string MenuButtonStr = "panel_menu_button";
    public const float MaxViewportScroll = 0.78f;
    public const float MenuSpeed = 1f;

    private dfControl _guiRoot;
    private dfControl _menuButton;
    private dfControl _menuPanel;
    private MovableGUIPanel _menuViewport;
    private MovableGUIPanel _guiViewport;
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

    public void Init(Map root)
    {
        _root3D = root;
        GameObject GUIRoot = GameObject.FindGameObjectWithTag(ViewController.UIRootTag);
        _menuViewport = GameObject.FindGameObjectWithTag(ViewController.MenuRootTag).transform.Find(RootPanelStr).GetComponent<MovableGUIPanel>();
        _guiViewport = GetComponent<MovableGUIPanel>();
        ViewportScrollState = 0f;
        _fightInvation = transform.Find(MessagePanelStr).GetComponent<GUIObjectMessage>();
        _menuPanel = transform.Find(MenuPanelStr).GetComponent<dfControl>();
        _menuPanel.BringToFront();
        _menuButton = _menuPanel.transform.Find(MenuButtonStr).GetComponent<dfControl>();
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
            _viewportScrollState = _menuViewport.Phase = _guiViewport.Phase = value;
            if (_root3D)
                _root3D.ViewportPhase = _viewportScrollState;
        }
    }

    public void OpenMenu()
    {
        TouchInput.OnClearAll();
        Debug.Log("sort back button to front");
        _menuButton.Click -= OnOpen;
        _menuPanel.Click += OnClose;
        _menuPanel.BringToFront();
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
        _menuPanel.Click -= OnClose;
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
        _menuPanel.Click -= OnClose;
        TouchInput.EnableBy(this);

        Debug.Log("ScrollState: "+ViewportScrollState);
    }

    public void AddMarker(TouchObject[] touches)
    {
        ObjectOnMap[] objects = Array.ConvertAll(touches, item => (ObjectOnMap)item);
        GUIObjectMarker.Create(_guiRoot,objects);
    }

    public void SetCreatureInfo(Creature creature)
    {
        if (!_creatureInfo)
            _creatureInfo = GUIObjectCreatureInfo.Create(_guiRoot, creature);
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
