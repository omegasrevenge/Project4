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
	public int Version;
	public int Defense;
	public int Dexterity;
	public int Skillpoints;
	public GameManager.ResourceElement BaseElement;
	public Slot[] slots;

	public struct Slot
	{
		public int fire;
        public int energy;
		public int nature;
		public int water;
		public int storm;
		public GameManager.ResourceElement driodenElement;
		public float driodenHealth;
		public int driodenLevel;
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
		BaseElement = (GameManager.ResourceElement)(int)json["Element"];
		CreatureID = (int)json["CId"];
        Name = (string)json["Name"];
		XP = (int)json["XP"];
        Level = (int)json["Level"];
		HP = (int)json["HP"];
		Version = (int)json["Version"];
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
                energy = (int)jsonSlots[i]["Element0"],
                fire = (int)jsonSlots[i]["Element1"],
                storm = (int)jsonSlots[i]["Element2"],
			    nature = (int) jsonSlots[i]["Element3"],
			    water = (int) jsonSlots[i]["Element4"],
				driodenElement = (GameManager.ResourceElement)(int)jsonSlots[i]["EquipElement"],
			    driodenHealth = (float)jsonSlots[i]["EquipHealth"],
			    driodenLevel = (int) jsonSlots[i]["EquipLevel"],
				slotId = (int)jsonSlots[i]["SlotId"]
		    };
	    }
    }
}
