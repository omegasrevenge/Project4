using UnityEngine;
using System.Collections;

public class SceneRoot3D : MonoBehaviour
{
    protected const string CameraStr = "camera";

    protected Camera _camera;
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
