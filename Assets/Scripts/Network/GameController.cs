using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
[RequireComponent(typeof(NetworkView))]
public class GameController : MonoBehaviour {
	
	private LanBroadcastService _broadcastService;
	[SerializeField]
	public ServerInfo TargetServer;
	public string PlayerName = "Underlord";
	public List<ServerInfo> ServerList
	{
		get 
		{ 
			return _broadcastService.ReceivedMessages.Select(rm => rm.Value.ServerInfo).ToList(); 
		}
	}

	public enum SpawnAt{Base1, Base2, BaseSpecific}
	private GameObject _player;
	
	// Use this for initialization
	void Start () 
	{
		_broadcastService = gameObject.AddComponent<LanBroadcastService>();
		_broadcastService.ServerListUpdated += OnServerListUpdated;
		Application.runInBackground = true;
		_broadcastService.StartSearching();
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
		if(GameObject.Find ("New Game Object") != null) Destroy (GameObject.Find ("New Game Object"));
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
	
	private void OnServerListUpdated()
	{
		if(ServerList.Count >= 1)
		{
			TargetServer = ServerList[0];
			ConnectToServer();
		}
	}

	
	//########### Below is copied ####################//
	public void StartServer()
	{
		Network.Disconnect();
		Network.InitializeServer(1, NetworkConfiguration.LAN_TCP_PORT, !Network.HavePublicAddress());
	}
	
	public void StartSearching()
	{
		_broadcastService.StartSearching();
	}
	
	public void StopBroadcastSession()
	{
		_broadcastService.StopSession();
	}
	
	public void ConnectToServer()
	{
		StopBroadcastSession();
		Network.Connect(TargetServer.IP, NetworkConfiguration.LAN_TCP_PORT);
	}
	
	void OnServerInitialized()
	{
		_broadcastService.StartAnnounceBroadCasting(PlayerName, 2);
		SpawnPlayer(SpawnAt.Base1);
	}
	
	private void OnPlayerConnected(NetworkPlayer player)
	{
		StopBroadcastSession();
	}
	
	public void CloseConnection()
	{
		Network.Disconnect();
		StopBroadcastSession();
	}
	
	void OnConnectedToServer()
	{
		SpawnPlayer(SpawnAt.Base2);
	}
	
	public static bool isNetwork
	{
		get { return Network.isServer || Network.isClient; }
	}

}
