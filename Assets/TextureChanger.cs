using UnityEngine;
using System.Collections;

public class TextureChanger : MonoBehaviour
{
    public Texture Vengea;
    public Texture VengeaNormal;
    public Texture Nce;  
    public Texture NceNormal;

	void Start ()
	{
	    Material material = GetComponent<MeshRenderer>().material;
        if (GameManager.Singleton.Player.CurrentFaction == Player.Faction.VENGEA)
	    {
            material.SetTexture("_MainTex", Vengea);
            material.SetTexture("_BumpMap", VengeaNormal);
	    }
	    else
	    {
            material.SetTexture("_MainTex", Nce);
            material.SetTexture("_BumpMap", NceNormal);
	    }
	}
}
