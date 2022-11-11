
using DarkRift;

public class KeyValueState : IDarkRiftSerializable
{
    public string Key { get; set; }
    public int Value { get; set; }

    public int Index { get; set; }

    public KeyValueState()
    {

    }

    public KeyValueState(string key, int value)
    {
        Key = key;
        Value = value;
    }

    public void Deserialize(DeserializeEvent e)
    {

        Key = e.Reader.ReadString();
        Value = e.Reader.ReadInt32();
        Index = e.Reader.ReadInt32();

    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(Key);
        e.Writer.Write(Value);
        e.Writer.Write(Index);

    }
}

