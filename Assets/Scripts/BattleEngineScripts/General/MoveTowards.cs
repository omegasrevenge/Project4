using System;
using UnityEngine;

public class MoveTowards : MonoBehaviour
{
    private const float LerpSpeed = 3f;

    public bool Playing = false;
    public Transform Target;
    public bool AtTarget = false;
	
	void Update ()
	{
	    if (!Playing) return;

	    float parentToTarget = Mathf.Abs((Target.position - transform.parent.position).magnitude);
	    float thisToTarget = Mathf.Abs((Target.position - transform.position).magnitude);

        transform.position = Vector3.Lerp(transform.position, Target.position, ((parentToTarget - thisToTarget) * LerpSpeed / parentToTarget) * Time.deltaTime);

        if(reachedTarget()) Stop();
	}

    public void Play(Transform target)
    {
        AtTarget = false;
        GetComponent<dfButton>().Show();
        Playing = true;
        Target = target;
    }

    public void Stop()
    {
        AtTarget = true;
        Playing = false;
        GetComponent<dfButton>().Hide();
    }

    private bool reachedTarget()
    {
        return Mathf.Abs((transform.position - Target.position).magnitude) < 3.5f;
    }
}
