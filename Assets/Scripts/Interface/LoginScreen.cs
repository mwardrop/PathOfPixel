using System;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

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
    }


    void LoginClicked()
    {
        if(String.IsNullOrWhiteSpace(ServerField.text) || 
            String.IsNullOrWhiteSpace(UsernameField.text) || 
            String.IsNullOrWhiteSpace(PasswordField.text))
        {
//EditorUtility.DisplayDialog("Missing Details", "Server, Username and Password are required to login.", "Ok");
            return;
        } 


        if(ServerField.text.ToLower() == "debug")
        {
            GameObject.FindWithTag("ServerManager").SetActive(false);
            ClientManager.Instance.Connect("127.0.0.1", 4296, UsernameField.text, PasswordField.text, OnConnectFail);

        } else {

            host = ServerField.text.Split(":")[0];
            port = int.Parse(ServerField.text.Split(":")[1]);

            if (!localhosts.Contains(host.ToLower()))
            {
                GameObject.FindWithTag("ServerManager").SetActive(false);
            }

            ClientManager.Instance.Connect(host, port, UsernameField.text, PasswordField.text, OnConnectFail);
        }

    }

    void HostClicked()
    {
        //EditorUtility.DisplayDialog("Not Implemented", "Dedicated Host is not yet implemented.", "Ok");
        //ServerManager.isDedicated = true;
        //SceneManager.LoadScene("OverworldScene", LoadSceneMode.Single);
    }

    void SettingsClicked()
    {
        //EditorUtility.DisplayDialog("Not Implemented", "Settings is not yet implemented.", "Ok");
    }


    private void OnConnectFail(Exception exception)
    {
        //EditorUtility.DisplayDialog("Could not connect to server.", exception.Message.ToString(), "Ok");
    }


}
