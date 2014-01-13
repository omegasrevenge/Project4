using System.Net.NetworkInformation;
using UnityEngine;
using System.Collections;

public class GUIBase : MonoBehaviour
{
	public bool init = false;
	public MapGrid Grid;

	void OnGUI()
	{
		if (GameManager.Singleton.LoggedIn)
		{
			if (GUI.Button(new Rect(10, 180, 120, 50), "Choose as Base!"))
			{
				GameManager.Singleton.SendBasePosition();
				Debug.Log("Current Base Poition: " + LocationManager.GetCurrentPosition());
				MoveBase();
			}
		}
	}

	private void MoveBase()
	{
		MapUtils.ProjectedPos curPos = GameManager.Singleton.Player.baseInstance.GetComponent<LocationTestComp>().ProjPos;

		if (!GameManager.Singleton.Player.BasePosition.Equals(MapUtils.ProjectionToGeographic(curPos)))
		{
			GameManager.Singleton.Player.baseInstance.GetComponent<LocationTestComp>().ProjPos = MapUtils.GeographicToProjection(GameManager.Singleton.Player.BasePosition, Grid.ZoomLevel);
		}
	}

	private void CreateBase()
	{
		string path = "Prefabs/";

		if (GameManager.Singleton.Player.baseInstance == null)
		{
			GameObject obj = Resources.Load<GameObject>(path + "Base");
			GameObject curGameObject = (GameObject)Instantiate(obj);
			GameManager.Singleton.Player.baseInstance = curGameObject;
			curGameObject.transform.parent = transform;
			float lon = GameManager.Singleton.Player.BasePosition.x;
			float lat = GameManager.Singleton.Player.BasePosition.y;
			MapUtils.ProjectedPos projPos = MapUtils.GeographicToProjection(new Vector2(lon, lat), Grid.ZoomLevel);
			LocationTestComp comp = curGameObject.AddComponent<LocationTestComp>();
			comp.setText("Base");
			comp.ProjPos = projPos;
			comp.Grid = Grid;
			return;
		}
	}

	void Update () {
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
