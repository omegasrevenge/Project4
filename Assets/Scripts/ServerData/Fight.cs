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

    public JSONObject Info;

    public void ReadJson(JSONObject json)
    {
        Info = json;
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
			if (Info == null) return false;
			return (int)Info["FighterA"]["Con"][1] > 0;
        }
    }

    public bool FighterBConfused
    {
        get
        {
			if (Info == null) return false;
			return (int)Info["FighterB"]["Con"][1] > 0;
        }
    }

    public bool FighterABuffed
    {
        get
        {
			if (Info == null) return false;
			return (int)Info["FighterA"]["Def"][1] > 0;
        }
    }

    public bool FighterBBuffed
    {
        get
        {
			if (Info == null) return false;
			return (int)Info["FighterB"]["Def"][1] > 0;
        }
    }

    public bool FighterABurning
    {
        get
        {
			if (Info == null) return false;
			return (int)Info["FighterA"]["Dot"][1] > 0;
        }
    }

    public bool FighterBBurning
    {
        get
        {
			if (Info == null) return false;
			return (int)Info["FighterB"]["Dot"][1] > 0;
        }
    }

    public bool FighterARegen
    {
        get
        {
			if (Info == null) return false;
			return (int)Info["FighterA"]["Hot"][1] > 0;
        }
    }

    public bool FighterBRegen
    {
        get
        {
			if (Info == null) return false;
			return (int)Info["FighterB"]["Hot"][1] > 0;
        }
    }
}
