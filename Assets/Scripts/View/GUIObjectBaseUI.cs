using UnityEngine;

public class GUIObjectBaseUI : MonoBehaviour 
{
    private const string Prefab = "GUI/panel_baseui";

    public static GUIObjectBaseUI Create(dfControl root)
    {
        dfControl cntrl = root.AddPrefab(Resources.Load<GameObject>(Prefab));
        cntrl.Size = cntrl.Parent.Size;
        cntrl.RelativePosition = Vector2.zero;
        GUIObjectBaseUI obj = cntrl.GetComponent<GUIObjectBaseUI>();
        return obj;
    }
    
}
