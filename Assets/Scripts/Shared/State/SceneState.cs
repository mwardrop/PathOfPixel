using DarkRift;
using System.Collections.Generic;
using System.Linq;

public class SceneState : IDarkRiftSerializable
{
    public string Name;
    public List<EnemyState> Enemies;
    public List<ItemState> ItemDrops;

    public SceneState()
    {
        Enemies = new List<EnemyState>();
        ItemDrops = new List<ItemState>();
    }

    public void Deserialize(DeserializeEvent e)
    {
        Name = e.Reader.ReadString();
        EnemyState[] tempEnemies = e.Reader.ReadSerializables<EnemyState>();
        Enemies = tempEnemies.ToList();
        ItemState[] tempInventoryDrops = e.Reader.ReadSerializables<ItemState>();
        ItemDrops = tempInventoryDrops.ToList();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(Name);
        e.Writer.Write(Enemies.ToArray());
        e.Writer.Write(ItemDrops.ToArray());
    }
}

