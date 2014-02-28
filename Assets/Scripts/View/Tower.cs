using UnityEngine;
using System.Collections;

public class Tower : PointOfInterest
{
    private const string Prefab = "POIs/Vengea_Tower";


    public static Tower Create(POI poi, MapGrid grid, Transform root)
    {
        if (poi.Name != "Sony Center") return null;
        if (poi.View != null)
            return poi.View as Tower;
        Tower res;
        GameObject obj = (GameObject)Instantiate(Resources.Load<GameObject>(Prefab));
        res = obj.GetComponent<Tower>();
        res.transform.parent = root;
        Animator animator = obj.GetComponent<Animator>();
        res.Init(poi, grid, animator);
        return res;
    }
}
