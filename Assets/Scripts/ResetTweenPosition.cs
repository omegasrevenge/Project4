using UnityEngine;
using System.Collections;

public class ResetTweenPosition : MonoBehaviour
{
    public void resetTween()
    {
        GetComponent<dfLabel>().Position = GetComponent<dfTweenVector3>().StartValue;
    }
}
