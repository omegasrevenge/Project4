using UnityEngine;

public class HealthBarController : MonoBehaviour
{
    public const float InterpolationSpeed = 0.05f;

    public dfProgressBar RedBar;
    public dfProgressBar GreenBar;

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
            return;
        }
        RedBar.Value = RedBar.Value * (1-InterpolationSpeed) + GreenBar.Value * InterpolationSpeed;
	}
}
