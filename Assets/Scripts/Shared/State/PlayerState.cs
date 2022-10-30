using DarkRift;
using DarkRift.Server;
using System;
using System.Collections.Generic;
using System.Linq;

public enum PlayerType
{
    Warrior,
    Mage
}

[Serializable]
public class PlayerState: CharacterState, ICharacterState, IDarkRiftSerializable
{
    public PlayerType Type;

    public List<ItemState> Inventory;

    public string Scene;
    public int ClientId;

    public PlayerState()
    {
        Inventory = new List<ItemState>();
    }

    public override void Deserialize(DeserializeEvent e)
    {
        base.Deserialize(e);
        Type = (PlayerType)e.Reader.ReadInt32();
        Scene = e.Reader.ReadString();
        ClientId = e.Reader.ReadInt32();
        ItemState[] tempInventory = e.Reader.ReadSerializables<ItemState>();
        Inventory = tempInventory.ToList();

    }

    public override void Serialize(SerializeEvent e)
    {
        base.Serialize(e);
        e.Writer.Write((int)Type);
        e.Writer.Write(Scene);
        e.Writer.Write(ClientId);
        e.Writer.Write(Inventory.ToArray());
    }

}
