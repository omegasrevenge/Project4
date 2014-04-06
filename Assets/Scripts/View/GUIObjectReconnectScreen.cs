using System;
using UnityEngine;
using System.Collections;

public class GUIObjectReconnectScreen : MonoBehaviour
{
    private const string Prefab = "GUI/panel_connectionLost";

    private Delegate _request;
    private object[] _params;

    public dfLabel ConnectionProblems;
    public dfLabel Cancel;
    public dfLabel Retry;

    public static GUIObjectReconnectScreen Create(dfControl root, Delegate serverRequest, params object[] @params)
    {
        dfControl cntrl = root.AddPrefab(Resources.Load<GameObject>(Prefab));
        cntrl.Size = cntrl.Parent.Size;
        cntrl.RelativePosition = Vector2.zero;
        cntrl.BringToFront();
        GUIObjectReconnectScreen obj = cntrl.GetComponent<GUIObjectReconnectScreen>();
        obj.Init(serverRequest, @params);
        return obj;
    }

    private void Init(Delegate serverRequest, params object[] @params)
    {
        _request = serverRequest;
        _params = @params;
        ConnectionProblems.Text = Localization.GetText("lost_connection_title");
        Cancel.Text = Localization.GetText("cancel");
        Cancel.Parent.Click += OnCancel;
        Retry.Text = Localization.GetText("retry");
        Retry.Parent.Click += OnRetry;
    }

    public void OnCancel(dfControl control, dfMouseEventArgs mouseEvent)
    {
        Destroy(gameObject);
    }

    public void OnRetry(dfControl control, dfMouseEventArgs mouseEvent)
    {
        Destroy(gameObject);
        _request.DynamicInvoke(_params);
    }
}
