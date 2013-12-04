using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class ServerInfo
{
    public string IP = "";
    public string Name = "";
    public int UniqueID = 0;
    public int ConnectedPlayers = 0;
    public int MaxConnectedPlayers = 2;


    public static implicit operator string(ServerInfo rhs)
    {
        return string.Format("IP:{0} Name:{1} (UID: {2})({3}/{4})", rhs.IP, rhs.Name, rhs.UniqueID, rhs.ConnectedPlayers, rhs.MaxConnectedPlayers);
    }
}

public class LanBroadcastService : MonoBehaviour
{
    private enum ServiceState { NotActive, Searching, Announcing };
    public struct ReceivedMessage
    {
        public float Time;
        public ServerInfo ServerInfo;
    }


    public Dictionary<int, ReceivedMessage> ReceivedMessages = new Dictionary<int, ReceivedMessage>();
    public event Action ServerListUpdated;
    private bool _updateFlag;

    private void OnServerListUpdated()
    {
        if (ServerListUpdated != null) ServerListUpdated();
        _updateFlag = false;
    }

    private string _IP;
    private ServiceState _currentState = ServiceState.NotActive;
    private UdpClient _udpClient;
    private ServerInfo _serverInfo;
    private string _strMessage = "";

    private const float IntervalMessageSending = 1f;
    private const float TimeMessagesLive = 6f;
    private float _time;
    private float _timeLastMessageSent;

    void Awake()
    {
        _IP = Dns.GetHostName();
    }

    void OnDestroy()
    {
        StopSession();
    }


    void Update()
    {
        if (_updateFlag)
            OnServerListUpdated();
        // Check if we need to send messages and the waiting interval has espired
        if ((_currentState == ServiceState.Searching || _currentState == ServiceState.Announcing)
                && Time.time > _timeLastMessageSent + IntervalMessageSending)
        {
            // Determine out of our current state what the content of the message will be
            if (_currentState == ServiceState.Announcing)
            {

                byte[] byteMessageToSend;
                string stringMessage = NetworkConfiguration.ServerLobbyReadyNetworkMessage(_serverInfo);
                byteMessageToSend = System.Text.Encoding.UTF8.GetBytes(stringMessage);
                _udpClient.Send(byteMessageToSend, byteMessageToSend.Length, new IPEndPoint(IPAddress.Broadcast, NetworkConfiguration.LAN_UDP_PORT));
                _udpClient.Send(byteMessageToSend, byteMessageToSend.Length, new IPEndPoint(IPAddress.Broadcast, NetworkConfiguration.LAN_UDP_PORT2));
            }

            // Refresh the list of received messages (remove old messages)
            else if (_currentState == ServiceState.Searching)
            {
                List<int> messagesToRemove = new List<int>();

                foreach (int key in ReceivedMessages.Keys)
                {
                    ReceivedMessage msg = ReceivedMessages[key];

                    if (Time.time > msg.Time + TimeMessagesLive)
                    {
                        // If this message is too old, delete it
                        messagesToRemove.Add(key);
                    }
                }

                foreach (int key in messagesToRemove)
                {
                    ReceivedMessages.Remove(key);
                    OnServerListUpdated();
                }
            }

            // Restart the timer
            _timeLastMessageSent = Time.time;
        }

        _time = Time.time;
    }

    public void StartAnnounceBroadCasting(string serverName, int maxPlayerConnections)
    {
        _serverInfo = new ServerInfo();
        _serverInfo.IP = _IP;
        _serverInfo.Name = serverName;
        _serverInfo.UniqueID = Random.Range(0, int.MaxValue);
        _serverInfo.MaxConnectedPlayers = maxPlayerConnections;

        StartSession();
        StartAnnouncing();
    }

