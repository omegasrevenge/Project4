using System.Collections.Generic;
using UnityEngine;

public class PlayerOnMap : TouchObject, IObjectOnMap
{
    private const string PrefabVengea = "POIs/agent";
    private const string PrefabNCE = "POIs/interference";
    private const string HideStr = "Hide";
    private const string InRangeStr = "InRange";

    public Player playerData;
    private MapGrid _grid;
    private bool _inRange;
    public MapUtils.ProjectedPos ProjPos;

    private Animator _animator;
    private bool _removing;

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

    public static PlayerOnMap Create(Player player, MapGrid grid, Transform root)
    {
        PlayerOnMap playerOnMap;
        GameObject obj = (GameObject)Instantiate(Resources.Load<GameObject>(PrefabVengea));
        playerOnMap = obj.GetComponent<PlayerOnMap>();
        playerOnMap.transform.parent = root;
        playerOnMap.gameObject.name = "player_" + player.Name;
        playerOnMap._grid = grid;
        playerOnMap.playerData = player;
        playerOnMap._animator = playerOnMap.GetComponent<Animator>();
        playerOnMap.SetPositionOnMap();

        return playerOnMap;
    }

    void Update()
    {
        SetPositionOnMap();
    }

    private void SetPositionOnMap()
    {
        ProjPos = MapUtils.GeographicToProjection(playerData.Position, _grid.ZoomLevel);
        Vector2 pos = _grid.GetPosition(ProjPos);
        transform.localPosition = new Vector3(pos.x, 0.001f, pos.y);
    }

    public void RemovePlayerFromMap()
    {
        if (_removing)
            return;
        _animator.SetTrigger(HideStr);
        _removing = true;
    }

    private void EnterRange()
    {
        _animator.SetBool(InRangeStr, InRange);
    }

    private void LeaveRange()
    {
        _animator.SetBool(InRangeStr, InRange);
    }

    //Don't rename! It shares the animator with resources
    private void DestroyObject()
    {
        Destroy(gameObject);
    }

    public void Execute()
    {
        //Do something
    }

    public Vector2 GetScreenPosition()
    {
        return ViewController.Singleton.Camera3D.WorldToScreenPoint(transform.position);
    }
}
