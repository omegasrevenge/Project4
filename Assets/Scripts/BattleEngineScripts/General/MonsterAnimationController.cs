using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using System.Collections;

public class MonsterAnimationController : MonoBehaviour
{
    private const string Exhausted = "Exhausted";
    private const string Bored = "Bored";
    private const string Attack = "Attack";
    private const string SpellRoarCast = "SpellRoarCast";

    private bool  _isPlaying = false;
    private float _delay = 0f;
    private string _animName = "";
    private float _idleTime = 0f;

    public void DoAnim(string animName, float delay = 0f)
    {
        _isPlaying = true;
        _delay = delay;
        _animName = animName;
    }
	
	void Update ()
	{
        DifferentIdlesController();

	    if (_delay > 0f)
	    {
	        _delay -= Time.deltaTime;
	        return;
	    }

        if(!_isPlaying) return;

	    _idleTime = 0f;
        StartSkill();
	    _isPlaying = false;
	}

    public void DifferentIdlesController()
    {
        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            _idleTime += Time.deltaTime;

        if (_idleTime >= 5f)
            if (Random.Range(0, 2) > 0) Trigger(Bored);

        _idleTime = 0f;
    }

    public void StartSkill()
    {
        switch (_animName)
        {
            case "Default":
                Trigger(SpellRoarCast);
                break;
            default:
                Debug.LogError("MonsterAnimatorController of " + gameObject.name + " does not known how to animate " + _animName + ". Please implement it.");
                break;
        }
    }

    public void Trigger(string triggerName)
    {
        GetComponent<Animator>().SetTrigger(triggerName);
    }

}
