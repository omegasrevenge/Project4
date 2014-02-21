using UnityEngine;

public class EnergyConjurationController : ActorControlls
{
    public float Delay = 1.5f;
    public float EmitTime = 1f;
    public float FadeTime = 0.1f;

    void Update () 
    {
	    if (Delay > 0f)
	    {
	        Delay -= Time.deltaTime;
	        return;
	    }

	    if (EmitTime > 0f)
        {
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(true);
            EmitTime -= Time.deltaTime;
            CanShowDamage = true;
	        return;
	    }

        if (FadeTime > 0f)
        {
            FadeTime -= Time.deltaTime;
            return;
        }

	    BattleEngine.Current.Actor = null;
        Destroy(gameObject);
    }
}
