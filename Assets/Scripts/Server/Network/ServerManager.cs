using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DarkRift;
using DarkRift.Server;
using DarkRift.Server.Unity;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    public static ServerManager Instance;

    private XmlUnityServer XmlServer;
    private DarkRiftServer Server;

    public Dictionary<int, ServerConnection> Connections = new Dictionary<int, ServerConnection>();
    public ServerStateManager StateManager;

    public void Update()
    {
        if (StateManager != null)
        {
            StateManager.Update();
        }
    }

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
        try
        {
            StateManager = new ServerStateManager();

            XmlServer = GetComponent<XmlUnityServer>();
            Server = XmlServer.Server;

            Server.ClientManager.ClientConnected += OnClientConnected;
            Server.ClientManager.ClientDisconnected += OnClientDisconnected;
        }  catch { }
    }

    void OnDestroy()
    {
        if (Server != null)
        {
            Server.ClientManager.ClientConnected -= OnClientConnected;
            Server.ClientManager.ClientDisconnected -= OnClientDisconnected;
        }
    }

    private void OnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
    {
        e.Client.MessageReceived -= OnMessage;

        StateManager.WorldState.Players.RemoveAll(x => x.ClientId == e.Client.ID);
        Connections.Remove(e.Client.ID);

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
            SendNetworkMessage(
                client,
                NetworkTags.LoginRequestDenied);

            return;
  
        }

        client.MessageReceived -= OnMessage;

        ServerConnection newConnection = new ServerConnection(client, data);

        Connections.Add(client.ID, newConnection);

        StateManager.WorldState.Players.Add(newConnection.PlayerState);

        SendNetworkMessage(
            client, 
            NetworkTags.LoginRequestAccepted, 
            new LoginResponseData(client.ID, ServerManager.Instance.StateManager.WorldState));
    
    }

    public static void SendNetworkMessage(IClient client, NetworkTags networkTag, IDarkRiftSerializable payload = null)
    {

        if (payload == null) {
            using (Message m = Message.CreateEmpty((ushort)networkTag))
            {
                client.SendMessage(m, SendMode.Reliable);
            }
        } else {
            using (Message m = Message.Create((ushort)networkTag, payload))
            {
                client.SendMessage(m, SendMode.Reliable);
            }
        }
    }

    public static void BroadcastNetworkMessage(NetworkTags networkTag, IDarkRiftSerializable payload = null )
    {
        foreach(KeyValuePair<int,ServerConnection> connection in Instance.Connections)
        {
            if(payload == null) {
                using (Message m = Message.CreateEmpty((ushort)networkTag))
                {
                    connection.Value.Client.SendMessage(m, SendMode.Reliable);
                }
            } else {
                using (Message m = Message.Create((ushort)networkTag, payload))
                {
                    connection.Value.Client.SendMessage(m, SendMode.Reliable);
                }
            }
        }

    }

}
