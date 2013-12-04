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
		
	}
	
	private void SpawnPlayer(bool value=false)
	{
		Transform target = value ? GameObject.Find("Base2").transform : GameObject.Find("Base1").transform;
		
		Object PlayerPrefab = Resources.Load("Player");
		GameObject Player = (GameObject)Network.Instantiate(PlayerPrefab, target.position, target.localRotation, 1);
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
		SpawnPlayer();
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
		SpawnPlayer(true);
	}
	
	public static bool isNetwork
	{
		get { return Network.isServer || Network.isClient; }
	}

}
