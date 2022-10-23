using System;
using System.Net;
using DarkRift;
using DarkRift.Client.Unity;
using UnityEngine;

[RequireComponent(typeof(UnityClient))]
public class ClientConnectionManager : MonoBehaviour
{
    public static ClientConnectionManager Instance;

    [Header("Settings")]
    [SerializeField]
    private string host;
    [SerializeField]
    private int port;

    public UnityClient Client { get; private set; }

    public delegate void OnConnectedDelegate();
    public event OnConnectedDelegate OnConnected;


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

    void Start()
    {
        //Client.ConnectInBackground(host, port, true, ConnectCallback);
    }

    public void Connect(string _host, int _port) {
        host = _host;
        port = _port;
        Connect();
    }

    public void Connect()
    {
        //Client.ConnectInBackground(host, port, true, ConnectCallback);
        Client.ConnectInBackground(IPAddress.Loopback, Client.Port, false, ConnectCallback);
    }

    private void ConnectCallback(Exception exception)
    {
        if (Client.ConnectionState == ConnectionState.Connected)
        {
            OnConnected?.Invoke();
        }
        else
        {
            Debug.LogError("Unable to connect to server.");
        }
    }
}
