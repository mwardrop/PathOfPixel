
using DarkRift;

public class KeyValueState : IDarkRiftSerializable
{
    public string Key { get; set; }
    public int Value { get; set; }

    public KeyValueState()
    {

    }

    public void Deserialize(DeserializeEvent e)
    {

        Key = e.Reader.ReadString();
        Value = e.Reader.ReadInt32();

    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(Key);
        e.Writer.Write(Value);

    }
}

