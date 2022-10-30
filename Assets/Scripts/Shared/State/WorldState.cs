using DarkRift;
using System.Collections.Generic;
using System.Linq;

public class WorldState: IDarkRiftSerializable
{
    public List<PlayerState> Players;
    public List<SceneState> Scenes;

    public WorldState()
    {
        Players = new List<PlayerState>();
        Scenes = new List<SceneState>();
    }

    public void Deserialize(DeserializeEvent e)
    {
        PlayerState[] tempPlayers = e.Reader.ReadSerializables<PlayerState>();
        Players = tempPlayers.ToList();
        SceneState[] tempScenes = e.Reader.ReadSerializables<SceneState>();
        Scenes = tempScenes.ToList();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(Players.ToArray());
        e.Writer.Write(Scenes.ToArray());
    }
}

