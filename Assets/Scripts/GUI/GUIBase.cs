using System.Net.NetworkInformation;
using UnityEngine;
using System.Collections;

public class GUIBase : MonoBehaviour {

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
		string path = "Prefabs/";

		if (GameManager.Singleton.Player.baseInstance == null)
		{
			GameObject obj = Resources.Load<GameObject>(path + "Base");
			GameObject curGameObject = (GameObject)Instantiate(obj);
			curGameObject.transform.parent = transform;
			float lon = GameManager.Singleton.Player.BasePosition.x;
			float lat = GameManager.Singleton.Player.BasePosition.y;
			MapUtils.ProjectedPos projPos = MapUtils.GeographicToProjection(new Vector2(lon, lat), Grid.ZoomLevel);
			LocationTestComp comp = curGameObject.AddComponent<LocationTestComp>();
			comp.setText("Base");
			comp.ProjPos = projPos;
			comp.Grid = Grid;
			GameManager.Singleton.Player.baseInstance = curGameObject;
			return;
		}

	}

	void Update () {
	
	}
}
