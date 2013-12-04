using System.Collections;
using UnityEngine;

public class CompassTest : MonoBehaviour
{
    public float Latitude;
    public float Longitude;
    public MapGrid Grid;
    public GameObject CameraRig;

    public const float MoveSpeed = 1f;
    public const float MoveRadius = 1f;


    void Awake()
    {
        Latitude = 52.50451f;
        Longitude = 13.39699f;

        Input.location.Start();
        Input.compass.enabled = true;

        StartCoroutine(RequestLocations(Latitude, Longitude));

    }

    void Update()
    {
        //Rotation:
        Vector3 cameraRot = CameraRig.transform.eulerAngles;
        CameraRig.transform.eulerAngles = new Vector3(cameraRot.x, TouchInput.Singleton.GetRotation(cameraRot.y, CameraRig.transform.position, true), cameraRot.z);

        //Activate Location Service:
#if UNITY_EDITOR

#else

        if (Input.location.status != LocationServiceStatus.Running) return;
        SetLocation();
#endif

        //New Position:
        MapUtils.ProjectedPos newPosition = MapUtils.GeographicToProjection(new Vector2(Longitude, Latitude), Grid.ZoomLevel);
        if ((newPosition - Grid.CurrentPosition).Magnitude < MoveRadius)
            Grid.CurrentPosition = MapUtils.ProjectedPos.Lerp(Grid.CurrentPosition, newPosition,
                                                              Time.deltaTime * MoveSpeed);
        else
            Grid.CurrentPosition = newPosition;
    }


    private void SetLocation()
    {
        Longitude = Input.location.lastData.longitude;
        Latitude = Input.location.lastData.latitude;
    }



    private void CreateMediadesignHochschule(JSONObject json)
    {
        Debug.Log("1");
        for (int i = 0; i < json.Count; i++)
        {
            if ((string)json[i]["name"] == "Mediadesign Hochschule")
            {
                Debug.Log("2");
                GameObject obj = new GameObject();
                SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
                renderer.sprite = Resources.Load<Sprite>("mark");
                float lon = (float)json[i]["geometry"]["location"]["lng"];
                float lat = (float)json[i]["geometry"]["location"]["lat"];
                Debug.Log(lon + " " + lat);
                MapUtils.ProjectedPos projPos = MapUtils.GeographicToProjection(new Vector2(lon, lat), Grid.ZoomLevel);
                LocationTestComp comp = obj.AddComponent<LocationTestComp>();
                comp.ProjPos = projPos;
                comp.Grid = Grid;
            }
        }
    }

    private IEnumerator RequestLocations(float lat, float lon)
    {
        string url =
            string.Format(
                "https://maps.googleapis.com/maps/api/place/search/json?location={0},{1}&radius=1000&sensor=true&key=AIzaSyD3jfeMZK1SWfRFDgMfxn_zrGRSjE7S8Vg&language=de",
                lat, lon);
        WWW www = new WWW(url);
        yield return www;
        JSONObject json = JSONParser.parse(www.text);
        CreateMediadesignHochschule(json["results"]);
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            Debug.Log("Pause App.");
            Input.location.Stop();
            Input.compass.enabled = false;
        }
        else
        {
            Debug.Log("Resume App.");
            Input.location.Start();
            Input.compass.enabled = true;
        }

    }

}
