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

    public bool init = false;

    private int pois_version = 0;
    private int grid_version = 0;

    public const float MoveSpeed = 1f;
    public const float MoveRadius = 1f;

    public const float RangeRadius = 0.5f;

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
    }

    //private void OnGUI()
    //{
    //    if (GameManager.Singleton.CurrentGameMode != GameManager.GameMode.Map || BattleEngine.Current != null) return;
    //    CheckPOIsInRange();
    //    ShowResouces();
    //    if (GameManager.Singleton.LoggedIn && GameManager.Singleton.DummyUI)
    //    {
    //        if (GUI.Button(new Rect(270, 40, 120, 50), "<color=white><size=20>Set Base</size></color>"))
    //        {
    //            GameManager.Singleton.SendBasePosition();
    //            Debug.Log("Current Base Poition: " + LocationManager.GetCurrentPosition());
    //            MoveBase();
    //        }
    //    }
    //}

    //private void ShowResouces()
    //{
    //    if (GameManager.Singleton.Player.Resources == null) return;

    //    GUIStyle curGuiStyle = new GUIStyle { fontSize = 30 };
    //    curGuiStyle.normal.textColor = Color.white;

    //    for (int i = 0; i < 7; i++)
    //    {
    //        string z = "" + i + ":";
    //        for (int j = 0; j < 5; j++)
    //        {
    //            z += GameManager.Singleton.Player.Resources[i, j] + " ";
    //        }
    //        GUI.Label(new Rect(20, 40 + i * 40, 200, 20), z, curGuiStyle);
    //    }
    //}

    public void UpdatePlayers()
    {
        foreach (Player player in GameManager.Singleton.PlayersOnMap.Where(player => !_playersOnMap.ContainsKey(player)))
        {
            PlayerOnMap pom = PlayerOnMap.Create(player, _grid, _mapRig);
            if (pom != null)
                _playersOnMap.Add(player, pom);
        }
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
                Resource.Create(poi, _grid, _mapRig);
            else if (poi.Type == POI.POIType.Fight)
                Spectre.Create(poi, _grid, _mapRig);
            else if (poi.Type == POI.POIType.Heal)
                HealStation.Create(poi, _grid, _mapRig);
        }
    }

    private void CheckRadar()
    {
        int inRange = 0;

        //GUIStyle curGuiStyle = new GUIStyle { fontSize = 30 };
        //curGuiStyle.normal.textColor = Color.white;

        //GUI.Label(new Rect(500, 10, 200, 20), GameManager.Singleton.lastFarmResult, curGuiStyle);

        foreach (POI poi in GameManager.Singleton.POIs)
        {
            if (poi.View)
            {
                poi.View.InRange = MapUtils.Distance(poi.View.ProjPos, _grid.CurrentPosition) <= RangeRadius;
                if (poi.View.InRange)
                    inRange++;      
            }
  
                //GUI.Label(new Rect(450, 40 + inRange * 95, 200, 20), poi.Name, curGuiStyle);
                //string btnString = poi.Rsc == "Fight" ? "<color=white><size=20>Fight</size></color>" : "<color=white><size=20>Farm</size></color>";
                //if (GUI.Button(new Rect(450, 80 + inRange * 95, 120, 50), btnString))
                //{
                //    GameManager.Singleton.PoiFarm(poi);
                //}
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

        //if (MapUtils.DistanceInKm(GameManager.Singleton.Player.BasePosition, LocationManager.GetCurrentPosition()) <= RangeRadius)
        //{
        //    //GUI.Label(new Rect(450, 40 + inRange * 95, 200, 20), "Base", curGuiStyle);
        //    //if (GUI.Button(new Rect(450, 80 + inRange * 95, 120, 50), "<color=white><size=20>Visit Base</size></color>"))
        //    //{
        //    //    GameManager.Singleton.SwitchGameMode(GameManager.GameMode.Base);
        //    //}
        //    inRange++;
        //}

        _radar.SetBool("Radar", inRange > 0);
    }

    private void MoveBase()
    {
        //MapUtils.ProjectedPos curPos = GameManager.Singleton.Player.baseInstance.GetComponent<PointOfInterest>().ProjPos;

        //if (!GameManager.Singleton.Player.BasePosition.Equals(MapUtils.ProjectionToGeographic(curPos)))
        //{
        //    GameManager.Singleton.Player.baseInstance.GetComponent<PointOfInterest>().ProjPos = MapUtils.GeographicToProjection(GameManager.Singleton.Player.BasePosition, _grid.ZoomLevel);
        //}
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
        _mapRig.eulerAngles = new Vector3(gridRot.x, TouchInput.Singleton.GetRotation(gridRot.y, _mapRig.position, true, true), gridRot.z);

        MapUtils.ProjectedPos newPosition = LocationManager.GetCurrentProjectedPos(_grid.ZoomLevel);
        if ((newPosition - _grid.CurrentPosition).Magnitude < MoveRadius)
            _grid.CurrentPosition = MapUtils.ProjectedPos.Lerp(_grid.CurrentPosition, newPosition,
                Time.deltaTime * MoveSpeed);
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

}
