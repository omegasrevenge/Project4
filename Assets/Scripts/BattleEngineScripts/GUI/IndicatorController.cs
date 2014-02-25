using UnityEngine;

public class IndicatorController : MonoBehaviour
{
    public bool CatchResult = false;

    private Vector3 _start;
    private float _length = -1f;
    private Vector3 _speed;
    private float _counter;
    private float _delayCounter = 0f;

    public bool IsPlaying
    {
        get { return GetComponent<dfLabel>().IsVisible; }
    }

    void Start()
    {
        transform.rotation = BattleEngine.Current.Camera.transform.rotation;
    }

    void Update ()
    {
	    if (_counter >= _length)
	    {
	        if (CatchResult) BattleEngine.Current.CatchInProcess = false;
            GetComponent<dfLabel>().Hide();
            return;
	    }
        if (_delayCounter > 0f)
        {
            _delayCounter -= Time.deltaTime;
            return;
        }
        if (_counter == 0f)
            transform.position = _start;
        GetComponent<dfLabel>().Show();
	    _counter += Time.deltaTime;
	    transform.position += _speed;
    }

    public void Play(Vector3 startPos, float length, Vector3 speed, float delay = 0f)
    {
        _delayCounter = delay;
        _start = startPos;
        _length = length;
        _speed = speed;
        _counter = 0f;
    }
}
