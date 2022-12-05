using DarkRift;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class InventoryState : IDarkRiftSerializable
{
    public List<InventoryItemState> Items;
    public List<InventoryItemState> Equiped;
    public int StandardCurrency = 0;
    public int PremiumCurrency = 0;

    public InventoryState() {
        Items = new List<InventoryItemState>();
        Equiped = new List<InventoryItemState>();
    }


    public void Deserialize(DeserializeEvent e)
    {
        InventoryItemState[] tempItems = e.Reader.ReadSerializables<InventoryItemState>();
        Items = tempItems.ToList();
        InventoryItemState[] tempEquiped = e.Reader.ReadSerializables<InventoryItemState>();
        Equiped = tempEquiped.ToList();
        StandardCurrency = e.Reader.ReadInt32();
        PremiumCurrency = e.Reader.ReadInt32();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(Items.ToArray());
        e.Writer.Write(Equiped.ToArray());
        e.Writer.Write(StandardCurrency);
        e.Writer.Write(PremiumCurrency);
    }
}
