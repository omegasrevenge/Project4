using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class GUIObjectSlotState : MonoBehaviour
{
    private const string ElementSprite = "combat_driod";
    private const string SlotSprite = "combat_slot_imprint_";
    
    [SerializeField]
    public List<dfSprite> imprints;
    [SerializeField]
    public dfSprite element;

    public string Element 
    {
        get { return element.SpriteName.Replace(ElementSprite, ""); }
        set { element.SpriteName = ElementSprite + value; }
    }

    public void InitUnlock(int cid, GUIObjectLevelUp objectLevelUp )
    {
        GetComponent<dfButton>().Click += (control, @event) =>
        {
            GameManager.Singleton.AddCreatureEQSlot(cid);
            objectLevelUp.Init();
        };
    }

    public void InitUpgrade(int cid, int sid, int element, GUIObjectLevelUp objectLevelUp)
    {
        GetComponent<dfButton>().Click += (control, @event) =>
            {
                GameManager.Singleton.UpgradeCreatureSlot(cid, sid, element);
                objectLevelUp.Init();
            };
    }

    public void SetImprint(string element, int index)
    {
        if (index > 8)
            imprints[index].SpriteName = SlotSprite + element + (index + 1).ToString();
        imprints[index].SpriteName = SlotSprite + element + "0" + (index+1).ToString();
    }

    public void DeactivateImprint(int index)
    {
        imprints[index].Hide();
    }
}
