using UnityEngine;

public class LocationManager : MonoBehaviour {

    private static LocationManager _instance;
    public float Latitude = 52f;
    public float Longitude = 13f;

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
#endif
    }


    private void SetLocation()
    {
        Longitude = Input.location.lastData.longitude;
        Latitude = Input.location.lastData.latitude;
    }

    public Vector2 GetCurrentPosition()
    {
        return new Vector2(Longitude, Latitude);
    }

    public MapUtils.ProjectedPos GetCurrentProjectedPos(int zoomLevel)
    {
        return MapUtils.GeographicToProjection(GetCurrentPosition(), zoomLevel);
    }
}
