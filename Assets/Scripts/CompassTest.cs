using System.Collections;
using UnityEngine;

public class CompassTest : MonoBehaviour
{
    public MapGrid Grid;
    public GameObject CameraRig;

	private int pois_version = 0;
	public int grid_version = 0;

    public const float MoveSpeed = 1f;
    public const float MoveRadius = 1f;

	public const float RangeRadius = 0.188f/2f;

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

	    if (pois_version != GameManager.Singleton.pois_version)
	    {
		    pois_version = GameManager.Singleton.pois_version;
			CreatePOIs();
	    }

	    if (grid_version != Grid.grid_version)
	    {
		    grid_version = Grid.grid_version;
		    GameManager.Singleton.pois_valid = false;
	    }
    }

	void OnGUI()
	{
		POIsInRange();

		if (GameManager.Singleton.Player.Resources == null)return;

		for (int i = 0; i < 7; i++)
		{
			string z = "" + i + ":";
			for (int j = 0; j < 5; j++)
			{
				z += GameManager.Singleton.Player.Resources[i, j] + " ";
			}
			GUI.Label(new Rect(250, 40+i*20, 200, 20), z);
		}
	}

	private void POIsInRange()
	{
		int inRange = 0;

		GUI.Label(new Rect(400, 10, 200, 20), GameManager.Singleton.lastFarmResult);

		foreach (POI poi in GameManager.Singleton.POIs)
		{
			if (MapUtils.DistanceInKm(poi.Position,LocationManager.GetCurrentPosition()) <= RangeRadius)
			{
                GUI.Label(new Rect(400, 40 + inRange * 40, 200, 20), poi.Name);
			    string btnString = poi.Rsc == "Fight" ? "Fight" : "Farm";
                if (GUI.Button(new Rect(400, 60 + inRange * 40, 100, 20), btnString))
				{
					GameManager.Singleton.PoiFarm(poi);
				}
				inRange++;
			}
		}
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
        foreach (POI poi in GameManager.Singleton.POIs)
        {
            if (poi.instance != null) continue;
            GameObject obj = Resources.Load<GameObject>(path+poi.Type);
            if (obj == null) obj = Resources.Load<GameObject>(path+"Default");
            poi.instance = (GameObject)Instantiate(obj);
	        poi.instance.transform.parent = transform;
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
