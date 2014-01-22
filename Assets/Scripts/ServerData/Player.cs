using System;
using UnityEngine;


[Serializable]
public class Player
{
    public string PlayerID;
    public string Name;
	public Vector2 Position;
	public Vector2 BasePosition;
	public Int64 BaseTime;
    public int[,] Resources;

	public bool Fighting;
	public Fight CurFight;

	public GameObject baseInstance;
    public int InitSteps;
	public int[] creatureIDs;
	public Creature CurCreature;

    public void ReadJson(JSONObject json)
    {
        PlayerID = (string)json["PId"];
        Name = (string)json["Name"];
		Position = new Vector2((float)json["Lon"], (float)json["Lat"]);
		BasePosition = new Vector2((float)json["BaseLon"], (float)json["BaseLat"]);
	    BaseTime = (Int64) json["TimeBase"];
        Resources = new int[7,5];
        JSONObject res = json["Resources"];
        for (int i = 0; i < res.Count; i++)
        {
            JSONObject element = res[i];
            for (int j = 0; j < element.Count; j++)
            {
                Resources[i, j] = (int) element[j];
            }
        }
	    
		JSONObject jcids = json["CreatureIds"];
		creatureIDs = new int[jcids.Count];
	    for (int i = 0; i < jcids.Count; i++)
		{
			creatureIDs[i] = (int)jcids[i];
	    }

		CurCreature.ReadJson(json["CurrentCreature"]);
        Fighting = (bool)json["Fighting"];
        InitSteps = (int) json["InitSteps"];
		if(Fighting)
		{
			JSONObject curF = json["Fight"];
			if(curF.type == JSONObject.Type.OBJECT) 
			{
				CurFight = new Fight();
				CurFight.ReadJson(curF);
			}
		}
		else
		{
			CurFight = null;
		}
    }

	public void UpdateBattle()
	{
		if(!Fighting)
		{
			if(BattleEngine.Current != null)
				BattleEngine.Current.Fighting = false;
			return; 
		}

		if(BattleEngine.Current == null)
		{
			BattleInit newBattle = new BattleInit();
			
			newBattle.MonsterAElement = CurCreature.BaseElement;
			newBattle.MonsterBElement = CurFight.EnemyCreature.BaseElement;
			
			newBattle.MonsterAName = CurCreature.Name;
			newBattle.MonsterBName = CurFight.EnemyCreature.Name;
			
			newBattle.MonsterAHealth = CurCreature.HP;
			newBattle.MonsterBHealth = CurFight.EnemyCreature.HP;
			
			newBattle.MonsterAMaxHealth = CurCreature.HPMax;
			newBattle.MonsterBMaxHealth = CurFight.EnemyCreature.HPMax;
			
			newBattle.MonsterALevel = CurCreature.Level;
			newBattle.MonsterBLevel = CurFight.EnemyCreature.Level;
			
			if(CurFight.Turn) 
			{
				newBattle.FirstTurnIsPlayer = FightRoundResult.Player.A;
			}
			else
			{
				newBattle.FirstTurnIsPlayer = FightRoundResult.Player.B;
			}
			
			BattleEngine.CreateBattle(newBattle);
            BattleEngine.Current.Fighting = !CurFight.Finished;
		}
		else
		{
			FightRoundResult newResult = new FightRoundResult();
			
			if(CurFight.Turn) 
			{
				newResult.PlayerTurn = FightRoundResult.Player.A;
			}
			else
			{
				newResult.PlayerTurn = FightRoundResult.Player.B;
			}
			
			newResult.PlayerAHealth = CurCreature.HP;
			newResult.PlayerBHealth = CurFight.EnemyCreature.HP;
			//newResult.SkillID = 1; <--------------------------------TODO NOT YET IMPLEMENTED
			newResult.Turn = CurFight.Round;
			BattleEngine.Current.Result = newResult;
		    BattleEngine.Current.Fighting = !CurFight.Finished;
		}
	}
}
