using DarkRift;
using DarkRift.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientConnection 
{

    public WorldState WorldState = new WorldState();
    public PlayerState PlayerState
    {
        get
        {
            return WorldState.Players
                .First(x => x.ClientId == ClientManager.Instance.Client.ID);
        }
    }

    public ClientConnection(WorldState worldState)
    {
        ClientManager.Instance.Client.MessageReceived += OnNetworkMessage;

        WorldState = worldState;
        SceneManager.LoadScene(PlayerState.Scene, LoadSceneMode.Single);
        RequestSpawn();
    }

    private void RequestSpawn()
    {
        ClientManager.SendNetworkMessage(NetworkTags.SpawnRequest);
    }

    public void OnNetworkMessage(object sender, MessageReceivedEventArgs e)
    {
        using (Message message = e.GetMessage())
        {
            switch ((NetworkTags)message.Tag)
            {
                case NetworkTags.SpawnPlayer:
                    
                    UnityEngine.Object.Instantiate(
                        ClientManager.ClientPrefabs.WarriorSprite,
                        message.Deserialize<SpawnPlayerData>().Target,
                        Quaternion.identity).tag = "LocalPlayer";
                    break;
            }
        }
    }
}
