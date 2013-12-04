using System;
using UnityEngine;


[Serializable]
public class Player
{
    public string PlayerID;
    public string Name;
    public Vector2 Position;
    public int HP;
    public int HPMax;
    public int XP;
    public int Level;
    public bool Fighting;

    public void ReadJson(JSONObject json)
    {
        PlayerID = (string)json["PId"];
        Name = (string)json["Name"];
        Position = new Vector2((float)json["Lon"], (float)json["Lat"]);
        HP = (int)json["HP"];
        HPMax = (int)json["HPMax"];
        XP = (int)json["XP"];
        Level = (int)json["Level"];
        Fighting = (bool)json["Fighting"];
    }
}
