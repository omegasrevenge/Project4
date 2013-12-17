using System;
using UnityEngine;


[Serializable]
public class Player
{
    public string PlayerID;
    public string Name;
    public Vector2 Position;
    public int[,] Resources;
    //public int XP;
    //public int Level;
    public bool Fighting;

    public void ReadJson(JSONObject json)
    {
        PlayerID = (string)json["PId"];
        Name = (string)json["Name"];
        Position = new Vector2((float)json["Lon"], (float)json["Lat"]);
        Resources = new int[7,5];
        JSONObject res = json["Resources"];
        for (int i = 0; i < res.Count; i++)
        {
            JSONObject element = res[i];
            for (int j = 0; j < element.Count; j++)
            {
                Resources[i, j] = (int) element[j];
            }
        }
        //XP = (int)json["XP"];
        //Level = (int)json["Level"];
        Fighting = (bool)json["Fighting"];
       
    }
}
