using System.Collections.Generic;
using UnityEngine;

public class Resource : PointOfInterest
{
    private const string Prefab = "Prefabs/POIs/resource";
    private const string HideResourceStr = "HidePOI";
    private const string InRangeStr = "InRange";

    private Animator _animator;

    public static Resource Create(POI poi, MapGrid grid, Transform root)
    {
        
        if (poi.View != null)
            return poi.View as Resource;
        Resource res;
        GameObject obj = (GameObject) Instantiate(Resources.Load<GameObject>(Prefab));
        res = obj.GetComponent<Resource>();
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
        base.EnterRange();
        _animator.SetBool(InRangeStr, InRange);
    }

    protected override void LeaveRange()
    {
        base.LeaveRange();
        _animator.SetBool(InRangeStr, InRange);
    }

    public void DestroyResource()
    {
        Poi.View = null;
        Destroy(gameObject);
    }

    override public void OnTap(TouchInput.Touch2D touch2D)
    {
        GameManager.Singleton.PoiFarm(Poi);
    }
}
