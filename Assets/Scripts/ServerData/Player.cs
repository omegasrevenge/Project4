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
    public bool Firewall;
    public Fight CurFight;

    public int InitSteps;
    [SerializeField]
    public TutorialStep Tutorial;
    public int[] creatureIDs;
    public Creature CurCreature;

    public void ReadJson(JSONObject json)
    {
		if(json==null||json["PId"]==null||((string)json["PId"]).Length<8) {return;} //not a player

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
        int unused = 0;
        for (int i = 0; i < jcids.Count; i++)
            if ((int) jcids[i] == 0) unused++;
        creatureIDs = new int[jcids.Count-unused];
        unused = 0;
        for (int i = 0; i < jcids.Count; i++)
        {
            if ((int) jcids[i] == 0)
            {
                unused++;
                continue;
            }
            creatureIDs[i+unused] = (int)jcids[i];
        }

        InitSteps = (int)json["InitSteps"];
        Tutorial = (TutorialStep)(int)json["Tutorial"];
        Firewall = (bool) json["Firewall"];
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
		GameObject nB = GameObject.Find ("BattleInit");
		if (nB != null)
			return nB.GetComponent<BattleInit> ();

        nB = new GameObject("BattleInit");

        BattleInit newBattle = nB.AddComponent<BattleInit>();

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
        newBattle.BaseMeshA = CurCreature.ModelID;
        newBattle.BaseMeshB = CurFight.EnemyCreature.ModelID;
        newBattle.FirstTurnIsPlayer = CurFight.Turn ? FightRoundResult.Player.A : FightRoundResult.Player.B;

        return newBattle;
    }

    public FightRoundResult GetResult()
    {
        GameObject nR = new GameObject("FightRoundResult @ Time = "+GameManager.Singleton.GetServerTime().ToLocalTime());

        FightRoundResult newResult = nR.AddComponent<FightRoundResult>();

		if (CurFight == null)
			return newResult;

        newResult.MonsterAHP = CurCreature.HP;
        newResult.MonsterBHP = CurFight.EnemyCreature.HP;
        newResult.PlayerTurn = CurFight.Turn ? FightRoundResult.Player.A : FightRoundResult.Player.B;
        newResult.Turn = CurFight.Round;
        newResult.ConA = (int) CurFight.Info["FighterA"]["Con"][0] > 0;
        newResult.ConB = (int) CurFight.Info["FighterB"]["Con"][0] > 0;
        newResult.BuffA = (int) CurFight.Info["FighterA"]["Def"][0] > 0;
        newResult.BuffB = (int) CurFight.Info["FighterB"]["Def"][0] > 0;
        newResult.DotA = (int) CurFight.Info["FighterA"]["Dot"][0] > 0;
        newResult.DotB = (int) CurFight.Info["FighterB"]["Dot"][0] > 0;
        newResult.HotA = (int) CurFight.Info["FighterA"]["Hot"][0] > 0;
        newResult.HotB = (int) CurFight.Info["FighterB"]["Hot"][0] > 0;
        newResult.EVDA = (bool) CurFight.Info["FighterA"]["evaded"];
        newResult.EVDB = (bool) CurFight.Info["FighterB"]["evaded"];

        string[] lastResult = CurFight.LastResult.Split(' ');

        if (lastResult.Length < 6) 
            return newResult;

        newResult.SkillName = lastResult[0];
        newResult.Damage = Convert.ToInt32(lastResult[1]);
        newResult.DoT = Convert.ToInt32(lastResult[2]);
        newResult.HoT = Convert.ToInt32(lastResult[3]);
        newResult.DefaultAttackElement1 = (GameManager.ResourceElement)Convert.ToInt32(lastResult[4]);
        newResult.DefaultAttackElement2 = (GameManager.ResourceElement)Convert.ToInt32(lastResult[5]);
        return newResult;
    }
}