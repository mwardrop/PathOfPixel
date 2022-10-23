using DarkRift;
using DarkRift.Server;
using UnityEditor.EditorTools;

public class ClientConnection
{
    public string Username { get; }
    public IClient Client { get; }

    public ClientConnection(IClient client, LoginRequestData data)
    {
        Client = client;
        Username = data.Username;

        ServerManager.Instance.Players.Add(client.ID, this);
        ServerManager.Instance.PlayersByUsername.Add(Username, this);

        using (Message m = Message.Create((ushort)NetworkTags.LoginRequestAccepted, new LoginInfoData(client.ID)))
        {
            client.SendMessage(m, SendMode.Reliable);
        }
    }

}