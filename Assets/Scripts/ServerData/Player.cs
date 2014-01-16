using System;
using UnityEngine;


[Serializable]
public class Player
{
    public string PlayerID;
    public string Name;
	public Vector2 Position;
	public Vector2 BasePosition;
	public Int64 BaseTime;
    public int[,] Resources;

	public bool Fighting;

	public GameObject baseInstance;
    public int InitSteps;

	public Creature CurCreature;

    public void ReadJson(JSONObject json)
    {
        PlayerID = (string)json["PId"];
        Name = (string)json["Name"];
		Position = new Vector2((float)json["Lon"], (float)json["Lat"]);
		BasePosition = new Vector2((float)json["BaseLon"], (float)json["BaseLat"]);
	    BaseTime = (Int64) json["TimeBase"];
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

		CurCreature.ReadJson(json["CurrentCreature"]);
        Fighting = (bool)json["Fighting"];
        InitSteps = (int) json["InitSteps"];

    }
}
