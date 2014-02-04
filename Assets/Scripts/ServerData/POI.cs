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

    private static readonly string[] ResourceTypes = {"Energy", "Nature", "Fire", "Water", "Storm" };
    private const string FightStr = "Fight";
    private const string HealStr = "Heal";


    public string POI_ID;
    public string Name;
    public Vector2 Position;
    public int Respawn;
	public bool CanFarm;
	public DateTime NextFarm;
    public string Resource;
    public string RealType;
    public PointOfInterest View;

    public bool FarmAllowed
    {
        get
        {
            return !(DateTime.Compare(DateTime.Now, NextFarm)<0);
        }
    }

    public double GetTimeUntilFarmAllowed
    {
        get 
        {
            if(FarmAllowed)
                return 0f;
            return (NextFarm-DateTime.Now).TotalSeconds;
        }
    }

    public static POI ReadJson(JSONObject json)
    {
        POI poi = new POI();
        poi.POI_ID = (string)json["ID"];
        poi.Name = (string)json["Name"];
        poi.Position = new Vector2((float)json["Lon"], (float)json["Lat"]);
        poi.Respawn = (int)json["Respawn"];
        poi.Resource = (string)json["Rsc"];
        poi.RealType = (string)json["Type"];
		poi.CanFarm = (bool)json["CanFarm"];
		poi.NextFarm = (DateTime)json["NextFarm"];
		//Debug.Log ("CanFarm:"+poi.CanFarm+" NextFarm:"+poi.NextFarm);
        return poi;
    }

    public POIType Type
    {
        get
        {
            if (ResourceTypes.Any(t => Resource.Contains(t)))
                return POIType.Resource;
            if(Resource == FightStr)
                return POIType.Fight;
            if (Resource == HealStr)
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
