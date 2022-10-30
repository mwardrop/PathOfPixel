using DarkRift;
using DarkriftSerializationExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemStateType
{
    Chest,
    Feet,
    Head,
    Legs,
    Other,
    Weapon
}

public enum ItemStateSubType
{
    None,
    WeaponSword,
    WeaponAxe,
    WeaponStaff,
    WeaponHammer,
    WeaponFishingRod,
    OtherRing,
    OtherAmulet,
    OtherGloves
}

public enum ItemStateRarity
{
    Common,
    Magic,
    Rare,
    Legendary,
    Mythic,
    Set
}

[Serializable]
public class ItemState: IDarkRiftSerializable
{

    public string ItemName = "Generic Item";
    public string ItemDescription = "Generic Description";
    public int ItemImageId = 0;
    public ItemStateType ItemType = ItemStateType.Other;
    public ItemStateSubType ItemSubType = ItemStateSubType.None;
    public ItemStateRarity ItemRarity = ItemStateRarity.Common;
    public int ItemLevel = 1;
    public Guid ItemGuid;
    public Vector2 ItemSceneLocation;

    //public List<ItemOffenseModifier> offenseModifiers;
    //public List<ItemDefenseModifier> defenseModifiers;
    //public List<ItemCustomModifier> customModifiers;

    public ItemState()
    {
        ItemGuid = Guid.NewGuid();
    }

    public void Deserialize(DeserializeEvent e)
    {
        ItemName = e.Reader.ReadString();
        ItemDescription = e.Reader.ReadString();
        ItemImageId = e.Reader.ReadInt32();
        ItemType = (ItemStateType)e.Reader.ReadInt32();
        ItemSubType = (ItemStateSubType)e.Reader.ReadInt32();
        ItemRarity = (ItemStateRarity)e.Reader.ReadInt32();
        ItemLevel = e.Reader.ReadInt32();
        String tempGuid = e.Reader.ReadString();
        ItemGuid = Guid.Parse(tempGuid);
        ItemSceneLocation = e.Reader.ReadVector2();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(ItemName);
        e.Writer.Write(ItemDescription);
        e.Writer.Write(ItemImageId);
        e.Writer.Write((int)ItemType);
        e.Writer.Write((int)ItemSubType);
        e.Writer.Write((int)ItemRarity);
        e.Writer.Write(ItemLevel);
        e.Writer.Write(ItemGuid.ToString());
        e.Writer.WriteVector2(ItemSceneLocation);
    }
}
