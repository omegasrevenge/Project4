using UnityEngine;
using System.Collections;

public class GUIObjectCreatureInfo : MonoBehaviour 
{
    private const string Prefab = "GUI/panel_creatureinfo";

    public static GUIObjectCreatureInfo Create(dfControl root, Creature creature)
    {
        dfControl cntrl = root.AddPrefab(Resources.Load<GameObject>(Prefab));
        cntrl.Size = cntrl.Parent.Size;
        cntrl.RelativePosition = Vector2.zero;
        GUIObjectCreatureInfo obj = cntrl.GetComponent<GUIObjectCreatureInfo>();
        return obj;
    }

    public void SetCreature(Creature creature)
    {
        
    }
}
