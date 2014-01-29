using System.Collections.Generic;
using UnityEngine;

public class HealStation : PointOfInterest
{
    private const string Prefab = "Prefabs/POIs/healingspot";
    //private const string HideResourceStr = "HidePOI";
    private const string LightStr = "point_light";
    private const string InRangeStr = "InRange";

    private GameObject _light;
    public static HealStation Create(POI poi, MapGrid grid, Transform root)
    {
        if (poi.View != null)
            return poi.View as HealStation;
        HealStation res;
        GameObject obj = (GameObject) Instantiate(Resources.Load<GameObject>(Prefab));
        res = obj.GetComponent<HealStation>();
        res.transform.parent = root;
        res.Init(poi, grid);
        res._light = res.transform.Find(LightStr).gameObject;
        return res;
    }

    protected override void EnterRange()
    {
        _light.SetActive(true);
        //_animator.SetBool(InRangeStr, InRange);
    }

    protected override void LeaveRange()
    {
        _light.SetActive(false);
        //_animator.SetBool(InRangeStr, InRange);
    }

    public void DestroyResource()
    {
        Poi.View = null;
        Destroy(gameObject);
    }
}
