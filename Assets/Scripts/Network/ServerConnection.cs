﻿using DarkRift;
using DarkRift.Server;
using UnityEngine;
using Random = UnityEngine.Random;

public class ServerConnection
{
    public string Username { get; }
    public IClient Client { get; }

    public PlayerState PlayerState;

    public ServerConnection(IClient client, LoginRequestData data)
    {

        Client = client;
        Username = data.Username;

        Client.MessageReceived += OnMessage;

        ServerManager.Instance.Connections.Add(Client.ID, this);

        // TODO : User should be able to create new and load existing PlayerStates (Warrior, Mage / Load, New)
        PlayerState = new PlayerState()
        {
            Name = data.Username,
            Health = 100,
            HealthRegen = 1,
            Mana = 100,
            ManaRegen = 1,
            PhysicalDamage = 5,
            FireDamage = 0,
            ColdDamage = 0,
            FireResistance = 0,
            ColdResistance = 0,
            Armor = 0,
            Dodge = 0,
            Level = 1,
            Experience = 0,
            Type = PlayerType.Warrior,
            Scene = "OverworldScene",
            ClientId = client.ID
        };

        ServerManager.Instance.WorldState.Players.Add(PlayerState);

        SendNetworkMessage(NetworkTags.LoginRequestAccepted, new LoginResponseData(client.ID, ServerManager.Instance.WorldState));
    }

    private void OnMessage(object sender, MessageReceivedEventArgs e)
    {
        IClient client = (IClient)sender;
        using (Message message = e.GetMessage())
        {
            switch ((NetworkTags)message.Tag)
            {
                case NetworkTags.SpawnRequest:
                    SpawnPlayer();
                    break;
                case NetworkTags.MoveRequest:
                    MovePlayer(message.Deserialize<TargetData>().Target);
                    break;
            }
        }
    }

    private void SpawnPlayer()
    {
        PlayerState.TargetLocation = PlayerState.Location = new Vector2(Random.Range(-3, 3), Random.Range(-3, 3));

        BroadcastNetworkMessage(
            NetworkTags.SpawnPlayer, 
            new PlayerStateData(PlayerState)
        );
    }

    private void MovePlayer(Vector2 target)
    {
        PlayerState.TargetLocation = target;

        BroadcastNetworkMessage(
            NetworkTags.MovePlayer,
            new MovePlayerData(Client.ID, PlayerState.TargetLocation)
        );
    }

    private void SendNetworkMessage(NetworkTags networkTag, IDarkRiftSerializable payload)
    {
        ServerManager.SendNetworkMessage(Client, networkTag, payload);
    }

    private void BroadcastNetworkMessage(NetworkTags networkTag, IDarkRiftSerializable payload)
    {
        ServerManager.BroadcastNetworkMessage(networkTag, payload);
    }

}