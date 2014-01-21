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
	
	public void ReadJson(JSONObject json)
	{
		LastResult = (string)json["LastResult"]; //TODO aus lastresult wird skillid ausgelesen
		Round = (int)json["Round"];
		EnemyCreature = new Creature();
		EnemyCreature.ReadJson(json["EnemyCreature"]);
		Turn = (bool)json["Turn"];
	}
}
