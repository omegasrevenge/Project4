using System;
using UnityEngine;

public class ViewController : MonoBehaviour
{
    public const string UIRootTag = "GUI";
    public const string MenuRootTag = "Menu";
    public const string MainCameraTag = "MainCamera";
    public const string RootPanelStr = "panel_root";
    public const string CameraStr = "camera";

    public const string Blindtext = "blindtext";

    public const string PanelMaxScreenStr = "GUI/panel_maxscreen";

    private static ViewController _instance;

    private GameManager _gameManager;

    private dfControl _gui;
    private dfControl _menu;
    private Camera _camera3D;

    private MovableViewport     _guiViewport;
    private MovableGUIViewport  _menuViewport;
    private MovableViewport     _3DViewport;

    [SerializeField]
    private float _viewportScrollState = 0f;
    public const float MaxViewportScroll = 0.7f;

    public static ViewController Singleton
    {
        get
        {
            if (_instance != null)
                return _instance;
            return null;
        }
    }

    public Camera Camera3D
    {
        get { return _camera3D; }
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
            _viewportScrollState = _menuViewport.phase = _guiViewport.phase = _3DViewport.phase = value;
        }
    }

    public static ViewController Create()
    {
        if (_instance != null)
            return _instance;
        GameObject obj = new GameObject("controller_viewcontroller")
        {
            hideFlags = HideFlags.DontSave,
            tag = GameManager.DontSaveTag
        };
        _instance = obj.AddComponent<ViewController>();
        return _instance;
    }

	void Awake ()
	{
        if (_instance == null) 
            _instance = this;
        else if (_instance != this) 
            Destroy(gameObject);
	    Init();
	}

    private void Init()
    {
        _gameManager = GameManager.Singleton;
        _camera3D = GameObject.FindGameObjectWithTag(MainCameraTag).GetComponent<Camera>();
        _3DViewport = _camera3D.GetComponent<MovableViewport>();

        if (_gameManager.DummyUI)
        {
            GameObject.FindGameObjectWithTag(UIRootTag).SetActive(false);
            GameObject.FindGameObjectWithTag(MenuRootTag).SetActive(false);
            return;
        }
        
        GameObject GUIRoot = GameObject.FindGameObjectWithTag(UIRootTag);
        GameObject MenuRoot = GameObject.FindGameObjectWithTag(MenuRootTag);

        _gui = GUIRoot.transform.FindChild(RootPanelStr).GetComponent<dfControl>();
        _menu = MenuRoot.transform.FindChild(RootPanelStr).GetComponent<dfControl>(); 

        _guiViewport = GUIRoot.transform.FindChild(CameraStr).GetComponent<MovableViewport>();
        _menuViewport = _menu.GetComponent<MovableGUIViewport>();
        

        ViewportScrollState = 0f;

        //AddMaxScreen("iris_01_a_title", "iris_01_a_text");
        //AddIrisPopup("iris_01_text", "Bodo_Wartke_Ja_Schatz_Ich_schneide_Dir_ein_Ohr_ab-de");
    }

    void Update () 
    {
	
	}


    public void AddMaxScreen(string textKeyTitle = Blindtext, string textKeyText = Blindtext, dfButton button = null, Action callback = null)
    {
        dfControl cntrl = _gui.AddPrefab(Resources.Load<GameObject>(PanelMaxScreenStr));
        cntrl.Size = cntrl.Parent.Size;
        cntrl.RelativePosition = Vector2.zero;
        GUIObjectMaxScreen obj = cntrl.GetComponent<GUIObjectMaxScreen>();
        obj.Text = textKeyText;
        obj.Title = textKeyTitle;
    }

    public GUIObjectIrisPopup AddIrisPopup(string textKeyText = Blindtext, string audio = "")
    {
        return GUIObjectIrisPopup.Create(_gui,textKeyText,audio).Show();
    }

}
