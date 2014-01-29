using System.Collections.Generic;
using UnityEngine;

public class HealStation : PointOfInterest
{
    private const string Prefab = "Prefabs/POIs/healingspot";
    private const string HideResourceStr = "HidePOI";
    private const string InRangeStr = "InRange";

    private Animator _animator;

    public static HealStation Create(POI poi, MapGrid grid, Transform root)
    {
        if (poi.View != null)
            return poi.View as HealStation;
        HealStation res;
        GameObject obj = (GameObject) Instantiate(Resources.Load<GameObject>(Prefab));
        res = obj.GetComponent<HealStation>();
        res.transform.parent = root;
        res.Init(poi, grid);
        res._animator = res.GetComponent<Animator>();
        return res;
    }

    protected override void RemovePOI()
    {
        _animator.Play(HideResourceStr);
    }

    protected override void EnterRange()
    {
        _animator.SetBool(InRangeStr, InRange);
    }

    protected override void LeaveRange()
    {
        _animator.SetBool(InRangeStr, InRange);
    }

    public void DestroyResource()
    {
        Poi.View = null;
        Destroy(gameObject);
    }
}
