using System;
using System.Collections;
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

    //public new void StartCoroutine(IEnumerator coroutine)
    //{
    //    base.StartCoroutine(coroutine);

    //    IEnumerator DeactivateSkillCoroutine()
    //    {
    //        yield return WaitForSeconds(3);

    //    }
    //}

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
            StateManager.LoadScene("OverworldScene");

            XmlServer = GetComponent<XmlUnityServer>();
            Server = XmlServer.Server;

            Server.ClientManager.ClientConnected += OnClientConnected;
            Server.ClientManager.ClientDisconnected += OnClientDisconnected;

            InvokeRepeating("OneSecondUpdate", 0, 1);

        }  catch(Exception ex) {
            Debug.LogWarning("Server failed to start.");
        }
    }

    public void Update()
    {
        if (StateManager != null)
        {
            StateManager.Update();
        }
    }

    public void OneSecondUpdate()
    {
        StateManager.OneSecondUpdate();
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
        //Connections.Remove(e.Client.ID);

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

        client.MessageReceived -= OnMessage;

        var existingConnectionId = 0;
        try
            {
            existingConnectionId = Connections.First(x =>
                x.Value.Username == data.Username &&
                x.Value.Client.ConnectionState == ConnectionState.Disconnected
            ).Key;
        } catch { }

        if(existingConnectionId != 0)
        {
            var existingConnection = Connections[existingConnectionId];

            if( existingConnection.Client.ConnectionState == ConnectionState.Connected ||
                existingConnection.Password != data.Password)
            {
                SendNetworkMessage(
                    client,
                    NetworkTags.LoginRequestDenied);

                return;
            }
            
            Connections.Remove(existingConnectionId);

            Connections.Add(client.ID, existingConnection.Restore(client));

        } else
        {
            ServerConnection newConnection = new ServerConnection(client, data.Username, data.Password, StateManager);

            Connections.Add(client.ID, newConnection);
        }

        SendNetworkMessage(
            client, 
            NetworkTags.LoginRequestAccepted, 
            new LoginResponseData(client.ID, ServerManager.Instance.StateManager.WorldState));
    
    }

    public static void SendNetworkMessage(int clientId, NetworkTags networkTag, IDarkRiftSerializable payload = null)
    {
        SendNetworkMessage(
            Instance.Connections.First(X => X.Key == clientId).Value.Client,
            networkTag,
            payload);

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

    public static void BroadcastNetworkMessage(NetworkTags networkTag, IDarkRiftSerializable payload = null)
    {
        foreach (KeyValuePair<int, ServerConnection> connection in Instance.Connections)
        {
            if (payload == null)
            {
                using (Message m = Message.CreateEmpty((ushort)networkTag))
                {
                    connection.Value.Client.SendMessage(m, SendMode.Reliable);
                }
            }
            else
            {
                using (Message m = Message.Create((ushort)networkTag, payload))
                {
                    connection.Value.Client.SendMessage(m, SendMode.Reliable);
                }
            }
        }

    }

}
