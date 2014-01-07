using UnityEngine;

public class GameController : MonoBehaviour
{
    public const string GameType = "PrototypeBattle.Project4.MDH2013";
    public const int Port = 23466;

    public string GameName = "Uns_geht_ein_Licht_auf";

	public float PlayerSpawnHeight = 30f;

	private GameObject _player;
	private Vector3 _spawnPos;
	private Transform _baseOne;
	private Transform _baseTwo;

	public bool HasNetworkConnection
	{
		get
		{
			return Network.connections.Length > 0;
		}
	}

	// Use this for initialization
	void Start () 
	{
		_baseOne = GameObject.Find("Base1").transform;
		_baseTwo = GameObject.Find("Base2").transform;
		_spawnPos = new Vector3(_baseOne.localPosition.x, 
		                        PlayerSpawnHeight, 
		                        0f);
		Application.runInBackground = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(_player == null && HasNetworkConnection) SpawnPlayer();
		CleanHierarchy();
	}

	private void CleanHierarchy()
	{
		GameObject target = GameObject.Find("New Game Object");
		if(target != null) Destroy(target);
	}
	
	private void SpawnPlayer()
	{
		_spawnPos.z = Random.Range(_baseOne.localPosition.z, 
		                           _baseTwo.localPosition.z);
		_player = (GameObject)Network.Instantiate(Resources.Load("Player"), 
		                                          _spawnPos, 
		                                          Quaternion.identity, 
		                                          1);
	}

    public void CreateGame()
	{
		Network.Disconnect();
		ResetLevel();
        Network.InitializeServer(4, Port, !Network.HavePublicAddress());
        MasterServer.RegisterHost(GameType, GameName);
    }

    public void RequestHosts()
	{
		if(HasNetworkConnection) return;
		MasterServer.RequestHostList(GameType);
    }


    private void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if(msEvent == MasterServerEvent.HostListReceived)
		{
			HostData[] data = MasterServer.PollHostList();
			if(data.Length>0) Network.Connect(data[0]);
		}
    }

    private void OnConnectedToServer()
	{
		SpawnPlayer();
	}
	
	void OnServerInitialized()
	{
		SpawnPlayer();
	}

	void OnPlayerConnected(NetworkPlayer player)
	{
		MasterServer.UnregisterHost();
	}

	void OnPlayerDisconnected(NetworkPlayer player) 
	{
		Network.Disconnect();
		ResetLevel();
	}

	[RPC]
	public void CleanUpAfterPlayer(NetworkPlayer player)
	{
		Network.DestroyPlayerObjects(player);
	}

	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		ResetLevel();
	}

	private void ResetLevel()
	{
		foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player")) Destroy(player);
		foreach(GameObject buff in GameObject.FindGameObjectsWithTag("NoCdBuff")) Destroy(buff);
		foreach(GameObject fireball in GameObject.FindGameObjectsWithTag("Fireball")) Destroy(fireball);
		foreach(GameObject laser in GameObject.FindGameObjectsWithTag("Laser")) Destroy(laser);
		foreach(GameObject axe in GameObject.FindGameObjectsWithTag("Axe")) Destroy(axe);
	}

	public void DisconnectNow()
	{
		Network.Disconnect();
	}
}

