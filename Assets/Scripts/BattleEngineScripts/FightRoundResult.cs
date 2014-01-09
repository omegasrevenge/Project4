using UnityEngine;
using System.Collections;

public class FightRoundResult 
{
	public enum Player{A, B};

	public Player PlayerTurn = Player.A; //Whos player's turn is it going to be now?
	public int PlayerAHealth = 300;
	public int PlayerBHealth = 300;
	public int SkillID = 1;
	public int Turn = 1;
}
