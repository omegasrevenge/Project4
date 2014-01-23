using UnityEngine;
using System.Collections;

public abstract class ActorControlls : MonoBehaviour 
{
	public bool CanShowDamage = false;
	public bool AnimationFinished = true;
	public BattleEngine Owner;
}