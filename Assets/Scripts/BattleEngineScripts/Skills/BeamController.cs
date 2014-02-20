using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class BeamController : ActorControlls
{
    public float Delay = 3f;
    public float EmitTime = 1f;
    public float FadeTime = 3f;
    public List<GameObject> Register; 

    private bool _energy = false;

    public List<ParticleEmitter> Emitters; 

	void Start ()
	{
	    if (gameObject.name.Contains("Energy_Beam"))
        {
            _energy = true;
            Register = new List<GameObject>();
	        foreach (var reg in GetComponentsInChildren<RegisterGameObjects>())
	        {
                reg.gameObject.SetActive(false);
                Register.Add(reg.gameObject);
	        }
	    }
        Emitters = new List<ParticleEmitter>();
	    foreach (var emitter in GetComponentsInChildren<ParticleEmitter>())
        {
            Emitters.Add(emitter);
            emitter.emit = false;
        }
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
            if (_energy)
                foreach (GameObject reg in Register)
                    reg.SetActive(true);
	        foreach (ParticleEmitter emitter in Emitters)
                emitter.emit = true;
	        EmitTime -= Time.deltaTime;
	        return;
	    }

        CanShowDamage = true;
        foreach (ParticleEmitter emitter in Emitters)
            emitter.emit = false;
	    FadeTime -= Time.deltaTime;

	    if (FadeTime > 0f) return;

	    BattleEngine.Current.Actor = null;
        Destroy(gameObject);
    }
}
