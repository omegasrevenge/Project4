using UnityEngine;
using System.Collections;

public class POI 
{
    public string POI_ID;
    public string Name;
    public Vector2 Position;
    public int Respawn;
    public string Rsc;
    public string Type;

    public GameObject instance;

    public void ReadJson(JSONObject json)
    {
        POI_ID = (string) json["ID"];
        Name = (string) json["Name"];
        Position = new Vector2((float) json["Lon"], (float) json["Lat"]);
        Respawn = (int)json["Respawn"];
        Rsc = (string)json["Rsc"];
        Type = (string)json["Type"];
    }
}
