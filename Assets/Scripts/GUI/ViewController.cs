using UnityEngine;

public class ViewController : MonoBehaviour
{
    public const string UIRootTag = "GUI";
    public const string MenuRootTag = "Menu";
    public const string MainCameraTag = "MainCamera";
    public const string RootPanelStr = "panel_root";
    public const string CameraStr = "camera";

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
        Init();
        if (_instance == null) 
            _instance = this;
        else if (_instance != this) 
            Destroy(gameObject);
	    Init();
	}

    private void Init()
    {
        _gameManager = GameManager.Singleton;
        GameObject GUIRoot = GameObject.FindGameObjectWithTag(UIRootTag);
        GameObject MenuRoot = GameObject.FindGameObjectWithTag(MenuRootTag);

        _gui = GUIRoot.transform.FindChild(RootPanelStr).GetComponent<dfControl>();
        _menu = MenuRoot.transform.FindChild(RootPanelStr).GetComponent<dfControl>();
        _camera3D = GameObject.FindGameObjectWithTag(MainCameraTag).GetComponent<Camera>();

        _guiViewport = GUIRoot.transform.FindChild(CameraStr).GetComponent<MovableViewport>();
        _menuViewport = _menu.GetComponent<MovableGUIViewport>();
        _3DViewport = _camera3D.GetComponent<MovableViewport>();

        ViewportScrollState = 0f;
    }

    void Update () 
    {
	
	}


}
