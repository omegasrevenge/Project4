using UnityEngine;
using System.Collections;

public class FireballController : ActorControlls 
{
    public enum To { None, A, B}

    private To _curDir = To.None;
    private float _counter = 0f;
    public float Speed = 0.03f;

	void Update () 
    {
        if(_curDir == To.None)
        {
            _curDir = BattleEngine.Current.CurrentPlayer == FightRoundResult.Player.A ? To.B : To.A;
        }
	    transform.position = Vector3.Lerp(transform.position, _curDir == To.A ? 
            BattleEngine.Current.FriendlyCreature.transform.position : BattleEngine.Current.EnemyCreature.transform.position, Speed);

	    if (_curDir == To.A)
        {
            if (reached(BattleEngine.Current.FriendlyCreature))
                CanShowDamage = true;
        }
        else
        {
            if (reached(BattleEngine.Current.EnemyCreature))
                CanShowDamage = true;
        }

	    if (!CanShowDamage) return;
	    _counter += Time.deltaTime;

	    if (!(_counter >= 1f)) return;
	    BattleEngine.Current.Actor = null;
	    Destroy(gameObject);
    }

    private bool reached(GameObject target) 
    {
        return Mathf.Abs((transform.position - target.transform.position).magnitude) < 1f;
    }
}
