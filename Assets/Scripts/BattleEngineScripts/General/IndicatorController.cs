﻿using UnityEngine;

public class IndicatorController : MonoBehaviour
{

    private Animation _myAnim;
    private Vector3 _start;
    private float _length = -1f;
    private Vector3 _speed;
    private float _counter;

	// Use this for initialization
	void Start ()
	{
	    _myAnim = GetComponent<Animation>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if (_counter > _length)
	    {
            GetComponent<dfLabel>().Hide();
            return;
	    }
	    if (_counter == 0f)
            transform.position = _start;
        GetComponent<dfLabel>().Show();
	    _counter += Time.deltaTime;
	    transform.position += _speed;
    }

    public void Play(Vector3 startPos, float length, Vector3 speed)
    {
        _start = startPos;
        _length = length;
        _speed = speed;
        _counter = 0f;
    }
}