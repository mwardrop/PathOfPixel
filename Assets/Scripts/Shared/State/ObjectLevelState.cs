
using DarkRift;

public class ObjectLevelState : IDarkRiftSerializable
{
    public int ObjectId { get; set; }
    public int Level { get; set; }

    public ObjectLevelState()
    {

    }

    public void Deserialize(DeserializeEvent e)
    {

        ObjectId = e.Reader.ReadInt32();
        Level = e.Reader.ReadInt32();

    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(ObjectId);
        e.Writer.Write(Level);

    }
}

