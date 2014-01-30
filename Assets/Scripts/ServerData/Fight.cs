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
    public bool confused = false;
    public bool defBoosted = false;
	
	public void ReadJson(JSONObject json)
	{
		LastResult = (string)json["LastResult"];
		Round = (int)json["Round"];
		EnemyCreature = new Creature();
		EnemyCreature.ReadJson(json["EnemyCreature"]);
		Turn = (bool)json["Turn"];
        Finished = (bool)json["Finished"];
        confused = (int)json["FighterA"]["Con"] > 0;
        defBoosted = (int)json["FighterA"]["Def"] > 0;
	}
}
