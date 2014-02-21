using System.Collections.Generic;
using UnityEngine;

public class ConjurationController : ActorControlls
{
    public float Delay = 1.1f;
    public float EmitTime = 5f;
    public float FadeTime = 6f;

    public List<ParticleEmitter> Emitters; 

	void Start ()
	{
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
	        foreach (ParticleEmitter emitter in Emitters)
                emitter.emit = true;
	        EmitTime -= Time.deltaTime;
	        return;
	    }

        CanShowDamage = true;
        foreach (ParticleEmitter emitter in Emitters)
            emitter.emit = false;

        if (FadeTime > 0f)
        {
            FadeTime -= Time.deltaTime;
            return;
        }

	    BattleEngine.Current.Actor = null;
        Destroy(gameObject);
    }
}
