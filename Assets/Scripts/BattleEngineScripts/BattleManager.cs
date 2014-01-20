using UnityEngine;
using System.Collections;

public class BattleManager : MonoBehaviour 
{
	public TESTBATTLEENGINE DummyServer;
	public string InputText = "";
	public string InitFightResult;

	void Start () 
	{
		DummyServer = GameManager.CreateController<TESTBATTLEENGINE>("DummyServer");
		DummyServer.Init();
	}

	void Update () 
	{
	
	}

	void OnGUI()
	{
		if(BattleEngine.Current == null) return;
		if(InputText != "Laser")
		{
			if (GUI.Button(new Rect(0, Screen.height-100, 200, 100), "Driode_1"))
			{
				InputText += "1";
			}
			if (GUI.Button(new Rect(200, Screen.height-100, 200, 100), "Driode_2"))
			{
				InputText += "2";
			}
			if (GUI.Button(new Rect(400, Screen.height-100, 200, 100), "Driode_3"))
			{
				InputText += "3";
			}
			if (GUI.Button(new Rect(600, Screen.height-100, 200, 100), "Driode_4"))
			{
				InputText += "4";
			}
		}

		if (GUI.Button(new Rect(Screen.width-200, Screen.height/2-100, 200, 200), "Execute!"))
		{
			DummyServer.CreateNewSubstituteForResult();
			InputText = "";
		}

		if(InputText == "1234")
		{
			InputText = "Laser";
		}

		GUI.TextArea(new Rect(Screen.width-100, Screen.height/2+100, 100, 100), InputText);

		if (GUI.Button(new Rect(Screen.width-200, Screen.height/2+100, 100, 100), "Delete Selection"))
		{
			InputText = "";
		}
	}

	public void GetRoundResult()
	{
		if (!GameManager.Singleton.LoggedIn) return;
		StartCoroutine(ServerRequest());
	}
	
	private IEnumerator ServerRequest()
	{
		WWW request = new WWW(GameManager.Singleton.GetSessionURL("fightgetinfo"));
		yield return request;
		
		JSONObject json = JSONParser.parse(request.text);
		InitFightResult = (string) json["data"];
	}
}
