using UnityEngine;
using System.Collections;

public class FightRoundResult 
{
	public enum Player{A, B};

	public Player PlayerTurn = Player.A;
	public int Turn = -1;
	public string SkillName = "Laser";
	public int Damage = 1;
	public int DoT = 0;
	public int HoT = 0;
}