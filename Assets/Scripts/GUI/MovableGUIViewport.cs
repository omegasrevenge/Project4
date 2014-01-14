using UnityEngine;
using System.Collections;

public class MovableGUIViewport : MonoBehaviour 
{

    public float phase = 1f;
    public bool pinned;

    private dfControl _control;

    private void Awake()
    {
        _control = GetComponent<dfControl>();
    }

    private void Update()
    {
        if (phase <= 0 &&!pinned)
        {
            _control.IsVisible = false;
            return;
        }
        _control.IsVisible = true;

        if (pinned)
        {
            Vector2 offset = new Vector2(phase*_control.GUIManager.GetScreenSize().x,0);
            _control.GUIManager.UIOffset = offset;
            return;
        }
        Vector2 size = _control.GUIManager.GetScreenSize();
        size.x *= phase;
        _control.Size = size;
    }
}
