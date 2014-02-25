using UnityEngine;

public class HealthBarController : MonoBehaviour
{

    public const float InterpolationSpeed = 0.035f;

    public dfProgressBar RedBar;
    public dfProgressBar GreenBar;

	private bool _processing;
	private float _delay = 0f;

	void Start ()
    {
        RedBar = GetComponent<dfProgressBar>();
        GreenBar = transform.FindChild("MonsterHealthProgress").GetComponent<dfProgressBar>();
	}
	
	void Update ()
	{
	    GreenBar.Width = RedBar.Width;
	    GreenBar.Height = RedBar.Height+1;

	    if (Mathf.Abs(RedBar.Value - GreenBar.Value) < 0.02f)
        {
            RedBar.Value = GreenBar.Value;
			_processing = false;
            return;
        }

		if(!_processing && RedBar.Value != GreenBar.Value)
		{
			_processing = true;
			_delay = 3f;
		}

		if (_processing && _delay > 0f) 
		{
			_delay -= Time.deltaTime;
			return;
		}

		if(_processing && _delay <= 0f)
        	RedBar.Value = RedBar.Value * (1-InterpolationSpeed) + GreenBar.Value * InterpolationSpeed;
	}
}
