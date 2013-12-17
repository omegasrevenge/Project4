using System.Collections;
using UnityEngine;

public class CompassTest : MonoBehaviour
{
    public MapGrid Grid;
    public GameObject CameraRig;

    public const float MoveSpeed = 1f;
    public const float MoveRadius = 1f;

    private POI[] POIs = new POI[0];

    public float pois_timeQ;

    void Update()
    {
        //Rotation:
        Vector3 cameraRot = CameraRig.transform.eulerAngles;
        CameraRig.transform.eulerAngles = new Vector3(cameraRot.x, TouchInput.Singleton.GetRotation(cameraRot.y, CameraRig.transform.position, true), cameraRot.z);

        //New Position:
        MapUtils.ProjectedPos newPosition = LocationManager.GetCurrentProjectedPos(Grid.ZoomLevel);
        if ((newPosition - Grid.CurrentPosition).Magnitude < MoveRadius)
            Grid.CurrentPosition = MapUtils.ProjectedPos.Lerp(Grid.CurrentPosition, newPosition,
                Time.deltaTime*MoveSpeed);
        else
        {
            Grid.CurrentPosition = newPosition;
            StartCoroutine(RequestLocations(LocationManager.GetCurrentPosition()));
        }

        if (!GameManager.Singleton.LoggedIn) return;

        if (!Grid.pois_valid && pois_timeQ <= 0)
            StartCoroutine(GetPois());

        pois_timeQ -= Time.deltaTime;
    }

    private IEnumerator GetPois()
    {
        pois_timeQ = 3;
        Grid.pois_valid = true;
        Vector2 pos = LocationManager.GetCurrentPosition();
        WWW request = new WWW(GameManager.Singleton.GetSessionURL("getpois") + "&lon=" + pos.x + "&lat=" + pos.y);

        yield return request;

        JSONObject json = JSONParser.parse(request.text);
        if (!CheckResult(json)) yield break;
        JSONObject data = json["data"];
        JSONObject pois = data["POIs"];
        POI[] tmpPOIs = new POI[pois.Count];
        Debug.Log(pois.Count);
        for (int i = 0; i < tmpPOIs.Length; i++)
        {
            tmpPOIs[i] = new POI();
            tmpPOIs[i].ReadJson(pois[i]);
        }

        for (int i = 0; i < POIs.Length; i++)
        {
            bool found = false;
            for (int j = 0; j < tmpPOIs.Length; j++)
            {
                if (POIs[i].POI_ID == tmpPOIs[j].POI_ID)
                {
                    tmpPOIs[j] = POIs[i];
                    found = true;
                    break;
                }
            }
            if (!found && POIs[i].instance!=null)
            {
                //Debug.Log("kaputt");
                Destroy(POIs[i].instance);
            }
        }

        POIs = tmpPOIs;
        CreatePOIs();
    }

    public bool CheckResult(JSONObject json)
    {
        if (!(bool)json["result"])
        {
            Debug.LogError("RPC Fail: " + json["error"]);
            return false;
        }
        return true;
    }

    private void CreatePOIs()
    {
        string path = "Prefabs/POIs/";
        foreach (POI poi in POIs)
        {
            if (poi.instance != null) continue;
            GameObject obj = Resources.Load<GameObject>(path+poi.Type);
            if (obj == null) obj = Resources.Load<GameObject>(path+"Default");
            poi.instance = (GameObject)Instantiate(obj);
            float lon = poi.Position.x;
            float lat = poi.Position.y;
            MapUtils.ProjectedPos projPos = MapUtils.GeographicToProjection(new Vector2(lon, lat), Grid.ZoomLevel);
            LocationTestComp comp = poi.instance.AddComponent<LocationTestComp>();
            comp.setText(poi.Name);
            comp.ProjPos = projPos;
            comp.Grid = Grid;
        }
    }

    //private void CreateMediadesignHochschule(JSONObject json)
    //{
    //    for (int i = 0; i < json.Count; i++)
    //    {
    //        if ((string)json[i]["name"] == "Mediadesign Hochschule")
    //        {
    //            GameObject obj = new GameObject();
    //            SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
    //            renderer.sprite = Resources.Load<Sprite>("mark");
    //            float lon = (float)json[i]["geometry"]["location"]["lng"];
    //            float lat = (float)json[i]["geometry"]["location"]["lat"];
    //            Debug.Log(lon + " " + lat);
    //            MapUtils.ProjectedPos projPos = MapUtils.GeographicToProjection(new Vector2(lon, lat), Grid.ZoomLevel);
    //            LocationTestComp comp = obj.AddComponent<LocationTestComp>();
    //            comp.ProjPos = projPos;
    //            comp.Grid = Grid;
    //        }
    //    }
    //}

    private IEnumerator RequestLocations(Vector2 pos)
    {
        string url =
            string.Format(
                "https://maps.googleapis.com/maps/api/place/search/json?location={0},{1}&radius=1000&sensor=true&key=AIzaSyD3jfeMZK1SWfRFDgMfxn_zrGRSjE7S8Vg&language=de",
                pos.y, pos.x);
        WWW www = new WWW(url);
        yield return www;
        JSONObject json = JSONParser.parse(www.text);
        //CreateMediadesignHochschule(json["results"]);
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
