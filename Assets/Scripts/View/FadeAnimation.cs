using System;
using UnityEngine;
using System.Collections;

public class FadeAnimation : MonoBehaviour
{
    public new event Action DestroyObject;

    protected virtual void OnDestroyObject()
    {
        Action handler = DestroyObject;
        if (handler != null) handler();
    }
}
