using System;
using DarkRift;
using DarkRift.Client;
using DarkRift.Client.Unity;
using UnityEngine;

[RequireComponent(typeof(UnityClient))]
public class ClientManager : MonoBehaviour
{
    public static ClientManager Instance;

    private static string Host;
    private static int Port;
    private static string Username;
    private static string Password;

    public UnityClient Client { get; private set; }

    public ClientStateManager StateManager;
    public static ClientPrefabs Prefabs;

    public delegate void ConnectFailedDelegate(Exception exception);
    ConnectFailedDelegate ConnectFailed;

    public static bool IsHost {
        get {
            if(IsDedicated) { return true; }
            return ClientManager.Instance.Client.ID == 0;
        } 
    }
    public static bool IsDedicated = false;


    public void Update()
    {
        if(StateManager != null)
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

        Client = GetComponent<UnityClient>();
        Prefabs = GetComponent<ClientPrefabs>();

        Instance.Client.MessageReceived += OnNetworkMessage;
    }

    public void Connect(string host, int port, string username, string password, ConnectFailedDelegate connectFailed)
    {
        Host = host;
        Port = port;
        Username = username;
        Password = password;
        ConnectFailed = connectFailed;

        Connect();
    }

    private void Connect()
    {
        Instance.Client.ConnectInBackground(Host, Port, false, OnConnect);
    }

    private void OnConnect(Exception exception)
    {
        if (Instance.Client.ConnectionState == ConnectionState.Connected)
        {
            SendNetworkMessage(
                NetworkTags.LoginRequest,
                new LoginRequestData(Username, Password));
        }
        else
        {
            ConnectFailed(exception);
        }
    }

    private void OnNetworkMessage(object sender, MessageReceivedEventArgs e)
    {
        using (Message message = e.GetMessage())
        {
            switch ((NetworkTags)message.Tag)
            {
                case NetworkTags.LoginRequestDenied:
                    ConnectFailed(new Exception("Failed Login."));
                    break;
                case NetworkTags.LoginRequestAccepted:
                    StateManager = new ClientStateManager(message.Deserialize<LoginResponseData>().State);
                    Instance.Client.MessageReceived -= OnNetworkMessage;
                    break;
            }
        }
    }

    public static void SendNetworkMessage(NetworkTags tag, IDarkRiftSerializable msgObject)
    {
        using (Message message = Message.Create((ushort)tag, msgObject))
        {
            Instance.Client.SendMessage(message, SendMode.Reliable);
        }
    }

    public static void SendNetworkMessage(NetworkTags tag)
    {
        using (Message message = Message.CreateEmpty((ushort)tag))
        {
            Instance.Client.SendMessage(message, SendMode.Reliable);
        }
    }

    public void OnDestroy()
    {
        Instance.Client.MessageReceived -= OnNetworkMessage;
    }

}
 