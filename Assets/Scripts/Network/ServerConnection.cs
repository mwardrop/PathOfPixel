using DarkRift;
using DarkRift.Server;
using System;
using UnityEditor.EditorTools;
using UnityEngine.SceneManagement;
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

        ServerManager.Instance.Connections.Add(Username, this);

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
            }
        }
    }

    private void SpawnPlayer()
    {
        PlayerState.Location = new Vector2(Random.Range(-3, 3), Random.Range(-3, 3));

        BroadcastNetworkMessage(
            NetworkTags.SpawnPlayer, 
            new SpawnPlayerData(PlayerState.Scene, PlayerState.Name, PlayerState.Location)
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