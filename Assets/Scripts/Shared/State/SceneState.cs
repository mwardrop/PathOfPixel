using DarkRift;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class SceneState : IDarkRiftSerializable
{
    public string Name;
    public List<EnemyState> Enemies;
    public List<ItemState> ItemDrops;
    public List<SpawnState> PlayerSpawns;
    public List<EnemySpawnState> EnemySpawns;
    public List<SpawnState> ChestSpawns;

    public SceneState()
    {
        Enemies = new List<EnemyState>();
        ItemDrops = new List<ItemState>();
        PlayerSpawns = new List<SpawnState>();
        EnemySpawns = new List<EnemySpawnState>();
        ChestSpawns = new List<SpawnState>();
    }

    public void Deserialize(DeserializeEvent e)
    {
        Name = e.Reader.ReadString();
        EnemyState[] tempEnemies = e.Reader.ReadSerializables<EnemyState>();
        Enemies = tempEnemies.ToList();
        ItemState[] tempInventoryDrops = e.Reader.ReadSerializables<ItemState>();
        ItemDrops = tempInventoryDrops.ToList();
        SpawnState[] tempPlayerSpawns = e.Reader.ReadSerializables<SpawnState>();
        PlayerSpawns = tempPlayerSpawns.ToList();
        EnemySpawnState[] tempEnemySpawns = e.Reader.ReadSerializables<EnemySpawnState>();
        EnemySpawns = tempEnemySpawns.ToList();
        SpawnState[] tempChestSpawns = e.Reader.ReadSerializables<SpawnState>();
        ChestSpawns = tempChestSpawns.ToList();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(Name);
        e.Writer.Write(Enemies.ToArray());
        e.Writer.Write(ItemDrops.ToArray());
        e.Writer.Write(PlayerSpawns.ToArray());
        e.Writer.Write(EnemySpawns.ToArray());
        e.Writer.Write(ChestSpawns.ToArray());
    }
}

