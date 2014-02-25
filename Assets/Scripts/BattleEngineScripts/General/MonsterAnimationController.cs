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

    public void SetState(string state, bool value = true)
    {
        GetComponent<Animator>().SetBool(state, value);
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
		if (GetComponent<Animator> ().layerCount == 0) 
		{
			Debug.LogError("UNITY ANIMATOR COMPILE ERROR!");
			return;	
		}
        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("FightIdle"))
            _idleTime += Time.deltaTime;

        if (_idleTime < 10f) return;

        if (Random.Range(0, 2) > 0) return;

        string curChannel = BattleEngine.Current.FriendlyCreature == gameObject ? BattleSounds.FriendlySoundChannel : BattleSounds.EnemySoundChannel;
        if (SoundController.GetChannel(curChannel) == null || !SoundController.GetChannel(curChannel).isPlaying)
            SoundController.PlaySound(gameObject.name.Contains("Wolf") ? BattleSounds.WolfIdle : BattleSounds.GiantIdle, curChannel);

        Trigger(Random.Range(0, 2) > 0 ? "Idle_std_var1" : "idle_std_var2");
        _idleTime = 0f;
    }

    public void Trigger(string triggerName)
    {
		GetComponent<Animator>().SetTrigger(triggerName);
    }

}
