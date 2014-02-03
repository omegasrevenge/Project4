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
            if(Owner.CurrentPlayer == FightRoundResult.Player.A)
                _curDir = To.B;
            else
                _curDir = To.A;
        }
        if(_curDir == To.A)
            transform.position = Vector3.Lerp(transform.position, Owner.FriendlyCreature.transform.position, Speed);
        else
            transform.position = Vector3.Lerp(transform.position, Owner.EnemyCreature.transform.position, Speed);

        if (_curDir == To.A)
        {
            if (reached(Owner.FriendlyCreature))
                CanShowDamage = true;
        }
        else
        {
            if (reached(Owner.EnemyCreature))
                CanShowDamage = true;
        }

        if(CanShowDamage)
        {
            _counter += Time.deltaTime;
            if(_counter >= 1f)
            {
                Owner.Actor = null;
                Destroy(gameObject);
            }
        }
	}

    private bool reached(GameObject target) 
    {
        return Mathf.Abs((transform.position - target.transform.position).magnitude) < 1f;
    }
}
