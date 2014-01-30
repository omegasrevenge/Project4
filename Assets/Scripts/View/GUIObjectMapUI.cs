using UnityEngine;

public class GUIObjectMapUI : MonoBehaviour 
{
    private const string Prefab = "GUI/panel_mapui";

    public static GUIObjectMapUI Create(dfControl root)
    {
        dfControl cntrl = root.AddPrefab(Resources.Load<GameObject>(Prefab));
        cntrl.Size = cntrl.Parent.Size;
        cntrl.RelativePosition = Vector2.zero;
        GUIObjectMapUI obj = cntrl.GetComponent<GUIObjectMapUI>();
        return obj;
    }
    
}
