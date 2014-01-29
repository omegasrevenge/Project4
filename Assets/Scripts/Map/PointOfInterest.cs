using UnityEngine;

public class PointOfInterest : MonoBehaviour
{

    public MapUtils.ProjectedPos ProjPos;
    private MapGrid _grid;
    protected POI Poi;
    private bool _inRange;

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

    public void Init(POI poi, MapGrid root)
    {
        Poi = poi;
        Poi.View = this;
        _grid = root;
        ProjPos = MapUtils.GeographicToProjection(new Vector2(poi.Position.x, poi.Position.y), root.ZoomLevel);
    }

    void Update()
    {
        if (!GameManager.Singleton.POIs.Contains(Poi))
            RemovePOI();
        Vector2 pos = _grid.GetPosition(ProjPos);
        transform.localPosition = new Vector3(pos.x, 0.001f, pos.y);
    }

    protected virtual void RemovePOI()
    {
        Destroy(gameObject);
    }

    protected virtual void EnterRange()
    {
        
    }

    protected virtual void LeaveRange()
    {
        
    }
}
