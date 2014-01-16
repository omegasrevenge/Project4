using System;
using UnityEngine;


[Serializable]
public class Creature
{
    public string CreatureID;
    public string Name;
    public int XP;
	public int Level;
	public int HP;
	public int HPMax;
	public int Damage;
	public int Defense;
	public int Dexterity;
	public int Skillpoints;
	public Slot[] slots = new Slot[4];

	public struct Slot
	{
		public int fire;
		public int tech;
		public int nature;
		public int water;
		public int storm;
		public int driodenElement;
		public int driodenHealth;
		public int driodenLevel;
	}

    public void ReadJson(JSONObject json)
    {
		CreatureID = (string)json["CId"];
        Name = (string)json["Name"];
		XP = (int)json["XP"];
        Level = (int)json["Level"];
		HP = (int)json["HP"];
		HPMax = (int)json["HPMax"];
		Damage = (int)json["Damage"];
		Defense = (int)json["Defense"];
		Skillpoints = (int)json["Skillpoints"];
	   
		JSONObject jsonSlots = json["slots"];
	    
		for (int i = 0; i < jsonSlots.Count; i++)
	    {
		    slots[i] = new Slot()
		    {
			    fire = (int) jsonSlots[i]["Element0"],
			    tech = (int) jsonSlots[i]["Element1"],
			    nature = (int) jsonSlots[i]["Element2"],
			    water = (int) jsonSlots[i]["Element3"],
			    storm = (int) jsonSlots[i]["Element4"],
			    driodenElement = (int) jsonSlots[i]["EquipElement"],
			    driodenHealth = (int) jsonSlots[i]["EquipHealth"],
			    driodenLevel = (int) jsonSlots[i]["EquipLevel"]
		    };
	    }
    }
}
