using UnityEngine;
using System.Collections;

public abstract class ActorControlls : MonoBehaviour
{
    [HideInInspector]
    public bool CanShowDamage = false;
    [HideInInspector]
    public bool AnimationFinished = true;
    [HideInInspector]
	public BattleEngine Owner;
}