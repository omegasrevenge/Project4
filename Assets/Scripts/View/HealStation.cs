using UnityEngine;

public class HealStation : PointOfInterest
{
    private const string Prefab = "POIs/healingspot";
    private const string LightStr = "point_light";
    private const string ModelStr = "healing_station";

    private GameObject _light;

    public static HealStation Create(POI poi, MapGrid grid, Transform root)
    {
        if (poi.View != null)
            return poi.View as HealStation;
        HealStation res;
        GameObject obj = (GameObject) Instantiate(Resources.Load<GameObject>(Prefab));
        res = obj.GetComponent<HealStation>();
        res.transform.parent = root;
        Animator animator = res.transform.Find(ModelStr).GetComponent<Animator>();
        res.Init(poi, grid, animator);
        res._light = res.transform.Find(LightStr).gameObject;
        
        res.transform.Find(ModelStr).GetComponent<FadeAnimation>().DestroyObject += res.DestroyObject;
        return res;
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

    override public void OnTap(TouchInput.Touch2D touch2D)
    {
        GameManager.Singleton.PoiFarm(Poi);
    }

}
