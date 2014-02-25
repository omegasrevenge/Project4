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
        control.RelativePosition = position;
    }
}
