using UnityEngine;

public class LocationManager : MonoBehaviour {

    private static LocationManager _instance;
    //Latitude = 52.50451
    //Longitude = 13.39699
    public float Latitude = 52.50451f;
    public float Longitude = 13.39699f;

    public static LocationManager Singleton
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("LocationManager");
                _instance = obj.AddComponent<LocationManager>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
        {
            Debug.LogError("Second instance of LocationManager.");
            Destroy(this);
            return;
        }
        Init();
    }

    private void Init()
    {
        Input.location.Start();
        Input.compass.enabled = true;

        
    }

    private void Update()
    {
#if !UNITY_EDITOR
        if (Input.location.status != LocationServiceStatus.Running) return;
            SetLocation();
#else
        Longitude += Input.GetAxis("Horizontal")*Time.deltaTime*0.001f;
        Latitude += Input.GetAxis("Vertical") * Time.deltaTime *0.001f;
#endif
    }


    private void SetLocation()
    {
        Longitude = Input.location.lastData.longitude;
        Latitude = Input.location.lastData.latitude;
    }

    public static Vector2 GetCurrentPosition()
    {
        return new Vector2(Singleton.Longitude, Singleton.Latitude);
    }

    public static MapUtils.ProjectedPos GetCurrentProjectedPos(int zoomLevel)
    {
        return MapUtils.GeographicToProjection(GetCurrentPosition(), zoomLevel);
    }
}
