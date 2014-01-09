using UnityEngine;
using System.Collections;

public class DFPanelAnimation : MonoBehaviour
{
    private const float ScrollSpeed = 500f;

    private dfControl _control;
    [SerializeField]
    private bool _playing;

    private Vector2 _targetPos;

	void Awake ()
	{
	    _control = GetComponent<dfControl>();
	    _playing = false;
	    _targetPos = _control.RelativePosition;
	}
	
	void LateUpdate () 
    {
	    if (_playing)
	    {
	        float delta = (_targetPos - (Vector2) _control.RelativePosition).magnitude;
            Debug.Log("d("+delta+")");
	        float t = (ScrollSpeed*Time.deltaTime)/delta;
            Debug.Log(t);
	        Vector2 newPos = Vector2.Lerp(_control.RelativePosition,_targetPos,t);
	        _control.RelativePosition = newPos;
	        if ((Vector2) _control.RelativePosition == _targetPos)
	            _playing = false;
	    }
	}

    /// <summary>
    /// Scrolls the panel with a relative value (panel size in percentage)
    /// </summary>
    /// <param name="right"></param>
    /// <param name="up"></param>
    public void Scroll(float right, float up)
    {
        if (_playing)
            return;

        _targetPos.x = Mathf.RoundToInt(_control.RelativePosition.x + Mathf.Clamp(right, -1f, 1f) * _control.Size.x);
        _targetPos.y = Mathf.RoundToInt(_control.RelativePosition.y + Mathf.Clamp(up, -1f, 1f) * _control.Size.y);
        Debug.Log("Play " + _targetPos + " :: " + _control.RelativePosition);
        _playing = true;
    }
}
