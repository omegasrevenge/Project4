using System.Collections.Generic;
using UnityEngine;

public class HealStation : PointOfInterest
{
    private const string Prefab = "Prefabs/POIs/healingspot";
    private const string HideStr = "HideHeal";
    private const string LightStr = "point_light";
    private const string ModelStr = "healing_station";

    private GameObject _light;
    private Animator _fadingAnimator;

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
        res._fadingAnimator = res.transform.Find(ModelStr).GetComponent<Animator>();
        res.transform.Find(ModelStr).GetComponent<FadeAnimation>().DestroyObject += res.DestroyResource;
        return res;
    }

    protected override void RemovePOI()
    {
        _fadingAnimator.Play(HideStr);
    }

    protected override void EnterRange()
    {
        base.EnterRange();
        _light.SetActive(true);
    }

    protected override void LeaveRange()
    {
        base.LeaveRange();
        _light.SetActive(false);
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
