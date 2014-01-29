using System;
using UnityEngine;


[Serializable]
public class Creature
{
	public int ModelID;
    public int CreatureID;
    public string Name;
    public int XP;
	public int Level;
	public int HP;
	public int HPMax;
	public int Damage;
	public int Defense;
	public int Dexterity;
	public int Skillpoints;
    public BattleEngine.ResourceElement BaseElement;
	public Slot[] slots;

	public struct Slot
	{
		public int fire;
        public int energy;
		public int nature;
		public int water;
		public int storm;
        public BattleEngine.ResourceElement driodenElement;
		public int driodenHealth;
		public int driodenLevel;
        public int driodenStrength;
		public int slotId;
	}

    public void ReadJson(JSONObject json)
    {
        if (json == null)
        {
            Debug.LogError("No JSON for Creature.cs! Please check, what's wrong. Cheers, Anton.");
            return;
        }
		//Debug.Log(json);
		ModelID = (int)json["ModelId"];
        BaseElement = (BattleEngine.ResourceElement)(int)json["Element"];
		CreatureID = (int)json["CId"];
        Name = (string)json["Name"];
		XP = (int)json["XP"];
        Level = (int)json["Level"];
		HP = (int)json["HP"];
		HPMax = (int)json["HPMax"];
		Damage = (int)json["Damage"];
		Defense = (int)json["Defense"];
		Dexterity = (int)json["Dexterity"];
		Skillpoints = (int)json["Skillpoints"];
	   
		JSONObject jsonSlots = json["Slots"];

	    slots = new Slot[jsonSlots.Count];

		for (int i = 0; i < jsonSlots.Count; i++)
	    {
		    slots[i] = new Slot()
		    {
			    fire = (int) jsonSlots[i]["Element0"],
			    energy = (int) jsonSlots[i]["Element1"],
			    nature = (int) jsonSlots[i]["Element2"],
			    water = (int) jsonSlots[i]["Element3"],
			    storm = (int) jsonSlots[i]["Element4"],
                driodenElement = (BattleEngine.ResourceElement)(int)jsonSlots[i]["EquipElement"],
			    driodenHealth = (int)((float)jsonSlots[i]["EquipHealth"]*100),
			    driodenLevel = (int) jsonSlots[i]["EquipLevel"],
				slotId = (int)jsonSlots[i]["SlotId"]
		    };
            switch(slots[i].driodenElement)
            {
                case BattleEngine.ResourceElement.Fire:
                    slots[i].driodenStrength = slots[i].fire;
                    break;

                case BattleEngine.ResourceElement.Nature:
                    slots[i].driodenStrength = slots[i].nature;
                    break;

                case BattleEngine.ResourceElement.Storm:
                    slots[i].driodenStrength = slots[i].storm;
                    break;

                case BattleEngine.ResourceElement.Energy:
                    slots[i].driodenStrength = slots[i].energy;
                    break;

                case BattleEngine.ResourceElement.Water:
                    slots[i].driodenStrength = slots[i].water;
                    break;
            }
	    }
    }
}
