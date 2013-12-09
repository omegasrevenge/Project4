using UnityEngine;

public class GameController : MonoBehaviour
{
    public const string GameType = "PrototypeBattle.Project4.MDH2013";
    public const int Port = 23466;

    public string GameName = "Uns_geht_ein_Licht_auf";

	public enum SpawnAt{Base1, Base2, BaseSpecific}
	private GameObject _player;
	private bool _tryingToConnect = false;
	
	// Use this for initialization
	void Start () 
	{
		Application.runInBackground = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(   _player != null
		   && _player.networkView.isMine
		   && _player.GetComponent<PlayerController>().IsDead 
		   && _player.GetComponent<PlayerController>().DyingAnimationFinished)
		{
			SpawnPlayer(SpawnAt.BaseSpecific);
		}
		if(GameObject.Find("New Game Object") != null)
		{
			Destroy(GameObject.Find("New Game Object"));
		}
	}
	
	private void SpawnPlayer(SpawnAt value)
	{
		Transform target = new GameObject().transform;
		switch(value)
		{
		case SpawnAt.Base1:
			target = GameObject.Find("Base1").transform;
			break;
		case SpawnAt.Base2:
			target = GameObject.Find("Base2").transform;
			break;
		case SpawnAt.BaseSpecific:
			target = _player.GetComponent<PlayerController>().SpawnBase;
			break;
		}
		GameObject player = new GameObject();
		if(value == SpawnAt.BaseSpecific)
		{
			networkView.RPC("DestroyEnemyPlayer", RPCMode.OthersBuffered);
			player = _player;
		}
		
		_player = (GameObject)Network.Instantiate(Resources.Load("Player"), target.position, target.localRotation, 1);
		_player.GetComponent<PlayerController>().SpawnBase = target;
		
		if(value == SpawnAt.BaseSpecific) Destroy (player.gameObject);
	}
	
	[RPC]
	public void DestroyEnemyPlayer()
	{
		foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player"))
		{
			if(!player.networkView.isMine)
			{
				Destroy(player.gameObject);
			}
		}
	}

    public void CreateGame()
	{
		_tryingToConnect = false;
		Network.Disconnect();
        Network.InitializeServer(4, Port, !Network.HavePublicAddress());
        MasterServer.RegisterHost(GameType, GameName);
    }

    public void RequestHosts()
	{
		_tryingToConnect = true;
		MasterServer.RequestHostList(GameType);
    }


    private void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if(msEvent == MasterServerEvent.HostListReceived && _tryingToConnect)
		{
			HostData[] data = MasterServer.PollHostList();
			if(data.Length>0) Network.Connect(data[0]);
		}
    }

    private void OnConnectedToServer()
	{
		SpawnPlayer(SpawnAt.Base2);
	}
	
	void OnServerInitialized()
	{
		SpawnPlayer(SpawnAt.Base1);
	}
}

