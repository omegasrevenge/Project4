using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class Map : SceneRoot3D 
{
    private const string Prefab = "Scene/map";
    private const string GridStr = "grid";
    private const string MapRigStr = "map_rig";
    private const string ArrowStr = "arrow";
    private const string RadarStr = "radar";

    private MapGrid _grid;
    private Transform _mapRig;
    private Transform _arrow;
    private Animator _radar;
    private Dictionary<Player, PlayerOnMap> _playersOnMap = new Dictionary<Player, PlayerOnMap>();
    private BaseOnMap _base;

    private MovableViewport _viewport;
    private float _viewportScrollState;

    public bool init = false;

    private int pois_version = 0;
    private int grid_version = 0;

    public const float MoveSpeed = 1f;
    public const float MoveRadius = 1f;
    public const float RangeRadius = 0.5f;

    public GUIObjectMapUI View
    {
        get { return _gui as GUIObjectMapUI; }
    }

    public static Map Create()
    {
        GameObject obj = (GameObject)Instantiate(Resources.Load<GameObject>(Prefab));
        if (obj)
        {
            Map map = obj.GetComponent<Map>();
            if(map)
                map.Init();
            return map;
        }
        return null;
    }

    public void Init()
    {
        _mapRig = transform.Find(MapRigStr);
        _grid = _mapRig.Find(GridStr).GetComponent<MapGrid>();
        _arrow = _mapRig.Find(ArrowStr);
        _radar = _mapRig.Find(RadarStr).GetComponent<Animator>();
        _base = BaseOnMap.Create(_grid, _mapRig);
        _base.Tap = OnTapShowMarker;
    }

    public float ViewportPhase
    {
        get
        {
            return _viewportScrollState;
        }

        set
        {
            if (_viewport == null && Camera)
                _viewport = Camera.GetComponent<MovableViewport>();
            if (_viewport == null) return;
            value = Mathf.Clamp(value, 0f, GUIObjectMapUI.MaxViewportScroll);
            _viewportScrollState = _viewport.phase = value;
        }
    }

    public override void AttachGUI(Component gui)
    {
        base.AttachGUI(gui);
        (gui as GUIObjectMapUI).Init(this);
    }

    public void UpdatePlayers()
    {
        foreach (Player player in GameManager.Singleton.PlayersOnMap.Where(player => !_playersOnMap.ContainsKey(player)))
        {
            PlayerOnMap pom = PlayerOnMap.Create(player, _grid, _mapRig);
            if (pom != null)
            {
                _playersOnMap.Add(player, pom);
                pom.Tap = OnTapShowMarker;
            }
        }

        //RemoveOldPlayers
        List<Player> oldItems = new List<Player>();
        foreach (KeyValuePair<Player, PlayerOnMap> p in _playersOnMap.Where(pom => !GameManager.Singleton.PlayersOnMap.Contains(pom.Key)))
        {
            p.Value.RemovePlayerFromMap();
            oldItems.Add(p.Key);
        }
        foreach (Player oldItem in oldItems)
        {
            _playersOnMap.Remove(oldItem);
        }
    }

    public void UpdatePOIs()
    {
        foreach (POI poi in GameManager.Singleton.POIs)
        {
            if (poi.Type == POI.POIType.Resource)
            {
                Resource res = Resource.Create(poi, _grid, _mapRig);
                res.Tap = OnTapShowMarker;
            }
            else if (poi.Type == POI.POIType.Fight)
            {
                if (poi.CanFarm)
                {
                    Spectre spec = Spectre.Create(poi, _grid, _mapRig);
                    spec.Tap = OnTapShowMarker;
                }
            }
            else if (poi.Type == POI.POIType.Heal)
            {
                HealStation heal = HealStation.Create(poi, _grid, _mapRig);
                heal.Tap = OnTapShowMarker;
            }
        }
    }

    public void OnTapShowMarker(TouchInput.Touch2D touches)
    {
        View.AddMarker(touches.Owner);
    }

    private void CheckRadar()
    {
        int inRange = 0;

        foreach (POI poi in GameManager.Singleton.POIs)
        {
            if (poi.View)
            {
                poi.View.InRange = MapUtils.Distance(poi.View.ProjPos, _grid.CurrentPosition) <= RangeRadius;
                if (poi.View.InRange)
                    inRange++;      
            }
        }

        foreach (PlayerOnMap pom in _playersOnMap.Values)
        {
            if (pom != null)
            {
                pom.InRange = MapUtils.Distance(pom.ProjPos, _grid.CurrentPosition) <= RangeRadius;
                if (pom.InRange)
                    inRange++;
            }
        }

        if (_base != null)
        {
            _base.InRange = MapUtils.Distance(_base.ProjPos, _grid.CurrentPosition) <= RangeRadius;
            if (_base.InRange)
                inRange++;
        }

        _radar.SetBool("Radar", inRange > 0);
    }

    void Update()
    {
        if (GameManager.Singleton.CurrentGameMode != GameManager.GameMode.Map) return;

        UpdatePlayers();
        UpdatePOIs();
        CheckRadar();

        _arrow.localEulerAngles = new Vector3(90f,LocationManager.GetDirection(),0f);

        //Rotation:
        Vector3 gridRot = _mapRig.eulerAngles;
        _mapRig.eulerAngles = new Vector3(gridRot.x, TouchInput.Singleton.GetRotation(gridRot.y, _mapRig.position, false, true), gridRot.z);

        MapUtils.ProjectedPos newPosition = LocationManager.GetCurrentProjectedPos(_grid.ZoomLevel);
        if ((newPosition - _grid.CurrentPosition).Magnitude < MoveRadius)
            _grid.CurrentPosition = newPosition;
        else
        {
            _grid.CurrentPosition = newPosition;
        }

        if (grid_version != _grid.grid_version)
        {
            grid_version = _grid.grid_version;
            GameManager.Singleton.pois_valid = false;
        }
    }

    public void SetCreatureInfo(Creature creature)
    {
        View.SetCreatureInfo(creature);
    }

    public void HideMenu()
    {
        View.CloseMenuImmediate();
    }

    public void ShowFightInvation()
    {
        if(View)
            View.ShowFightInvation();
    }

    public void HideFightInvation()
    {
        if (View)
            View.HideFightInvation();
    }

    public bool MenuControlsEnabled
    {
        get
        {
            if (View)
                return View.MenuControlsEnabled;
            return false;
        }
        set
        {
            if (View)
                View.MenuControlsEnabled = value;
        }
    }
}
