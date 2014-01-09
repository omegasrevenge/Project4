using UnityEngine;

public class Login : MonoBehaviour
{
    public string PlayerID = "PlayerID";
	public string PlayerName= "PlayerName";
    public string Password = "Password";
	public string XP	= "10"; 
	public string Enemy = "Enemy";
	public bool Register = false;

    void Start()
    {
#if !UNITY_EDITOR
        Social.Active = new UnityEngine.SocialPlatforms.GPGSocial();
        Social.localUser.Authenticate(OnAuthCB);
#endif
    }

    void OnGUI()
    {
#if UNITY_EDITOR
		if (!GameManager.Singleton.LoggedIn && !Register)
		{
	        PlayerID = GUI.TextField(new Rect(10, 10, 200, 20), PlayerID, 100);
	        Password = GUI.TextField(new Rect(10, 40, 200, 20), Password, 100);
	        if (GUI.Button(new Rect(10, 70, 100, 20), "Login"))
	        {
	            GameManager.Singleton.Login(PlayerID, Password, false);
	        }

            if (GUI.Button(new Rect(210, 70, 100, 20), " >>> Local <<<"))
            {
                GameManager.Singleton.Login(PlayerID, Password, true);
            }

            //if (GUI.Button(new Rect(120, 70, 100, 20), "Register"))
            //{
            //    Register = true;

            //    Password ="Enter Password";
            //    PlayerName ="Enter Playname";

            //}
        }
#endif
        //if ( GameManager.Singleton.LoggedIn == false && Register == true)
        //{
        //    PlayerID = GUI.TextField(new Rect(10, 10, 200, 20), PlayerID, 100);
        //    PlayerName = GUI.TextField(new Rect(10, 40, 200, 20), PlayerName, 100);
        //    Password = GUI.TextField(new Rect(10, 70, 200, 20), Password, 100);

        //    if (GUI.Button(new Rect(10, 100, 100, 20), "Register"))
        //    {
        //        GameManager.Singleton.CreatePlayer(PlayerID,PlayerName,Password);
        //    }

        //}

		if (GameManager.Singleton.LoggedIn)
		{
			Enemy = GUI.TextField(new Rect(10, 100, 200, 20), Enemy, 100);

	        if (GUI.Button(new Rect(10, 130, 100, 20), "Attack"))
	        {
	            GameManager.Singleton.Attack(Enemy);
	        }

			XP = GUI.TextField(new Rect(10, 40, 200, 20), XP, 100);

			if (GUI.Button(new Rect(10, 70, 100, 20), "AddXP"))
			{
				GameManager.Singleton.AddXP(XP);
			}

		}

    }

    void OnAuthCB(bool result)
    {
        if (!result)
        {
            //@To-do: Do something!
        }
        string token = NerdGPG.Instance().GetToken();
        if (string.IsNullOrEmpty(token))
        {
            //@To-do: Do something!
        }
        Debug.Log("GPGUI: Got Login Response: " + result);
        Debug.Log("!!!!!!!!!!" + NerdGPG.Instance().GetToken());
        GameManager.Singleton.Login(token);
    }
}
