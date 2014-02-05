using UnityEngine;

public class GUIObjectMapUI : MonoBehaviour 
{
    private const string Prefab = "GUI/panel_mapui";

    private dfControl _root;

    public static GUIObjectMapUI Create(dfControl root)
    {
        dfControl cntrl = root.AddPrefab(Resources.Load<GameObject>(Prefab));
        cntrl.Size = cntrl.Parent.Size;
        cntrl.RelativePosition = Vector2.zero;
        GUIObjectMapUI obj = cntrl.GetComponent<GUIObjectMapUI>();
        obj._root = cntrl;
        return obj;
    }

    public void AddMarker(ObjectOnMap[] objects)
    {
        Debug.Log("AddMarker");
        foreach (ObjectOnMap objectOnMap in objects)
        {
            Debug.Log(objectOnMap.gameObject.name);
        }
        GUIObjectMarker.Create(_root,objects);
    }
}
