using System;
using System.Linq;
using UnityEngine;
using System.Collections;

public class POI 
{
    public enum POIType
    {
        Unexpected,
        Resource,
        Fight,
        Heal,
        Tower
    }
    private const string FightStr = "Fight";
    private const string HealStr = "Heal";
    private const string TowerStr = "Tower";


    public string POI_ID;
    public string Name;
    public Vector2 Position;
    public int RespawnInMinutes;
	//private bool _canFarm;
	public DateTime NextFarm;
    public string ResourceType;
    public string RealType;
	public string MapPos;
    public PointOfInterest View;

    public bool CanFarm { get { return GameManager.Singleton.GetServerTime() > NextFarm; } }
    public float CooldownInSeconds{get {return Mathf.Max(0f,(float)(NextFarm-GameManager.Singleton.GetServerTime()).TotalSeconds);}}

    public float CooldownInPercent
    {
        get
        {
            if (RespawnInMinutes == 0)
                return 0;
            return CooldownInSeconds/(RespawnInMinutes*60f);
        }
    }

    public string GetCooldownString()
    {
        int cd = Mathf.FloorToInt(CooldownInSeconds);
        int s = Mathf.FloorToInt(cd%60f);
        int m = Mathf.FloorToInt((cd/60f)%60);
        int h = Mathf.FloorToInt(cd/3600f);
        return string.Format("{0:00}:{1:00}:{2:00}", h, m, s);
    }

    public static POI ReadJson(JSONObject json)
    {
        POI poi = new POI();
        poi.POI_ID = (string)json["ID"];
        poi.Name = (string)json["Name"];
        poi.Position = new Vector2((float)json["Lon"], (float)json["Lat"]);
        poi.RespawnInMinutes = (int)json["Respawn"];
        poi.ResourceType = (string)json["Rsc"];
        poi.RealType = (string)json["Type"];
		//poi._canFarm = (bool)json["CanFarm"];
		poi.NextFarm = (DateTime)json["NextFarm"];
		poi.MapPos= (string)json["MapPos"];
		//Debug.Log ("CanFarm:"+poi.CanFarm+" NextFarm:"+poi.NextFarm);
        return poi;
    }

    public POIType Type
    {
        get
        {
            if (Resource.ResourceTypes.Any(t => ResourceType.Contains(t)))
                return POIType.Resource;
            if(ResourceType == FightStr)
                return POIType.Fight;
            if (ResourceType == HealStr)
                return POIType.Heal;
            if (ResourceType == TowerStr)
                return  POIType.Tower;
            return POIType.Unexpected;
        }
    }

	//sadly calculating this can be incorrect due tu rounding issues
	/*public string MapPos()
	{
		MapUtils.ProjectedPos curProjectedPos = MapUtils.GeographicToProjection(Position, 17);
		return ("" + curProjectedPos.X + "," + curProjectedPos.Y);
	}*/
}
