using System;
using UnityEngine;


[Serializable]
public class Player
{
    public enum Faction { VENGEA, NCE}

    public Faction CurrentFaction;
    public string PlayerID;
    public string Name;
    public Vector2 Position;
    public Vector2 BasePosition;
    public Int64 BaseTime;
    public int[,] Resources;

    public bool Fighting;
    public Fight CurFight;

    public int InitSteps;
    public int[] creatureIDs;
    public Creature CurCreature;

    public void ReadJson(JSONObject json)
    {
        CurrentFaction = (Faction)(int)json["Faction"];
        PlayerID = (string)json["PId"];
        Name = (string)json["Name"];
        Position = new Vector2((float)json["Lon"], (float)json["Lat"]);
        BasePosition = new Vector2((float)json["BaseLon"], (float)json["BaseLat"]);
        BaseTime = (Int64)json["TimeBase"];
        Resources = new int[7, 5];

        if (GameManager.Singleton.Player.PlayerID != PlayerID)
            return;
        JSONObject res = json["Resources"];
        for (int i = 0; i < res.Count; i++)
        {
            JSONObject element = res[i];
            for (int j = 0; j < element.Count; j++)
            {
                Resources[i, j] = (int)element[j];
            }
        }

        JSONObject jcids = json["CreatureIds"];
        creatureIDs = new int[jcids.Count];
        for (int i = 0; i < jcids.Count; i++)
        {
            creatureIDs[i] = (int)jcids[i];
        }

	    CurCreature = new Creature();
        CurCreature.ReadJson(json["CurrentCreature"]);
        Fighting = (bool)json["Fighting"];
        InitSteps = (int)json["InitSteps"];
        if (Fighting)
        {
            JSONObject curF = json["Fight"];
            if (curF.type == JSONObject.Type.OBJECT)
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

    public BattleInit GetBattleInit()
    {
        BattleInit newBattle = new BattleInit
        {
            MonsterAElement = CurCreature.BaseElement,
            MonsterBElement = CurFight.EnemyCreature.BaseElement,
            MonsterAName = CurCreature.Name,
            MonsterBName = CurFight.EnemyCreature.Name,
            MonsterAHealth = CurCreature.HP,
            MonsterBHealth = CurFight.EnemyCreature.HP,
            MonsterAMaxHealth = CurCreature.HPMax,
            MonsterBMaxHealth = CurFight.EnemyCreature.HPMax,
            MonsterALevel = CurCreature.Level,
            MonsterBLevel = CurFight.EnemyCreature.Level,
            BaseMeshA = CurCreature.ModelID,
            BaseMeshB = CurFight.EnemyCreature.ModelID
        };

        newBattle.FirstTurnIsPlayer = CurFight.Turn ? FightRoundResult.Player.A : FightRoundResult.Player.B;
        return newBattle;
    }

    public FightRoundResult GetResult()
    {
		if (CurFight == null)
						return new FightRoundResult ();
        FightRoundResult newResult = new FightRoundResult
        {
            PlayerTurn = CurFight.Turn ? FightRoundResult.Player.A : FightRoundResult.Player.B,
            Turn = CurFight.Round
        };

        string[] lastResult = CurFight.LastResult.Split(' ');

        if (lastResult.Length != 4)
        {
            Debug.Log("LAST RESULT LENGTH != 4. lastresult = " + CurFight.LastResult);
            return newResult;
        }
        newResult.SkillName = lastResult[0];
        newResult.Damage = Convert.ToInt32(lastResult[1]);
        newResult.DoT = Convert.ToInt32(lastResult[2]);
        newResult.HoT = Convert.ToInt32(lastResult[3]);
        return newResult;
    }
}