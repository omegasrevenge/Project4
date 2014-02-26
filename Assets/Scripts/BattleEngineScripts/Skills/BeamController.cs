using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class BeamController : ActorControlls
{
    public float Delay = 3f;
    public float EmitTime = 1f;
    public float FadeTime = 3f;
    public float MovementSpeed = 1f;
    public List<GameObject> Register;

    public List<ParticleEmitter> Emitters;

    private bool _jumpGiantAttackWithMovement = false;

	void Start ()
	{
	    _jumpGiantAttackWithMovement = gameObject.name.Contains("Fire_Jump") || gameObject.name.Contains("Water_Jump");

        Register = new List<GameObject>();
        Emitters = new List<ParticleEmitter>();

	    foreach (var emitter in GetComponentsInChildren<ParticleEmitter>())
            Emitters.Add(emitter);
        foreach (var reg in GetComponentsInChildren<RegisterGameObjects>())
            Register.Add(reg.gameObject);

        SetEmit(false);
	}
	
	void Update () 
    {
	    if (Delay > 0f)
	    {
	        Delay -= Time.deltaTime;
	        return;
	    }

	    if (EmitTime > 0f)
	    {
	        if (_jumpGiantAttackWithMovement)
	            Register[0].transform.position =
	                Register[0].transform.TransformPoint(new Vector3(0f, 0f, MovementSpeed*Time.deltaTime));
            SetEmit(true);
	        EmitTime -= Time.deltaTime;
	        return;
	    }

        CanShowDamage = true;
        SetEmit(false);

        if (FadeTime > 0f)
        {
            FadeTime -= Time.deltaTime;
            return;
        }

	    BattleEngine.Current.Actor = null;
        Destroy(gameObject);
    }

    public void SetEmit(bool value)
    {
        if(!_jumpGiantAttackWithMovement)
            foreach (GameObject reg in Register)
                reg.SetActive(value);
        foreach (ParticleEmitter emitter in Emitters)
            emitter.emit = value;
    }
}
