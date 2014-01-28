using UnityEngine;
using System.Collections;

public class Map3D : MonoBehaviour {
    private const string Prefab = "Scene/spectresintro";

    public static Map3D Create()
    {
        GameObject obj = (GameObject)Instantiate(Resources.Load<GameObject>(Prefab));
        if (obj)
        {
            Map3D map = obj.GetComponent<Map3D>();
            return map;
        }
        return null;
    }
}