    /// <summary>
    /// Prepares the UDPClient for the broadcasting session.
    /// </summary>
    private void StartSession()
    {
        if (_currentState != ServiceState.NotActive)
            StopSession();

        try
        {
            _udpClient = new UdpClient(NetworkConfiguration.LAN_UDP_PORT);
        }
        catch (SocketException)
        {
            _udpClient = new UdpClient(NetworkConfiguration.LAN_UDP_PORT2);

            Debug.Log("Nimm halt Nummer 2");
        }
        _udpClient.EnableBroadcast = true;
        _timeLastMessageSent = Time.time;
    }

    /// <summary>
    /// Stops the UDPClient.
    /// </summary>
    public void StopSession()
    {
        if (_currentState == ServiceState.Searching)
            StopSearching();
        else if (_currentState == ServiceState.Announcing)
            StopAnnouncing();
        if (_udpClient != null)
        {
            _udpClient.Close();
            _udpClient = null;
        }
    }

    private void BeginAsyncReceive()
    {
        _udpClient.BeginReceive(EndAsyncReceive, null);
    }

    private void EndAsyncReceive(IAsyncResult result)
    {
        if (_udpClient == null) return;

        // Create an empty EndPoint, that will be filled by the UDPClient, holding information about the sender
        IPEndPoint sendersIPEndPoint = new IPEndPoint(IPAddress.Any, 0);
        // Read the message
        byte[] byteMessage = _udpClient.EndReceive(result, ref sendersIPEndPoint);
        string senderIP = sendersIPEndPoint.Address.ToString();

        // If the received message has content and it was not sent by ourselves...  
        if (byteMessage.Length > 0 && !senderIP.Equals(_IP))
        {
            // Translate message to string
            string stringMessage = System.Text.Encoding.UTF8.GetString(byteMessage);
            ReceivedMessage receivedMessage = new ReceivedMessage();

            ServerInfo serverInfo;
            if (NetworkConfiguration.ValidateServerLobbyReadyNetworkMessage(stringMessage, out serverInfo))
            {
                receivedMessage.Time = _time;
                serverInfo.IP = senderIP;
                receivedMessage.ServerInfo = serverInfo;
                if (!ReceivedMessages.ContainsKey(serverInfo.UniqueID))
                    _updateFlag = true;
                //Save the received message with the unique ID as key
                ReceivedMessages[serverInfo.UniqueID] = receivedMessage;


            }
            else if (NetworkConfiguration.ValidateServerLobbyClosedNetworkMessage(stringMessage, out serverInfo))
            {
                if (ReceivedMessages.ContainsKey(serverInfo.UniqueID))
                {
                    ReceivedMessages.Remove(serverInfo.UniqueID);
                    _updateFlag = true;
                }
            }
        }

        Debug.Log(_currentState);
        // Check if we're still searching and if so, restart the receive procedure
        if (_currentState == ServiceState.Searching)
        {
            BeginAsyncReceive();
        }
    }

    public void StartSearching()
    {
        StartSession();
        ReceivedMessages.Clear();
        BeginAsyncReceive();
        _currentState = ServiceState.Searching;
        _strMessage = "Searching for servers.";
    }

    private void StopSearching()
    {
        _currentState = ServiceState.NotActive;
        _strMessage = "Ending the search for servers";
    }

    private void StartAnnouncing()
    {
        _currentState = ServiceState.Announcing;
        _strMessage = "Announcing we are a server...";
    }

    // Method to stop this object announcing this is a server, used by the script itself
    private void StopAnnouncing()
    {
        Byte[] byteMessageToSend = System.Text.Encoding.UTF8.GetBytes(NetworkConfiguration.ServerLobbyClosedNetworkMessage(_serverInfo));
        _udpClient.Send(byteMessageToSend, byteMessageToSend.Length, new IPEndPoint(IPAddress.Broadcast, NetworkConfiguration.LAN_UDP_PORT));
        //Debug.Log("Sent a request to remote clients to remove this server from their lists");

        _currentState = ServiceState.NotActive;
        _strMessage = "Announcements stopped.";
    }

    public static implicit operator string(LanBroadcastService rhs)
    {
        return rhs._strMessage;
    }

}
