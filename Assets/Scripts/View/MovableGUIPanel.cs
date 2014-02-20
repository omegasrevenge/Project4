using UnityEngine;
using System.Collections;

public class MovableGUIPanel : MonoBehaviour 
{

    public float Phase = 0f;

    private dfControl _control;

    private void Awake()
    {
        _control = GetComponent<dfControl>();
    }

    private void Update()
    {
            Vector2 offset = new Vector2(Phase*_control.Size.x,0);
            _control.RelativePosition = offset;

    }
}
