using DarkRift;
using DarkRift.Client;
using System;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Message = DarkRift.Message;

public class LoginScreen : MonoBehaviour
{

    public Button LoginButton, HostButton, SettingsButton;
    public TMP_InputField ServerField, UsernameField, PasswordField;

    private string host;
    private int port;
    private string[] localhosts = { "localhost", "127.0.0.1" };

    void Start()
    {
        LoginButton.onClick.AddListener(LoginClicked);
        HostButton.onClick.AddListener(HostClicked);
        SettingsButton.onClick.AddListener(SettingsClicked);

        ClientManager.Instance.Client.MessageReceived += OnNetworkMessage;
    }


    void LoginClicked()
    {
        if(String.IsNullOrWhiteSpace(ServerField.text) || 
            String.IsNullOrWhiteSpace(UsernameField.text) || 
            String.IsNullOrWhiteSpace(PasswordField.text))
        {
            EditorUtility.DisplayDialog("Missing Details", "Server, Username and Password are required to login.", "Ok");
            return;
        } 

        host = ServerField.text.Split(":")[0];
        port = int.Parse(ServerField.text.Split(":")[1]);

        if (!localhosts.Contains(host.ToLower())){
            GameObject.FindWithTag("ServerManager").SetActive(false);
        }

        if(host.ToLower() == "debug")
        {
            ClientManager.Instance.Connect("localhost", 666, Login);
        } else
        {
            ClientManager.Instance.Connect(host, port, Login);
        }

    }

    void HostClicked()
    {
        ServerManager.isDedicated = true;
        SceneManager.LoadScene("OverworldScene", LoadSceneMode.Single);
    }

    void SettingsClicked()
    {
        EditorUtility.DisplayDialog("Not Implemented", "Settings is not yet implemented.", "Ok");
    }

    public void Login(Exception exception)
    {
        if (ClientManager.Instance.Client.ConnectionState == ConnectionState.Connected)
        {
            Debug.Log("Connected to server.");

            ClientManager.SendMessage(
                NetworkTags.LoginRequest, 
                new LoginRequestData(UsernameField.text, PasswordField.text));
        }
        else
        {
            EditorUtility.DisplayDialog("Could not connect to server.", exception.Message.ToString(), "Ok");
        }

    }

    public void OnNetworkMessage(object sender, MessageReceivedEventArgs e)
    {
        using (Message message = e.GetMessage())
        {
            switch ((NetworkTags)message.Tag)
            {
                case NetworkTags.LoginRequestDenied:
                    OnLoginDecline();
                    break;
                case NetworkTags.LoginRequestAccepted:
                    OnLoginAccept(message.Deserialize<LoginInfoData>());
                    break;
            }
        }
    }

    private void OnLoginDecline()
    {
        EditorUtility.DisplayDialog("Login", "Failed Login.", "Ok");
    }

    private void OnLoginAccept(LoginInfoData data)
    {
        ClientManager.Instance.WorldState = data.State;
        EditorUtility.DisplayDialog("Login", "World State Set.", "Ok");
    }
}
