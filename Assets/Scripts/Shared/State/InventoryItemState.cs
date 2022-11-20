using DarkRift;
using System;

public enum InventorySlots
{
    Slot1 = 0, Slot2 = 1, Slot3 = 2, Slot4 = 3, Slot5 = 4,
    Slot6 = 5, Slot7 = 6, Slot8 = 7, Slot9 = 8, Slot10 = 9,
    Slot11 = 10, Slot12 = 11, Slot13 = 12, Slot14 = 13, Slot15 = 14,
    Slot16 = 15, Slot17 = 16, Slot18 = 17, Slot19 = 18, Slot20 = 19,
    Slot21 = 20, Slot22 = 21, Slot23 = 22, Slot24 = 23, Slot25 = 24, 
    Slot26 = 25, Slot27 = 26, Slot28 = 27, Slot29 = 28, Slot30 = 29,
    Slot31 = 30, Slot32 = 31, Slot33 = 32, Slot34 = 33, Slot35 = 34,
    Slot36 = 35, Slot37 = 36, Slot38 = 37, Slot39 = 38, Slot40 = 39,
    Slot41 = 40, Slot42 = 41, Slot43 = 42, Slot44 = 43, Slot45 = 44,
    Slot46 = 45, Slot47 = 46, Slot48 = 47, Slot49 = 48, Slot50 = 49,

    Head = 51,
    Chest = 52,
    Legs = 53,
    Feet = 54,
    Weapon = 55,
    Other1 = 56,
    Other2 = 57
}

[Serializable]
public class InventoryItemState: IDarkRiftSerializable
{
    public InventorySlots Slot;
    public ItemState Item;

    public InventoryItemState() { }

    public InventoryItemState(int slot, ItemState item)
    {
        Slot = (InventorySlots)slot;
        Item = item;
    }

    public void Deserialize(DeserializeEvent e)
    {
        Slot = (InventorySlots)e.Reader.ReadInt32();
        Item = e.Reader.ReadSerializable<ItemState>();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write((int)Slot);
        e.Writer.Write(Item);
    }
}
