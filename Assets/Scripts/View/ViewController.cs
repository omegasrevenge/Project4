﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ViewController : MonoBehaviour
{
    public const string UIRootTag = "GUI";
    public const string MenuRootTag = "Menu";
    public const string RootPanelStr = "panel_root";
    public const string CameraStr = "camera";

    public const string Blindtext = "blindtext";

    private static ViewController _instance;

    private GameManager _gameManager;

    private dfControl _gui;
    private dfControl _menu;

    private MovableViewport     _guiViewport;
    private MovableGUIViewport  _menuViewport;

    private List<SceneRoot3D> _3DRoots = new List<SceneRoot3D>();
    private SceneRoot3D _current3DRoot;

    [SerializeField]
    private float _viewportScrollState = 0f;
    public const float MaxViewportScroll = 0.7f;

    private GUIObjectMaxScreen _maxScreen;
    private GUIObjectLoadingScreen _loadingScreen;

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
        get
        {
            if (_current3DRoot)
                return _current3DRoot.Camera;
            return null;
        }
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
            _viewportScrollState = _menuViewport.phase = _guiViewport.phase = value;
            if (_current3DRoot)
                _current3DRoot.ViewportPhase = _viewportScrollState;
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
        
        GameObject GUIRoot = GameObject.FindGameObjectWithTag(UIRootTag);
        GameObject MenuRoot = GameObject.FindGameObjectWithTag(MenuRootTag);

        _gui = GUIRoot.transform.FindChild(RootPanelStr).GetComponent<dfControl>();
        _menu = MenuRoot.transform.FindChild(RootPanelStr).GetComponent<dfControl>(); 

        _guiViewport = GUIRoot.transform.FindChild(CameraStr).GetComponent<MovableViewport>();
        _menuViewport = _menu.GetComponent<MovableGUIViewport>();
        

        ViewportScrollState = 0f;

        //AddIrisPopup("iris_01_text", "Bodo_Wartke_Ja_Schatz_Ich_schneide_Dir_ein_Ohr_ab-de");
        //AddMaxScreen(GUIObjectNameInput.Create("screen_entername_title", "screen_entername_text", "continue", "default_name", null));
        //AddMaxScreen(null);
        ShowLoadingScreen(Localization.GetText("loadingscreen_login"));
    }

    void Update () 
    {
	
	}

    public SceneRoot3D Switch3DSceneRoot(SceneRoot3D newRoot, bool destroyOld = false)
    {
        if (_current3DRoot == newRoot)
            return _current3DRoot;

        //Deactivate old one
        if (_current3DRoot && destroyOld)
        {
            _3DRoots.Remove(_current3DRoot);
            Destroy(_current3DRoot.gameObject);
        }
        else if(_current3DRoot)
            _current3DRoot.gameObject.SetActive(false);
        
        //Activate new one
        if (newRoot && !_3DRoots.Contains(newRoot))
            _3DRoots.Add(newRoot);
        else if(newRoot)
            newRoot.gameObject.SetActive(true);

        _current3DRoot = newRoot;
        return _current3DRoot;
    }

    public SceneRoot3D GetSceneRoot3D<TRoot>() where TRoot : SceneRoot3D
    {
        return _3DRoots.FirstOrDefault(r => r is TRoot);
    }


    public GUIObjectMaxScreen AddMaxScreen(GameObject content)
    {
        if (_maxScreen == null)
            _maxScreen = GUIObjectMaxScreen.Create(_gui, content);
        else
            _maxScreen.AddMaxScreen(content);
        return _maxScreen;
    }

    public void RemoveMaxScreen()
    {
        if(_maxScreen != null)
            _maxScreen.Remove();
    }

    public GUIObjectIrisPopup AddIrisPopup(string textKeyText = Blindtext, string audio = "")
    {
        return GUIObjectIrisPopup.Create(_gui,textKeyText,audio).Show();
    }

    public GUIObjectSpectresIntro AddSpectresIntro(string textKeyText)
    {
        return GUIObjectSpectresIntro.Create(_gui, textKeyText);
    }

    public void ShowLoadingScreen(string text)
    {
        if (_loadingScreen == null)
            _loadingScreen = GUIObjectLoadingScreen.Create(text);
        else
            _loadingScreen.Show(text);
    }

    public void HideLoadingScreen()
    {
        if(_loadingScreen)
            _loadingScreen.Hide();
    }

}