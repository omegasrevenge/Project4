using UnityEngine;

// ReSharper disable once CheckNamespace
public class MonsterAnimationController : MonoBehaviour
{
    private bool  _isPlaying;
    private float _delay;
    private string _animName = "";
    private float _idleTime;

    public void DoAnim(string animName, float delay = 0f)
    {
        if (string.IsNullOrEmpty(animName))
        {
            Debug.LogError("string.IsNullOrEmpty(animName) == true!");
            return;
        }

        _isPlaying = true;
        _delay = delay;
        _animName = animName;
    }
	
// ReSharper disable once UnusedMember.Local
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
        Trigger(_animName);
	    _isPlaying = false;
	}

    public void DifferentIdlesController()
    {
        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("FightIdle"))
            _idleTime += Time.deltaTime;

        if (_idleTime < 5f) return;

        if (Random.Range(0, 2) > 0) Trigger("FightIdle_to_Agitated");
        _idleTime = 0f;
    }

    public void Trigger(string triggerName)
    {
		GetComponent<Animator>().SetTrigger(triggerName);
    }

}
