using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace _3
{

public class Server : MonoBehaviour
{
    private const int MAX_CONNECTION = 10;

    private int port = 5805;

    private int hostID;
    private int reliableChannel;

    private bool isStarted = false;
    private byte error;

    List<int> connectionIDs = new List<int>();
    private Dictionary<int, string> idsAndNames = new Dictionary<int, string>();
    public void StartServer()
    {        
        NetworkTransport.Init();

        ConnectionConfig cc = new ConnectionConfig();
        reliableChannel = cc.AddChannel(QosType.Reliable);

        HostTopology topology = new HostTopology(cc, MAX_CONNECTION);        
        hostID = NetworkTransport.AddHost(topology, port);

        isStarted = true;
    }

    void Update()
    {
        if (!isStarted)
            return;

        int recHostId;
        int connectionId;
        int channelId;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;
        NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);

        while (recData != NetworkEventType.Nothing)
        {
            switch (recData)
            {
                case NetworkEventType.Nothing:
                    break;

                case NetworkEventType.ConnectEvent:
                    connectionIDs.Add(connectionId);
                    SendMessage($"Set your name,using chat",connectionId);
                    SendMessageToAll($"Player {connectionId} has connected.");
                    Debug.Log($"Player {connectionId} has connected.");
                    break;

                case NetworkEventType.DataEvent:
                    string message = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    if (!FindNameSet(connectionId))
                    {
                        idsAndNames.Add(connectionId,message);
                        SendMessage($"Your name is {message}", connectionId);
                        Debug.Log($"Your name is {message}");
                    }
                    else
                    {
                        SendMessageToAll($"{idsAndNames[connectionId]}: {message}");
                        Debug.Log($"{idsAndNames[connectionId]}: {message}");
                    }
                    
                    break;

                case NetworkEventType.DisconnectEvent:
                    idsAndNames.Remove(connectionId);
                    connectionIDs.Remove(connectionId);
                    SendMessageToAll($"{idsAndNames[connectionId]} has disconnected.");
                    Debug.Log($"{idsAndNames[connectionId]} has disconnected.");
                    break;

                case NetworkEventType.BroadcastEvent:
                    break;

            }

            recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
        }
    }

    public void ShutDownServer()
    {
        if (!isStarted)
            return;

        NetworkTransport.RemoveHost(hostID);
        NetworkTransport.Shutdown();
        isStarted = false;
    }

    private bool FindNameSet(int connectionID)
    {
        return idsAndNames.ContainsKey(connectionID);
    }
    
    public void SendMessage(string message, int connectionID)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(message);
        NetworkTransport.Send(hostID, connectionID, reliableChannel, buffer, message.Length * sizeof(char), out error);
        if ((NetworkError)error != NetworkError.Ok)
            Debug.Log((NetworkError)error);
    }

    public void SendMessageToAll(string message)
    {
        for (int i = 0; i < connectionIDs.Count; i++)        
            SendMessage(message, connectionIDs[i]);        
    }
}

}