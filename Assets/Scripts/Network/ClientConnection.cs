using DarkRift;
using DarkRift.Server;
using UnityEditor.EditorTools;

public class ClientConnection
{
    public string Username { get; }
    public IClient Client { get; }

    public PlayerState PlayerState;

    public ClientConnection(IClient client, LoginRequestData data)
    {
        Client = client;
        Username = data.Username;

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

        using (Message m = Message.Create((ushort)NetworkTags.LoginRequestAccepted, new LoginInfoData(client.ID, ServerManager.Instance.WorldState)))
        {
            client.SendMessage(m, SendMode.Reliable);
        }
    }

}