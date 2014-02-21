using UnityEngine;
using System.Collections;

public class MovableGUIPanel : MonoBehaviour 
{

    public float Phase = 0f;
    public bool Pinned = true;

    private dfControl _control;

    private void Awake()
    {
        _control = GetComponent<dfControl>();
    }

    private void Update()
    {
        if (Pinned)
        {
            Vector2 offset = new Vector2(Phase*_control.Size.x, 0);
            _control.RelativePosition = offset;
        }
        else
        {
            if(Phase == 0f)
                _control.Hide();
            else
            {
                _control.Show();
                _control.Width = Phase*Screen.width;
            }
            
        }
    }
}
