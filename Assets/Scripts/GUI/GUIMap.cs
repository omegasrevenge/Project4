using UnityEngine;
using System.Collections;

public class GUIMap : MonoBehaviour {

	public GameObject CameraRig;
	public MapGrid Grid;

	private int pois_version = 0;
	public int grid_version = 0;

	public const float MoveSpeed = 1f;
	public const float MoveRadius = 1f;

	public const float RangeRadius = 0.188f / 2f;

	void OnGUI()
	{
		POIsInRange();

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
			GUI.Label(new Rect(300, 40 + i * 40, 200, 20), z, curGuiStyle);
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
			if (MapUtils.DistanceInKm(poi.Position, LocationManager.GetCurrentPosition()) <= RangeRadius)
			{
				GUI.Label(new Rect(500, 40 + inRange * 60, 200, 20), poi.Name, curGuiStyle);
				string btnString = poi.Rsc == "Fight" ? "Fight" : "Farm";
				if (GUI.Button(new Rect(500, 80 + inRange * 60, 120, 50), btnString))
				{
					GameManager.Singleton.PoiFarm(poi);
				}
				inRange++;
			}
		}
		if (MapUtils.DistanceInKm(GameManager.Singleton.Player.BasePosition, LocationManager.GetCurrentPosition()) <= RangeRadius)
		{
			GUI.Label(new Rect(500, 40 + inRange * 60, 200, 20), "Base", curGuiStyle);
			if (GUI.Button(new Rect(500, 80 + inRange * 60, 120, 50), "Visit Base"))
			{
				Debug.Log("is now visiting the Base!");
			}
			inRange++;
		}
	}

	public void CreatePOIs()
	{
		string path = "Prefabs/POIs/";
		foreach (POI poi in GameManager.Singleton.POIs)
		{
			if (poi.instance != null) continue;
			GameObject obj = Resources.Load<GameObject>(path + poi.Type);
			if (obj == null) obj = Resources.Load<GameObject>(path + "Default");
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

	void Update()
	{
		//Rotation:
		Vector3 cameraRot = CameraRig.transform.eulerAngles;
		CameraRig.transform.eulerAngles = new Vector3(cameraRot.x, TouchInput.Singleton.GetRotation(cameraRot.y, CameraRig.transform.position, true), cameraRot.z);

		MapUtils.ProjectedPos newPosition = LocationManager.GetCurrentProjectedPos(Grid.ZoomLevel);
		if ((newPosition - Grid.CurrentPosition).Magnitude < MoveRadius)
			Grid.CurrentPosition = MapUtils.ProjectedPos.Lerp(Grid.CurrentPosition, newPosition,
				Time.deltaTime * MoveSpeed);
		else
		{
			Grid.CurrentPosition = newPosition;
		}

		if (grid_version != Grid.grid_version)
		{
			grid_version = Grid.grid_version;
			GameManager.Singleton.pois_valid = false;
		}

		if (pois_version != GameManager.Singleton.pois_version)
		{
			pois_version = GameManager.Singleton.pois_version;
			CreatePOIs();
		}
	}
}
