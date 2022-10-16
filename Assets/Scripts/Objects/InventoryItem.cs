using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InventoryItemType
{
    Chest,
    Feet,
    Head,
    Legs,
    Other,
    Weapon
}

public enum InventoryItemSubType
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

public enum InventoryItemRarity
{
    Common,
    Magic,
    Rare,
    Legendary,
    Mythic,
    Set
}

[Serializable]
public class InventoryItem
{

    public string itemName = "Generic Item";
    public string itemDescription = "Generic Description";
    public int itemImageId = 0;
    public InventoryItemType itemType = InventoryItemType.Other;
    public InventoryItemSubType itemSubType = InventoryItemSubType.None;
    public InventoryItemRarity itemRarity = InventoryItemRarity.Common;
    public int itemLevel = 1;
    public readonly Guid itemGuid;
    public String identifier = ""; // TODO: Remove when no longer need to see GUID in Unity Editor

    public List<ItemOffenseModifier> offenseModifiers;
    public List<ItemDefenseModifier> defenseModifiers;

    public List<ItemCustomModifier> customModifiers;

    public InventoryItem()
    {
        itemGuid = Guid.NewGuid();
        identifier = itemGuid.ToString();
    }

}
