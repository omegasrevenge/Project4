using UnityEngine;
using System.Collections;

public class SceneRoot3D : MonoBehaviour
{
    protected const string CameraStr = "camera";

    protected Camera _camera;
    protected MovableViewport _viewport;
    protected float _viewportScrollState;
    protected Component _gui;

    public Camera Camera
    {
        get
        {
            if (_camera == null)
            {
                _camera = transform.Find(CameraStr).GetComponent<Camera>();
            }
            return _camera;
        }
    }

    public float ViewportPhase
    {
        get
        {
            return _viewportScrollState;
        }

        set
        {
            if(_viewport == null && Camera)
                _viewport = Camera.GetComponent<MovableViewport>();
            if (_viewport == null) return;
            value = Mathf.Clamp(value, 0f, ViewController.MaxViewportScroll);
            _viewportScrollState = _viewport.phase = value;
        }
    }

    public virtual void AttachGUI(Component gui)
    {
        if(gui)
            _gui = gui;
    }

    void OnDisable()
    {
        if(_gui)
            _gui.GetComponent<dfControl>().Hide();
    }

    void OnEnable()
    {
        if (_gui)
            _gui.GetComponent<dfControl>().Show();
    }

    void OnDestroy()
    {
        if(_gui)
            Destroy(_gui.gameObject);
    }
}
