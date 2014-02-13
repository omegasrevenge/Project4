using System;
using UnityEngine;


[Serializable]
public class Fight
{
    public Fighter FighterA = new Fighter();
    public Fighter FighterB = new Fighter();
    public int SkillID;
	public int Round;
	public string LastResult;
	public Creature EnemyCreature;
	public bool Turn;
    public bool Started;
    public bool Finished;

    public JSONObject Info;

    public class Fighter
    {
        public string PId;
        public int CId;
        public int[] damage_over_time = new int[2];
        public int[] heal_over_time = new int[2];
        public int[] confusion = new int[2];
        public int[] exdefense = new int[2];
        public int evade;
        public int weaken;
        public bool skip;

        public JSONObject Info;

        public void ReadJson(JSONObject json)
        {
            //Debug.Log("Fighter:" + json.ToString());
            Info = json;
            PId = (string) json["PId"];
            CId = (int) json["CId"];
            damage_over_time[0] = (int) json["Dot"][0];
            damage_over_time[1] = (int)json["Dot"][1];
            heal_over_time[0] = (int)json["Hot"][0];
            heal_over_time[1] = (int)json["Hot"][1];
            confusion[0] = (int)json["Con"][0];
            confusion[1] = (int)json["Con"][1];
            exdefense[0] = (int)json["Def"][0];
            exdefense[1] = (int)json["Def"][1];
            evade = (int) json["EVD"];
            weaken = (int)json["WEK"];
            skip = (bool)json["SKP"];
            //Debug.Log("FighterPid:" + PId);
        }
    }

    public void ReadJson(JSONObject json)
    {
        Info = json;
        FighterA.ReadJson(json["FighterA"]);
        FighterB.ReadJson(json["FighterB"]);
        LastResult = (string)json["LastResult"];
        Round = (int)json["Round"];
        EnemyCreature = new Creature();
        EnemyCreature.ReadJson(json["EnemyCreature"]);
        Turn = (bool)json["Turn"];
        Started = (bool) json["Started"];
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
