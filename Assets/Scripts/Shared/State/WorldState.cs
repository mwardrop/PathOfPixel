using DarkRift;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class WorldState: IDarkRiftSerializable
{
    public List<PlayerState> Players { get; set; }
    public List<SceneState> Scenes { get; set; }

    public WorldState()
    {
        Players = new List<PlayerState>();
        Scenes = new List<SceneState>();
    }

    public PlayerState GetPlayerState(int clientId)
    {
        try
        {
            return Players.First(x => x.ClientId == clientId);
        }
        catch { return null; }
    }

    public EnemyState GetEnemyStateByGuid(System.Guid enemyGuid, string scene)
    {
        try
        {
            return Scenes.First(x => x.Name.ToLower() == scene.ToLower())
                        .Enemies.First(x => x.EnemyGuid == enemyGuid);
        } 
        catch { return null; }
    }

    public EnemyState GetEnemyStateByHashCode(int enemyHashCode, string scene)
    {
        try
        {
            return Scenes.First(x => x.Name.ToLower() == scene.ToLower())
                        .Enemies.First(x => x.EnemyGuid.GetHashCode() == enemyHashCode);
        }
        catch { return null; }
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

