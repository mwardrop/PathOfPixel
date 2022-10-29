using DarkRift;
using DarkRift.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.SceneManagement;

public class ClientConnection 
{

    public WorldState WorldState = new WorldState();
    public PlayerState PlayerState
    {
        get
        {
            return WorldState.Players
                .First(x => x.ClientId == ClientId);
        }
    }
    public int ClientId
    {
        get
        {
            return ClientManager.Instance.Client.ID;
        }
    }

    public ClientConnection(WorldState worldState)
    {
        ClientManager.Instance.Client.MessageReceived += OnNetworkMessage;

        WorldState = worldState;

        SceneManager.LoadScene(PlayerState.Scene, LoadSceneMode.Single);
        
        foreach(PlayerState player in WorldState.Players)
        {
            if(player.Scene == PlayerState.Scene && player.ClientId != PlayerState.ClientId)
            {
                SpawnPlayer(new PlayerStateData(player));
            }
        }

        RequestSpawn();
    }

    public void OnNetworkMessage(object sender, MessageReceivedEventArgs e)
    {
        using (Message message = e.GetMessage())
        {
            switch ((NetworkTags)message.Tag)
            {
                case NetworkTags.SpawnPlayer:
                    SpawnPlayer(message.Deserialize<PlayerStateData>());
                    break;
                case NetworkTags.MovePlayer:
                    MovePlayer(message.Deserialize<MovePlayerData>());
                    break;
            }
        }
    }

    public void RequestSpawn()
    {
        ClientManager.SendNetworkMessage(NetworkTags.SpawnRequest);
    }

    private void SpawnPlayer(PlayerStateData playerStateData)
    {

        PlayerState playerState = playerStateData.PlayerState;

        if (WorldState.Players.Where(x=> x.ClientId == playerState.ClientId).Count() == 0)
        {
            WorldState.Players.Add(playerState);
        }

        GameObject newPlayer = UnityEngine.Object.Instantiate(
            ClientManager.ClientPrefabs.WarriorSprite,
            playerStateData.PlayerState.Location,
            Quaternion.identity);

        if(playerState.ClientId == ClientId)
        {
            newPlayer.tag = "LocalPlayer";
        } else
        {
            newPlayer.tag = "NetworkPlayer";
        }

        newPlayer.GetComponent<PlayerSprite>().NetworkClientId = playerState.ClientId;

    }

    public void RequestMove(Vector2 target)
    {
        ClientManager.SendNetworkMessage(NetworkTags.MoveRequest, new TargetData(target));
    }

    private void MovePlayer(MovePlayerData movePlayerData)
    {
        WorldState.Players.First(x => x.ClientId == movePlayerData.ClientId).TargetLocation = movePlayerData.Target;
    }

}
