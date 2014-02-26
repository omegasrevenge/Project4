using UnityEngine;

public class GUIObjectBaseUI : MonoBehaviour 
{
    private const string Prefab = "GUI/panel_baseui";

	private dfControl _root;

    public static GUIObjectBaseUI Create(dfControl root)
    {
        dfControl cntrl = root.AddPrefab(Resources.Load<GameObject>(Prefab));
        cntrl.Size = cntrl.Parent.Size;
        cntrl.RelativePosition = Vector2.zero;
        GUIObjectBaseUI obj = cntrl.GetComponent<GUIObjectBaseUI>();
	    obj._root = cntrl;
		obj.AddMenue();
        return obj;
    }

	public void AddMenue()
	{
		GUIObjectBaseMenue.Create(_root);
	}

	public void AddCrafting()
	{
		GUIObjectCrafting.Create(_root);
	}

	public void AddEquip(Creature curCreature, Creature.Slot slot)
	{
		GUIObjectEquip.Create(_root, curCreature, slot);
	}
}
