using UnityEngine;
using System.Collections;

public class MonsterAnimationController : MonoBehaviour
{
    public const float AnimationLength = 7f;

    public Animator MyAnimator;

    private bool  _isPlaying = false;
    private float _counter   = 0f;

    public void DoAttackAnim()
    {
        _isPlaying = true;
        _counter = AnimationLength;
        MyAnimator.SetBool("Attack", true);
    }

	void Start () { MyAnimator = GetComponent<Animator>(); }
	
	void Update ()
	{
	    if (_counter > 0f)
	        _counter -= Time.deltaTime;

	    if (_isPlaying && _counter <= 0f)
	    {
	        _isPlaying = false;
            MyAnimator.SetBool("Attack", false);
	    }
	}
}
