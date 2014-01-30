using System.Collections.Generic;
using UnityEngine;

public class BaseOnMap : TouchObject
{
    private const string Prefab = "Prefabs/POIs/base";
    private const string HideResourceStr = "HidePOI";
    private const string InRangeStr = "InRange";

    private Animator _animator;
    private MapGrid _grid;
    private bool _inRange;
    public MapUtils.ProjectedPos ProjPos;

    public bool InRange
    {
        get { return _inRange; }
        set
        {
            if (_inRange == value) return;
            _inRange = value;
            if (value)
                EnterRange();
            else
                LeaveRange();
        }
    }

    public static BaseOnMap Create(MapGrid grid, Transform root)
    {
        BaseOnMap baseOnMap;
        GameObject obj = (GameObject)Instantiate(Resources.Load<GameObject>(Prefab));
        baseOnMap = obj.GetComponent<BaseOnMap>();
        baseOnMap.transform.parent = root;
        baseOnMap.gameObject.name = "base";
        baseOnMap._grid = grid;
        Debug.Log(GameManager.Singleton.Player.BasePosition+ " "+ grid.ZoomLevel);
        baseOnMap.ProjPos = MapUtils.GeographicToProjection(GameManager.Singleton.Player.BasePosition, grid.ZoomLevel);
        baseOnMap._animator = baseOnMap.GetComponent<Animator>();
        baseOnMap.Enabled = false;
        baseOnMap.SetPositionOnMap();
        return baseOnMap;
    }

    void Update()
    {
        SetPositionOnMap();
    }

    private void SetPositionOnMap()
    {
        Vector2 pos = _grid.GetPosition(ProjPos);
        transform.localPosition = new Vector3(pos.x, 0.001f, pos.y);
    }

    protected void RemovePOI()
    {
        _animator.Play(HideResourceStr);
    }

    protected void EnterRange()
    {
        Enabled = true;
        _animator.SetBool(InRangeStr, InRange);

    }

    protected void LeaveRange()
    {
        Enabled = false;
        _animator.SetBool(InRangeStr, InRange);
    }

    public void DestroyResource()
    {
        Destroy(gameObject);
    }

    override public void OnTap(TouchInput.Touch2D touch2D)
    {
        GameManager.Singleton.SwitchGameMode(GameManager.GameMode.Base);
    }
}
