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
    public Sprite itemImage;
    public InventoryItemType itemType = InventoryItemType.Other;
    public InventoryItemRarity itemRarity = InventoryItemRarity.Common;
    public int itemLevel = 1;
    public readonly Guid itemGuid;
    public String identifier = "test";

    public List<ItemOffenseModifier> offenseModifiers;
    public List<ItemDefenseModifier> defenseModifiers;

    public List<ItemCustomModifier> customModifiers;

    public InventoryItem()
    {
        itemGuid = Guid.NewGuid();
        identifier = itemGuid.ToString();
    }

}
