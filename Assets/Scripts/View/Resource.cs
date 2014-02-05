using System.Linq;
using UnityEngine;

public class Resource : PointOfInterest
{
    private const string Prefab = "POIs/resource";
    private const string InRangeStr = "InRange";

    public static readonly string[] ResourceTypes = { "Default", "Energy", "Nature", "Fire", "Water", "Storm" };

    public static Resource Create(POI poi, MapGrid grid, Transform root)
    {
        
        if (poi.View != null)
            return poi.View as Resource;
        Resource res;
        GameObject obj = (GameObject) Instantiate(Resources.Load<GameObject>(Prefab));
        res = obj.GetComponent<Resource>();
        res.transform.parent = root;
        Animator animator = res.GetComponent<Animator>();
        res.Init(poi, grid, animator);
        return res;
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

    public string GetElement()
    {
        if (ResourceTypes.Contains(Poi.ResourceType))
            return Poi.ResourceType;
        return ResourceTypes[0];
    }

}
