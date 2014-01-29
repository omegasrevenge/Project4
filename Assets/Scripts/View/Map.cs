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
    }

    private void OnGUI()
    {
        if (GameManager.Singleton.CurrentGameMode != GameManager.GameMode.Map || BattleEngine.Current != null) return;
        POIsInRange();
        //ShowResouces();
        if (GameManager.Singleton.LoggedIn && GameManager.Singleton.DummyUI)
        {
            if (GUI.Button(new Rect(270, 40, 120, 50), "<color=white><size=20>Set Base</size></color>"))
            {
                GameManager.Singleton.SendBasePosition();
                Debug.Log("Current Base Poition: " + LocationManager.GetCurrentPosition());
                MoveBase();
            }
        }
    }

    private void ShowResouces()
    {
        if (GameManager.Singleton.Player.Resources == null) return;

        GUIStyle curGuiStyle = new GUIStyle { fontSize = 30 };
        curGuiStyle.normal.textColor = Color.white;

        for (int i = 0; i < 7; i++)
        {
            string z = "" + i + ":";
            for (int j = 0; j < 5; j++)
            {
                z += GameManager.Singleton.Player.Resources[i, j] + " ";
            }
            GUI.Label(new Rect(20, 40 + i * 40, 200, 20), z, curGuiStyle);
        }
    }

    private void POIsInRange()
    {
        int inRange = 0;

        GUIStyle curGuiStyle = new GUIStyle { fontSize = 30 };
        curGuiStyle.normal.textColor = Color.white;

        GUI.Label(new Rect(500, 10, 200, 20), GameManager.Singleton.lastFarmResult, curGuiStyle);

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

    public void CreatePOIs()
    {
        string path = "Prefabs/POIs/";
        foreach (POI poi in GameManager.Singleton.POIs)
        {
            if(poi.Type == POI.POIType.Resource)
                Resource.Create(poi, _grid);
            else
                Resource.Create(poi, _grid);
        }
    }

    private void CreateBase()
    {
        string path = "Prefabs/POIs/";

        if (GameManager.Singleton.Player.baseInstance == null)
        {
            //GameObject obj = Resources.Load<GameObject>(path + "base");
            //GameObject curGameObject = (GameObject)Instantiate(obj);
            //GameManager.Singleton.Player.baseInstance = curGameObject;
            //curGameObject.transform.parent = _mapRig;
            //float lon = GameManager.Singleton.Player.BasePosition.x;
            //float lat = GameManager.Singleton.Player.BasePosition.y;
            //MapUtils.ProjectedPos projPos = MapUtils.GeographicToProjection(new Vector2(lon, lat), _grid.ZoomLevel);
            //PointOfInterest comp = curGameObject.AddComponent<PointOfInterest>();
            //comp.ProjPos = projPos;
            //comp.Grid = _grid;
            return;
        }
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

        if (pois_version != GameManager.Singleton.pois_version)
        {
            pois_version = GameManager.Singleton.pois_version;
            CreatePOIs();
        }

        if (GameManager.Singleton.LoggedIn)
        {
            if (!init)
            {
                CreateBase();
                init = true;
            }

            MoveBase();
        }
    }

}
