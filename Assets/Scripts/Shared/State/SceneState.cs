using DarkRift;
using System.Collections.Generic;
using System.Linq;

public class SceneState : IDarkRiftSerializable
{
    public string Name;
    public List<EnemyState> Enemies;

    public SceneState()
    {
        Enemies = new List<EnemyState>();
    }

    public void Deserialize(DeserializeEvent e)
    {
        Name = e.Reader.ReadString();
        EnemyState[] tempEnemies = e.Reader.ReadSerializables<EnemyState>();
        Enemies = tempEnemies.ToList();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(Name);
        e.Writer.Write(Enemies.ToArray());
    }
}

