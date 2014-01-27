using UnityEngine;
using System.Collections;

public class SceneRoot3D : MonoBehaviour
{
    private const string CameraStr = "camera";

    private Camera _camera;
    private MovableViewport _viewport;
    private float _viewportScrollState;

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
}
