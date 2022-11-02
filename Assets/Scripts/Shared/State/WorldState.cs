using DarkRift;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldState: IDarkRiftSerializable
{
    public List<PlayerState> Players;
    public List<SceneState> Scenes;

    public WorldState()
    {
        Players = new List<PlayerState>();
        Scenes = new List<SceneState>();
    }

    public PlayerState GetPlayerState(int clientId)
    {
        try
        {
            return ClientManager.Instance.StateManager.WorldState
                                .Players.First(x => x.ClientId == clientId);
        }
        catch { return null; }
    }

    public EnemyState GetEnemyState(System.Guid enemyGuid, string scene)
    {
        try
        {
            return ClientManager.Instance.StateManager.WorldState
                        .Scenes.First(x => x.Name.ToLower() == scene.ToLower())
                        .Enemies.First(x => x.EnemyGuid == enemyGuid);
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

