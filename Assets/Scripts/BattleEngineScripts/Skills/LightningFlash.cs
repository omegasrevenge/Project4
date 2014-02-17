using UnityEngine;

public class LightningFlash : MonoBehaviour
{

    public float MinTime = 0.05f;
    public Light MyLight;

    private float _counter;

	void Start ()
	{
	    MyLight = BattleEngine.CurrentGameObject.transform.FindChild("Light").GetComponent<Light>();
	}
	
	void Update ()
	{
	    if (MyLight == null) return;
	    if ((Time.time - _counter) <= MinTime) return;
	    MyLight.enabled = Random.Range(0, 2) > 0;

	    _counter = Time.time;
	}
}
