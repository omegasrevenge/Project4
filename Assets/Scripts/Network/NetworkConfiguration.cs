using UnityEngine;

public class NetworkConfiguration
{
    public const int LAN_TCP_PORT = 25122;
    public const int LAN_UDP_PORT = 25123;
    public const int LAN_UDP_PORT2 = 25124;
    public const string MESSAGE_TYPE_SERVERINFO = "SERVINFO";	//server name, connected player amount, max player amount
    public const string MESSAGE_TYPE_SERVERCLOSE = "SERVCLOS";	//server name

    private const char SEPERATOR = ((char)0x1D);

    /*
     * Should be called when a server should appear in the lobby
     * Returns a concatenated string which elements are seperated by the SEPERATOR including the following:
     * 1.) (string) MESSAGE_TYPE_SERVERINFO
     * 2.) (int) uniqueServerID
     * 3.) (string) nameOfServer
     * 4.) (string) connectedPlayerAmount
     * 5.) (int) maxPlayerAmount
     */
    public static string ServerLobbyReadyNetworkMessage(ServerInfo serverInfo)
    {
        return MESSAGE_TYPE_SERVERINFO + SEPERATOR
                + ServerInfoToSeperatedString(serverInfo);
    }

    /*
     * Should be called to generate a message which can be send by an UdpClient to notify searching clients
     * that this server should be removed from the lobby list.
     * Returns a string including the following:
     * 1.) (string) MESSAGE_TYPE_SERVERCLOSE
     * 2.) (int) uniqueServerID
     */
    public static string ServerLobbyClosedNetworkMessage(ServerInfo serverInfo)
    {
        return MESSAGE_TYPE_SERVERCLOSE + SEPERATOR
                + ServerInfoToSeperatedString(serverInfo);
    }

    private static string ServerInfoToSeperatedString(ServerInfo serverInfo)
    {
        string msg = serverInfo.UniqueID.ToString() + SEPERATOR
                + serverInfo.Name + SEPERATOR
                + serverInfo.ConnectedPlayers.ToString() + SEPERATOR
                + serverInfo.MaxConnectedPlayers.ToString();

        return msg;
    }

    /*
     * Returns true if the given serverMessage could be casted properly. False otherwise
     */
    public static bool ValidateServerLobbyReadyNetworkMessage(string serverMessage, out ServerInfo serverInfo)
    {
        string[] msg = SeperateMessage(serverMessage);
        serverInfo = new ServerInfo();

        if (!ValidateServerInfo(msg, ref serverInfo) || !msg[0].Equals(MESSAGE_TYPE_SERVERINFO))
            return false;
        else
            return true;
    }

    public static bool ValidateServerLobbyClosedNetworkMessage(string serverMessage, out ServerInfo serverInfo)
    {
        string[] msg = SeperateMessage(serverMessage);
        serverInfo = new ServerInfo();

        if (!ValidateServerInfo(msg, ref serverInfo) || !msg[0].Equals(MESSAGE_TYPE_SERVERCLOSE))
            return false;
        else
            return true;
    }

    private static bool ValidateServerInfo(string[] serverMessage, ref ServerInfo serverInfo)
    {
        if (serverMessage == null || serverMessage.Length < 4)
            return false;

        try
        {
            serverInfo.UniqueID = int.Parse(serverMessage[1]);
            serverInfo.Name = serverMessage[2];
            serverInfo.ConnectedPlayers = int.Parse(serverMessage[3]);
            serverInfo.MaxConnectedPlayers = int.Parse(serverMessage[4]);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Error validating server info with error description: " + e.Message);
            return false;
        }

        return true;
    }

    public static string[] SeperateMessage(string message)
    {
        return message.Split(SEPERATOR);
    }
}
