using UnityEngine;
using System.Collections;

public class LaserControl : ActorControlls {

	public float FinalHeadScale = 3f;
	public float HeadScalingSpeed = 3f;
	public float FadeOutTime = 2f;
	public float LifeTime = 5f;
    public float CastingDelay = 3f;

	private float _counter = 0f;
	private Transform _head;
	private Transform _beam;
	private bool _nowCasting = false;

    public float CurAlpha
    {
        get { return _beam.GetComponent<MeshRenderer>().material.color.a; }
    }

	void Start () 
	{
		AnimationFinished = false;
		_head = transform.FindChild("Head");
		_beam = transform.FindChild("Beam");
		SetNewAlpha(0f);
	    _counter -= CastingDelay;
	}

	void Update () 
	{
	    if (!_nowCasting && _counter >= 0f)
        {
            _nowCasting = true;
            SetNewAlpha(1f);
	    }

		_counter += Time.deltaTime;
		if (!_nowCasting) return;

		if(_head.localScale.x < FinalHeadScale*0.95)
		{
			_head.localScale = Vector3.Lerp(_head.localScale, Vector3.one*FinalHeadScale, HeadScalingSpeed*Time.deltaTime);
		}
		else
		{
			if(!_beam.gameObject.activeSelf)
			{
				_beam.gameObject.SetActive(true);
				CanShowDamage = true;
			}
		}

		if(_beam.gameObject.activeSelf)
            SetNewAlpha(CurAlpha - (Time.deltaTime / FadeOutTime));

		if(_counter >= LifeTime)
		{
			Owner.Actor = null;
			Destroy(gameObject);
		}
	}

    public void SetNewAlpha(float value)
    {
        _head.GetComponent<MeshRenderer>().material.color = new Color(_head.GetComponent<MeshRenderer>().material.color.r,
                                                                      _head.GetComponent<MeshRenderer>().material.color.g,
                                                                      _head.GetComponent<MeshRenderer>().material.color.b,
                                                                      value);

        _beam.GetComponent<MeshRenderer>().material.color = new Color(_beam.GetComponent<MeshRenderer>().material.color.r,
                                                                      _beam.GetComponent<MeshRenderer>().material.color.g,
                                                                      _beam.GetComponent<MeshRenderer>().material.color.b,
                                                                      value);
    }
}
