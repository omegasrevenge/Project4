using System;
using UnityEngine;

public class GUIObjectMapUI : MonoBehaviour 
{
    private const string Prefab = "GUI/panel_mapui";

    private dfControl _root;
    private GUIObjectCreatureInfo _creatureInfo;
    public static GUIObjectMapUI Create(dfControl root)
    {
        dfControl cntrl = root.AddPrefab(Resources.Load<GameObject>(Prefab));
        cntrl.Size = cntrl.Parent.Size;
        cntrl.RelativePosition = Vector2.zero;
        GUIObjectMapUI obj = cntrl.GetComponent<GUIObjectMapUI>();
        obj._root = cntrl;
        return obj;
    }

    public void AddMarker(TouchObject[] touches)
    {
        ObjectOnMap[] objects = Array.ConvertAll(touches, item => (ObjectOnMap)item);
        GUIObjectMarker.Create(_root,objects);
    }

    public void SetCreatureInfo(Creature creature)
    {
        if (!_creatureInfo)
            _creatureInfo = GUIObjectCreatureInfo.Create(_root, creature);

    }
}
