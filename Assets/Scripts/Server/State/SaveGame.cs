using DarkRift;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class SaveGame: IDarkRiftSerializable
{
    public List<PlayerSaveGame> Saves;

    public SaveGame()
    {
        Saves = new List<PlayerSaveGame>();
    }

    public void Deserialize(DeserializeEvent e)
    {
        PlayerSaveGame[] saveGames = e.Reader.ReadSerializables<PlayerSaveGame>();
        Saves = saveGames.ToList();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(Saves.ToArray());
    }

    [Serializable]
    public class PlayerSaveGame: IDarkRiftSerializable
    {
        public PlayerState State;
        public string Username;
        public string Password;

        public void Deserialize(DeserializeEvent e)
        {
            State = e.Reader.ReadSerializable<PlayerState>();
            Username = e.Reader.ReadString();
            Password = e.Reader.ReadString();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(State);
            e.Writer.Write(Username);
            e.Writer.Write(Password);
        }
    }
}
