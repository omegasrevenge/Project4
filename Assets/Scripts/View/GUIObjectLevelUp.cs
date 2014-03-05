using System;
using System.Collections.Generic;
using UnityEngine;

public class GUIObjectLevelUp : MonoBehaviour, IPopupContent
{
    private const string Prefab = "GUI/panel_levelslot";
    private const string background = "combat_panel_";

    public event Action ClosePopup;
    //public GameObject ResultTemplate;


    [SerializeField] public dfSprite Background;
    [SerializeField] public List<dfButton> slots; 


    public static GameObject Create()
    {
        GameObject go = Instantiate(Resources.Load<GameObject>(Prefab)) as GameObject;
        GUIObjectLevelUp levelUpContent = go.GetComponent<GUIObjectLevelUp>();

        levelUpContent.Init();
        return go;
    }

    public void Init()
    {
        GetComponent<GUIObjectTextPanel>().Title = "level_up_title";
        GetComponent<GUIObjectTextPanel>().Text = "level_up_text"+"#"+GameManager.Singleton.Player.CurCreature.Name;


        Background.SpriteName = background + GameManager.Singleton.Player.CurrentFaction.ToString().ToLower();
        Creature creature = GameManager.Singleton.Player.CurCreature;
        GUIObjectSlotState currentSlot;

        for (int i = 0; i < slots.Count; i++)
        {
            currentSlot = slots[i].GetComponent<GUIObjectSlotState>();        
            if (i >= creature.slots.Length)
            {
                currentSlot.InitUnlock(creature.CreatureID, this);
                currentSlot.gameObject.GetComponent<dfButton>().BackgroundSprite = "";
                currentSlot.Element = "";
                for (int j = 0; j < currentSlot.imprints.Count; j++)
                    currentSlot.DeactivateImprint(j);
                continue;
            }
            if (creature.slots[i].driodenElement == GameManager.ResourceElement.None)
                currentSlot.Element = "";
            else
                currentSlot.Element = "_" + creature.slots[i].driodenElement.ToString();
            int element = (int)creature.slots[i].driodenElement;
            if (element < 0) element = (int)creature.BaseElement;

            currentSlot.InitUpgrade(creature.CreatureID, creature.slots[i].slotId, element, this);
            int e = 0; int f = 0; int s = 0; int l = 0; int w = 0;

            for (int j = 0; j < currentSlot.imprints.Count; j++)
            {
                if (e < creature.slots[i].energy)
                {
                    currentSlot.SetImprint("energy", j);
                    e++;
                }
                else if (f < creature.slots[i].fire)
                {
                    currentSlot.SetImprint("fire", j);
                    f++;
                }
                else if (s < creature.slots[i].storm)
                {
                    currentSlot.SetImprint("storm", j);
                    s++;
                }
                else if (l < creature.slots[i].nature)
                {
                    currentSlot.SetImprint("life", j);
                    l++;
                }
                else if (w < creature.slots[i].water)
                {
                    currentSlot.SetImprint("water", j);
                    w++;
                }
                else
                    currentSlot.DeactivateImprint(j);
            }
        }
    }
}