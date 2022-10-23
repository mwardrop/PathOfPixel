using DarkRift;
using DarkRift.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Message = DarkRift.Message;

public class LoginScreen : MonoBehaviour
{

    public Button LoginButton, HostButton, SettingsButton;
    public TMP_InputField ServerField, UsernameField, PasswordField;

    private string host;
    private int port;

    void Start()
    {
        LoginButton.onClick.AddListener(LoginClicked);
        HostButton.onClick.AddListener(HostClicked);
        SettingsButton.onClick.AddListener(SettingsClicked);

        ClientConnectionManager.Instance.Client.MessageReceived += OnNetworkMessage;
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

        ClientConnectionManager.Instance.OnConnected += Login;
        ClientConnectionManager.Instance.Connect();


    }

    void HostClicked()
    {
        EditorUtility.DisplayDialog("Not Implemented", "Dedicated Hosting is not yet implemented.", "Ok");
    }

    void SettingsClicked()
    {
        EditorUtility.DisplayDialog("Not Implemented", "Settings is not yet implemented.", "Ok");
    }

    public void Login()
    {
        using (Message message = Message.Create((ushort)NetworkTags.LoginRequest, new LoginRequestData(UsernameField.text, PasswordField.text)))
        {
            ClientConnectionManager.Instance.Client.SendMessage(message, SendMode.Reliable);
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
        EditorUtility.DisplayDialog("Login", "Login Success.", "Ok");
    }
}
