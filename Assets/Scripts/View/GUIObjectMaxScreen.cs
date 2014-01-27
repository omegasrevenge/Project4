using System;
using UnityEngine;
using System.Collections;

public class GUIObjectMaxScreen : MonoBehaviour
{
    private const string Prefab = "GUI/panel_maxscreen";

    private GameObject _content;
    private dfControl _control;
    private dfControl _root;

    public static GUIObjectMaxScreen Create(dfControl root, GameObject content = null)
    {
        dfControl cntrl = root.AddPrefab(Resources.Load<GameObject>(Prefab));
        cntrl.Size = cntrl.Parent.Size;
        cntrl.RelativePosition = Vector2.zero;

        GUIObjectMaxScreen obj = cntrl.GetComponent<GUIObjectMaxScreen>();
        obj._root = root;
        obj._control = cntrl;
        obj.AddMaxScreen(content);
        return obj;
    }

    public GUIObjectMaxScreen AddMaxScreen(GameObject content)
    {
        if(_content != null)
            Destroy(_content);
        if (content == null)
            return this;

        if (content.GetComponent<dfControl>() == null)
        {
            throw new InvalidCastException();
        }
        content.transform.parent = transform;
        content.layer = gameObject.layer;

        var child = content.GetComponent<dfControl>();
        _control.AddControl(child);
        child.Size = _control.Size;
        child.RelativePosition = Vector2.zero;

        child.BringToFront();

        _content = content;

        return this;
    }

    public void Remove()
    {
        Destroy(gameObject);
    }
}
