using System.Linq;
using UnityEngine;
using System.Collections;

public class FixCreditsReset : MonoBehaviour
{

	public dfButton AbortButton;
    public dfTweenVector3 ScrollingCredits;
    public dfTweenVector3 AbortButtonTween;
    public Vector3 XEndPos;

	// Use this for initialization
	void Start ()
	{
		foreach (dfButton button in FindObjectsOfType<dfButton>().Where(button => button.name == "Button_EndCredits"))
		{
			AbortButton = button;
		}
        ScrollingCredits = GetComponent<dfTweenVector3>();
        foreach (dfTweenVector3 tween in AbortButton.GetComponents<dfTweenVector3>().Where(tween => tween.TweenName == "BlendOut"))
        {
            XEndPos = tween.EndValue;
            AbortButtonTween = tween;
        }
	}

	// Update is called once per frame
	void Update ()
	{
	    if (!ScrollingCredits.IsPlaying) return;
	    if (!AbortButtonTween.IsPlaying && AbortButton.Position != XEndPos) return;

	    ScrollingCredits.Stop();
	    ScrollingCredits.Reset();
	}
}
