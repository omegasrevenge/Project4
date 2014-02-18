using System;
using UnityEngine;

public class GUIObjectMapUI : MonoBehaviour 
{
    private const string Prefab = "GUI/panel_mapui";
    private const string MenuPanelStr = "panel_menu";
    private const string MenuButtonStr = "panel_menu_button";
    public const float MaxViewportScroll = 0.78f;
    public const float MenuSpeed = 1f;

    private dfControl _guiRoot;
    private GameObject _menuRoot;
    private dfControl _menuButton;
    private dfControl _menuPanel;
    private MovableViewport _guiViewport;
    private GUIObjectCreatureInfo _creatureInfo;
    private Map _root3D;

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
         _menuRoot = GameObject.FindGameObjectWithTag(ViewController.MenuRootTag);
        _guiViewport = GUIRoot.transform.FindChild(ViewController.CameraStr).GetComponent<MovableViewport>();
        ViewportScrollState = 0f;

        _menuPanel = transform.FindChild(MenuPanelStr).GetComponent<dfControl>();
        _menuPanel.BringToFront();
        _menuButton = _menuPanel.transform.FindChild(MenuButtonStr).GetComponent<dfControl>();
        Debug.Log(_menuButton);
        _menuButton.Click += OnOpen;


    }

    public void OnOpen(dfControl control, dfMouseEventArgs args)
    {
        if (args.Used) return;
        args.Use();
        TouchInput.OnClearAll();
        ViewportScrollState = 0.001f;
        OpenMenu();
        _menuButton.Click -= OnOpen;
        _menuPanel.Click += OnClose;
        TouchInput.DisableBy(this);
    }

    public void OnClose(dfControl control, dfMouseEventArgs args)
    {
        if (args.Used) return;
        args.Use();
        CloseMenu();
        _menuButton.Click += OnOpen;
        _menuPanel.Click -= OnClose;
        TouchInput.EnableBy(this);
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
            _viewportScrollState = _guiViewport.phase = value;
            if (_root3D)
                _root3D.ViewportPhase = _viewportScrollState;
            if (_viewportScrollState == 0f)
                _menuRoot.SetActive(false);
            else
                _menuRoot.SetActive(true);
        }
    }

    public void OpenMenu()
    {
        dfTweenFloat tween = GetComponent<dfTweenFloat>();
        if (tween)
            Destroy(tween);
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
        dfTweenFloat tween = GetComponent<dfTweenFloat>();
        if (tween)
            Destroy(tween);
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
}
