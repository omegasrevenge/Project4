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
    public DateTime BaseTime;
    public int[,] Resources;
	public Int64 Version;

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
		BaseTime = (DateTime)json["TimeBase"];
		Version = (Int64)json["Version"];
        Resources = new int[7, 5];

        //if (GameManager.Singleton.Player.PlayerID != PlayerID) return;
        JSONObject res = json["Resources"];
        if (res == null) return;

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

        InitSteps = (int)json["InitSteps"];

		CurCreature = new Creature();
        if(InitSteps >= 2) CurCreature.ReadJson(json["CurrentCreature"]);

        Fighting = (bool)json["Fighting"];
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
        var newResult = new FightRoundResult
        {
            MonsterAHP = CurCreature.HP,
            MonsterBHP = CurFight.EnemyCreature.HP,
            PlayerTurn = CurFight.Turn ? FightRoundResult.Player.A : FightRoundResult.Player.B,
            Turn = CurFight.Round,
            ConA = (int)CurFight.Info["FighterA"]["Con"][0] > 0,
            ConB = (int)CurFight.Info["FighterB"]["Con"][0] > 0,
            BuffA = (int)CurFight.Info["FighterA"]["Def"][0] > 0,
            BuffB = (int)CurFight.Info["FighterB"]["Def"][0] > 0,
            DotA = (int)CurFight.Info["FighterA"]["Dot"][0] > 0,
            DotB = (int)CurFight.Info["FighterB"]["Dot"][0] > 0,
            HotA = (int)CurFight.Info["FighterA"]["Hot"][0] > 0,
            HotB = (int)CurFight.Info["FighterB"]["Hot"][0] > 0
        };

        string[] lastResult = CurFight.LastResult.Split(' ');

        if (lastResult.Length != 4) 
            return newResult;

        newResult.SkillName = lastResult[0];
        newResult.Damage = Convert.ToInt32(lastResult[1]);
        newResult.DoT = Convert.ToInt32(lastResult[2]);
        newResult.HoT = Convert.ToInt32(lastResult[3]);

        return newResult;
    }
}