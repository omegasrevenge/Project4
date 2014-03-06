using UnityEngine;
using System.Collections;

public class dfClickEventDebugger : MonoBehaviour 
{
    void Awake()
    {
        Debug.LogWarning("Please remove this DebugScript from "+gameObject.name);
    }
    void OnMouseDown(dfControl control, dfMouseEventArgs args)
    {
        Debug.Log("MouseDown(sender: "+control+", reciever: "+gameObject.name+", used: "+args.Used+")");
    }

    void OnMouseUp(dfControl control, dfMouseEventArgs args)
    {
        Debug.Log("MouseUp(sender: " + control + ", reciever: " + gameObject.name + ", used: " + args.Used + ")");
    }

    void OnClick(dfControl control, dfMouseEventArgs args)
    {
        Debug.Log("Click(sender: " + control + ", reciever: " + gameObject.name + ", used: " + args.Used + ")");
    }

}
