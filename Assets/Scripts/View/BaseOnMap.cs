using System.Collections.Generic;
using UnityEngine;

public class BaseOnMap : ObjectOnMap
{
    private const string Prefab = "POIs/base";
    private const string InRangeStr = "InRange";

    private static BaseOnMap _instance;
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

    public static BaseOnMap Singleton
    {
        get
        {
            if (_instance != null)
                return _instance;
            return null;
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
        baseOnMap.ProjPos = MapUtils.GeographicToProjection(GameManager.Singleton.Player.BasePosition, grid.ZoomLevel);
        baseOnMap._animator = baseOnMap.GetComponent<Animator>();
        baseOnMap.Enabled = false;
        baseOnMap.SetPositionOnMap();
        _instance = baseOnMap;
        return baseOnMap;
    }

    void Update()
    {
        SetPositionOnMap();
    }

    public void SetProjPos()
    {
        ProjPos = MapUtils.GeographicToProjection(GameManager.Singleton.Player.BasePosition, _grid.ZoomLevel);
    }

    private void SetPositionOnMap()
    {
        Vector2 pos = _grid.GetPosition(ProjPos);
        transform.localPosition = new Vector3(pos.x, 0.001f, pos.y);
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

    //override public void OnTap(TouchInput.Touch2D touch2D)
    //{
    //    GameManager.Singleton.SwitchGameMode(GameManager.GameMode.Base);
    //}

    public override void Execute()
    {
        GameManager.Singleton.SwitchGameMode(GameManager.GameMode.Base);
    }
}
