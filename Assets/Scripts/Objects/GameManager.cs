using DarkRift;
using DarkRift.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ClientManager.Instance.Client.MessageReceived += OnNetworkMessage;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnNetworkMessage(object sender, MessageReceivedEventArgs e)
    {
        using (Message message = e.GetMessage())
        {
            switch ((NetworkTags)message.Tag)
            {
                case NetworkTags.SpawnPlayer:
                    //OnLoginDecline();
                    break;
            }
        }
    }

    public void SpawnPlayer()
    {

    }
}
