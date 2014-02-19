using UnityEngine;
using System.Collections;

public class FightRoundResult 
{
	public enum Player{A, B};

	public int MonsterAHP = 1;
	public int MonsterBHP = 1;
	public Player PlayerTurn = Player.A;
	public int Turn = -1;
	public string SkillName = "Default";
	public int Damage = 1;
	public int DoT = 0;
	public int HoT = 0;
	public bool ConA = false;
    public bool ConB = false;
    public bool BuffA = false;
    public bool BuffB = false;
    public bool DotA = false;
    public bool DotB = false;
    public bool HotA = false;
    public bool HotB = false;
    public bool EVDA = false;
    public bool EVDB = false;
}