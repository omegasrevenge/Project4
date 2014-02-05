using System.Collections.Generic;
using UnityEngine;

public class Spectre : PointOfInterest
{
    private const string Prefab = "POIs/spectre";
    private const string InRangeStr = "InRange";


    public static Spectre Create(POI poi, MapGrid grid, Transform root)
    {
        if (poi.View != null)
            return poi.View as Spectre;
        Spectre res;
        GameObject obj = (GameObject) Instantiate(Resources.Load<GameObject>(Prefab));
        res = obj.GetComponent<Spectre>();
        res.transform.parent = root;
        Animator animator = res.GetComponent<Animator>();
        res.Init(poi, grid, animator);
        return res;
    }

    protected override void Update()
    {
        base.Update();
        if(!Poi.CanFarm)
            RemovePOI();
    }


    protected override void EnterRange()
    {
        base.EnterRange();
        _animator.SetBool(InRangeStr, InRange);
    }

    protected override void LeaveRange()
    {
        base.LeaveRange();
        _animator.SetBool(InRangeStr, InRange);
    }

    override public void OnTap(TouchInput.Touch2D touch2D)
    {
        GameManager.Singleton.PoiFarm(Poi);
    }

}
