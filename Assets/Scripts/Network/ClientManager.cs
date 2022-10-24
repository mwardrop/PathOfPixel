using System;
using System.Net;
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

    public UnityClient Client { get; private set; }
    public WorldState WorldState = new WorldState();

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
    }

    public void Connect(string host, int port, DarkRiftClient.ConnectCompleteHandler callback)
    {
        Host = host;
        Port = port;
        Connect(callback);
    }

    private void Connect(DarkRiftClient.ConnectCompleteHandler callback)
    {
        Client.ConnectInBackground(Host, Port, false, callback);
    }

    public static void SendMessage(NetworkTags tag, IDarkRiftSerializable msgObject)
    {
        using (Message message = Message.Create((ushort)tag, msgObject))
        {
            Instance.Client.SendMessage(message, SendMode.Reliable);
        }
    }

}
