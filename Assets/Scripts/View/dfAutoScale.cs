using System;
using UnityEngine;

public class dfAutoScale : MonoBehaviour
{
    private static readonly float ReferenceHeight = 1280f;

    void Start()
    {

        dfControl control = GetComponent<dfControl>();
        Vector2 size = control.Size;
        Vector3 position = control.RelativePosition;
        float scaleFactor = Screen.height/ReferenceHeight;
        control.Size = size*scaleFactor;
 
        if (control.Anchor.IsFlagSet(dfAnchorStyle.Bottom))
        {
            position.y += (size - control.Size).y;
        }
        if (control.Anchor.IsFlagSet(dfAnchorStyle.Right))
        {
            position.x += (size - control.Size).x;
        }

        control.RelativePosition = position;
    }
}
