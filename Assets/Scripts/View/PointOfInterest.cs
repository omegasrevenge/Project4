using UnityEngine;

public class PointOfInterest : ObjectOnMap
{
    private const string HideStr = "Hide";

    public MapUtils.ProjectedPos ProjPos;
    private MapGrid _grid;
    protected POI Poi;
    private bool _inRange;
    private bool _removing;
    protected Animator _animator;

    public bool InRange
    {
        get { return _inRange; }
        set
        {
            if (_inRange == value) return;
            _inRange = value;
            if(value)
                EnterRange();
            else
                LeaveRange();
        }
    }

    public void Init(POI poi, MapGrid grid, Animator animator = null)
    {
        Poi = poi;
        if(Poi.View != null)
            Destroy(gameObject);
        gameObject.name = Poi.Type + "_" + Poi.POI_ID;
        Poi.View = this;
        _grid = grid;
        ProjPos = MapUtils.GeographicToProjection(new Vector2(poi.Position.x, poi.Position.y), grid.ZoomLevel);
        Vector2 pos = _grid.GetPosition(ProjPos);
        transform.localPosition = new Vector3(pos.x, 0.001f, pos.y);
        _animator = animator;
        Enabled = false;
    }

    protected virtual void Update()
    {
        if (Poi == null || !GameManager.Singleton.POIs.Contains(Poi))
            RemovePOI();
        Vector2 pos = _grid.GetPosition(ProjPos);
        transform.localPosition = new Vector3(pos.x, 0.001f, pos.y);
    }

    protected virtual void RemovePOI()
    {
        if (_animator)
        {
            if (!_removing)
            {
                _animator.SetTrigger(HideStr);
                _removing = true;
            }
        }
        else
            DestroyObject();
    
    }

    public void DestroyObject()
    {
        Poi.View = null;
        Destroy(gameObject);
    }

    public override void Execute()
    {
        GameManager.Singleton.PoiFarm(Poi);
    }

    public override float GetCooldownProgress()
    {
        return Poi.CooldownInPercent;
    }

    public override string GetCooldownString()
    {
        return Poi.GetCooldownString();
    }
}
