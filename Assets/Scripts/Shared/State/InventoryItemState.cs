using DarkRift;
using System;

[Serializable]
public class InventoryItemState: IDarkRiftSerializable
{
    public int Slot;
    public ItemState Item;

    public InventoryItemState() { }

    public InventoryItemState(int slot, ItemState item)
    {
        Slot = slot;
        Item = item;
    }

    public void Deserialize(DeserializeEvent e)
    {
        Slot = e.Reader.ReadInt32();
        Item = e.Reader.ReadSerializable<ItemState>();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(Slot);
        e.Writer.Write(Item);
    }
}
