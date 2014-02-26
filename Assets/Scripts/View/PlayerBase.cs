using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : SceneRoot3D
{
    private const string Prefab = "Scene/base";

    public List<GameObject> CreatureMeshes;

    private GameObject crtrMsh;

    public static PlayerBase Create()
    {
        GameObject obj = (GameObject) Instantiate(Resources.Load<GameObject>(Prefab));
        if (obj)
        {
            PlayerBase playerBase = obj.GetComponent<PlayerBase>();
            if (playerBase)
                playerBase.Init();
            return playerBase;
        }
        return null;
    }

    private void Init()
    {
        UpdateCreature(GameManager.Singleton.Player.CurCreature);
    }

    public void UpdateCreature(Creature creature)
    {
        if (crtrMsh != null)
            crtrMsh.SetActive(false);

        crtrMsh = CreatureMeshes[creature.ModelID];
        crtrMsh.SetActive(true);
		crtrMsh.GetComponent<MonsterStats>().Init(creature.BaseElement, creature.ModelID == 0, true);
    }
}