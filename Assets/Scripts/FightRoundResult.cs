using UnityEngine;
using System.Collections;

public class FightRoundResult : MonoBehaviour 
{
	public enum Player{A, B};

	public Player PlayerTurn; //Whos player's turn is it going to be now?
	public int PlayerAHealth;
	public int PlayerBHealth;
	public int SkillID;
	public int Turn;
}
