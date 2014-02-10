using System;
using UnityEngine;


[Serializable]
public class Fight
{
	public int SkillID;
	public int Round;
	public string LastResult;
	public Creature EnemyCreature;
	public bool Turn;
    public bool Finished;

    public JSONObject info;

    public void ReadJson(JSONObject json)
    {
        info = json;
        LastResult = (string)json["LastResult"];
        Round = (int)json["Round"];
        EnemyCreature = new Creature();
        EnemyCreature.ReadJson(json["EnemyCreature"]);
        Turn = (bool)json["Turn"];
        Finished = (bool)json["Finished"];
    }

    public bool FighterAConfused
    {
        get
        {
            if (info == null) return false;
            return (int)info["FighterA"]["Con"][1] > 0;
        }
    }

    public bool FighterBConfused
    {
        get
        {
            if (info == null) return false;
            return (int)info["FighterB"]["Con"][1] > 0;
        }
    }

    public bool FighterABuffed
    {
        get
        {
            if (info == null) return false;
            return (int)info["FighterA"]["Def"][1] > 0;
        }
    }

    public bool FighterBBuffed
    {
        get
        {
            if (info == null) return false;
            return (int)info["FighterB"]["Def"][1] > 0;
        }
    }

    public bool FighterABurning
    {
        get
        {
            if (info == null) return false;
            return (int)info["FighterA"]["Dot"][1] > 0;
        }
    }

    public bool FighterBBurning
    {
        get
        {
            if (info == null) return false;
            return (int)info["FighterB"]["Dot"][1] > 0;
        }
    }

    public bool FighterARegen
    {
        get
        {
            if (info == null) return false;
            return (int)info["FighterA"]["Hot"][1] > 0;
        }
    }

    public bool FighterBRegen
    {
        get
        {
            if (info == null) return false;
            return (int)info["FighterB"]["Hot"][1] > 0;
        }
    }
}
