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
        Heal
    }
    private const string FightStr = "Fight";
    private const string HealStr = "Heal";


    public string POI_ID;
    public string Name;
    public Vector2 Position;
    public int RespawnInMinutes;
	private bool _canFarm;
	public DateTime NextFarm;
    public string ResourceType;
    public string RealType;
    public PointOfInterest View;

    public bool CanFarm { get{ return _canFarm || DateTime.Now > NextFarm;}}
    public float CooldownInSeconds{get {return Mathf.Max(0f,(float)(NextFarm-DateTime.Now).TotalSeconds);}}
    public float CooldownInPercent{get {return CooldownInSeconds/(RespawnInMinutes*60f);}}

    public static POI ReadJson(JSONObject json)
    {
        POI poi = new POI();
        poi.POI_ID = (string)json["ID"];
        poi.Name = (string)json["Name"];
        poi.Position = new Vector2((float)json["Lon"], (float)json["Lat"]);
        poi.RespawnInMinutes = (int)json["Respawn"];
        poi.ResourceType = (string)json["Rsc"];
        poi.RealType = (string)json["Type"];
		poi._canFarm = (bool)json["CanFarm"];
		poi.NextFarm = (DateTime)json["NextFarm"];
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
            return POIType.Unexpected;
        }
    }

	public string MapPos()
	{
		MapUtils.ProjectedPos curProjectedPos = MapUtils.GeographicToProjection(Position, 17);
		return ("" + curProjectedPos.X + "," + curProjectedPos.Y);
	}
}
