using System.Collections.Generic;
using System.Linq;
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
    public StateManager StateManager = new StateManager();

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
        loadWorldState();

        XmlServer = GetComponent<XmlUnityServer>();
        Server = XmlServer.Server;
        Server.ClientManager.ClientConnected += OnClientConnected;
        Server.ClientManager.ClientDisconnected += OnClientDisconnected;
    }

    private void loadWorldState()
    {
        SceneState OverworldScene = new SceneState() { Name = "OverworldScene" };

        OverworldScene.Enemies.Add(new EnemyState()
        {
            Name = "Possessed 1",
            Health = 30,
            HealthRegen = 1,
            Mana = 100,
            ManaRegen = 1,
            PhysicalDamage = 5,
            FireDamage = 0,
            ColdDamage = 0,
            FireResistance = 0,
            ColdResistance = 0,
            Armor = 0,
            Dodge = 0,
            Level = 1,
            Experience = 0,
            Type = EnemyType.Possessed,
            Location = new Vector2(-2f, -4.5f)
        });

        OverworldScene.Enemies.Add(new EnemyState()
        {
            Name = "Possessed 2",
            Health = 30,
            HealthRegen = 1,
            Mana = 100,
            ManaRegen = 1,
            PhysicalDamage = 5,
            FireDamage = 0,
            ColdDamage = 0,
            FireResistance = 0,
            ColdResistance = 0,
            Armor = 0,
            Dodge = 0,
            Level = 1,
            Experience = 0,
            Type = EnemyType.Possessed,
            Location = new Vector2(-6f, -4.5f)
        });

        StateManager.WorldState.Scenes.Add(OverworldScene);
    }

    void OnDestroy()
    {
        Server.ClientManager.ClientConnected -= OnClientConnected;
        Server.ClientManager.ClientDisconnected -= OnClientDisconnected;
    }

    private void OnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
    {
        e.Client.MessageReceived -= OnMessage;

        ServerManager.Instance.StateManager.WorldState.Players.RemoveAll(x => x.ClientId == e.Client.ID);
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

        StateManager.WorldState.Players.Add(newConnection.PlayerState);

        SendNetworkMessage(
            client, 
            NetworkTags.LoginRequestAccepted, 
            new LoginResponseData(client.ID, ServerManager.Instance.StateManager.WorldState));
    
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
