using System.Collections.Generic;
using System.Linq;
using DarkRift;
using DarkRift.Server;
using DarkRift.Server.Unity;
using UnityEditor.PackageManager;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    public static ServerManager Instance;

    private XmlUnityServer XmlServer;
    private DarkRiftServer Server;

    public Dictionary<int, ServerConnection> Connections = new Dictionary<int, ServerConnection>();
    public WorldState WorldState = new WorldState();

    public static bool isDedicated = false;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        XmlServer = GetComponent<XmlUnityServer>();
        Server = XmlServer.Server;
        Server.ClientManager.ClientConnected += OnClientConnected;
        Server.ClientManager.ClientDisconnected += OnClientDisconnected;
    }

    void OnDestroy()
    {
        Server.ClientManager.ClientConnected -= OnClientConnected;
        Server.ClientManager.ClientDisconnected -= OnClientDisconnected;
    }

    private void OnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
    {
        e.Client.MessageReceived -= OnMessage;

        ServerManager.Instance.WorldState.Players.RemoveAll(x => x.ClientId == e.Client.ID);
        ServerManager.Instance.Connections.Remove(e.Client.ID);

        BroadcastNetworkMessage(
            NetworkTags.PlayerDisconnect,
            new IntegerData(e.Client.ID));
    }

    private void OnClientConnected(object sender, ClientConnectedEventArgs e)
    {
        e.Client.MessageReceived += OnMessage;
    }

    private void OnMessage(object sender, MessageReceivedEventArgs e)
    {
        IClient client = (IClient)sender;
        using (Message message = e.GetMessage())
        {
            switch ((NetworkTags)message.Tag)
            {
                case NetworkTags.LoginRequest:
                    OnClientLogin(client, message.Deserialize<LoginRequestData>());
                    break;
            }
        }
    }

    private void OnClientLogin(IClient client, LoginRequestData data)
    {
        if (Connections.Where(x => x.Value.Username == data.Username).Count() > 0)
        {
            using (Message message = Message.CreateEmpty((ushort)NetworkTags.LoginRequestDenied))
            {
                client.SendMessage(message, SendMode.Reliable);
            }
            return;
        }

        // In the future the ClientConnection will handle its messages
        client.MessageReceived -= OnMessage;

        ServerConnection newConnection = new ServerConnection(client, data);

        Connections.Add(client.ID, newConnection);

        WorldState.Players.Add(newConnection.PlayerState);

        SendNetworkMessage(
            client, 
            NetworkTags.LoginRequestAccepted, 
            new LoginResponseData(client.ID, ServerManager.Instance.WorldState));
    
    }

    public static void SendNetworkMessage(IClient client, NetworkTags networkTag, IDarkRiftSerializable payload)
    {
        using (Message m = Message.Create((ushort)networkTag, payload))
        {
            client.SendMessage(m, SendMode.Reliable);
        }
    }

    public static void BroadcastNetworkMessage(NetworkTags networkTag, IDarkRiftSerializable payload)
    {
        foreach(KeyValuePair<int,ServerConnection> connection in Instance.Connections)
        {
            using (Message m = Message.Create((ushort)networkTag, payload))
            {
                connection.Value.Client.SendMessage(m, SendMode.Reliable);
            }
        }
 
    }

}
