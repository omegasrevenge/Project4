using UnityEngine;

public class LocationManager : MonoBehaviour {

    private static LocationManager _instance;

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

    private void Start()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Debug.LogError("Second instance of LocationManager.");
    }

    public Vector2 GetCurrentPosition()
    {
        return new Vector2(13.39699f, 52.50451f);
    }
}
