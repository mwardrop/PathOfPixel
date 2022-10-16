using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator
{
    // Ratios for getting specific item rarities to drop
    public static readonly int RATIO_ITEM_COMMON = 80;
    public static readonly int RATIO_ITEM_MAGIC = 10;
    public static readonly int RATIO_ITEM_RARE = 5;
    public static readonly int RATIO_ITEM_LEGENDARY = 3;
    public static readonly int RATIO_ITEM_MYTHIC = 2;
    public static readonly int RATIO_ITEM_SET = 0;

    public static readonly int RATIO_ITEM_TOTAL = RATIO_ITEM_COMMON
                                           + RATIO_ITEM_MAGIC
                                           + RATIO_ITEM_RARE
                                           + RATIO_ITEM_LEGENDARY
                                           + RATIO_ITEM_MYTHIC
                                           + RATIO_ITEM_SET;

    private ChestItemGenerator chestItemGenerator = new ChestItemGenerator();
    private FeetItemGenerator feetItemGenerator = new FeetItemGenerator(); 
    private HeadItemGenerator headItemGenerator = new HeadItemGenerator();
    private LegsItemGenerator legsItemGenerator = new LegsItemGenerator();
    private OtherItemGenerator otherItemGenerator = new OtherItemGenerator();
    private WeaponItemGenerator weaponItemGenerator = new WeaponItemGenerator();


    public InventoryItem createInventoryItem(int itemLevel)
    {
        
        return createInventoryItem(
            itemLevel,
            (InventoryItemType)Random.Range(0, typeof(InventoryItemType).GetEnumValues().Length)
        );
    }

    public InventoryItem createInventoryItem(int itemLevel, InventoryItemType itemType)
    {

        return createInventoryItem(
            itemLevel,
            itemType,
            GetWeightedItemRarity()
        );
    }

    public InventoryItem createInventoryItem(int itemLevel, InventoryItemType itemType, InventoryItemRarity itemRarity)
    {
        InventoryItem item = new InventoryItem();
        item.itemLevel = itemLevel;
        item.itemType = itemType;
        item.itemRarity = itemRarity;

        switch(item.itemType)
        {
            case InventoryItemType.Chest:
                return chestItemGenerator.RollItem(item);
            case InventoryItemType.Feet:
                return feetItemGenerator.RollItem(item);
            case InventoryItemType.Head:
                return headItemGenerator.RollItem(item);
            case InventoryItemType.Legs:
                return legsItemGenerator.RollItem(item);
            case InventoryItemType.Other:
                return otherItemGenerator.RollItem(item);
            case InventoryItemType.Weapon:
                return weaponItemGenerator.RollItem(item);
            default:
                return otherItemGenerator.RollItem(item);

        }
    }

    private InventoryItemRarity GetWeightedItemRarity()
    {
        int x = Random.Range(0, RATIO_ITEM_TOTAL);

        if ((x -= RATIO_ITEM_COMMON) < 0)
        {
            return InventoryItemRarity.Common;
        }
        else if ((x -= RATIO_ITEM_MAGIC) < 0)
        {
            return InventoryItemRarity.Magic;
        }
        else if ((x -= RATIO_ITEM_RARE) < 0)
        {
            return InventoryItemRarity.Rare;
        }
        else if ((x -= RATIO_ITEM_LEGENDARY) < 0)
        {
            return InventoryItemRarity.Legendary;
        }
        else if ((x -= RATIO_ITEM_MYTHIC) < 0)
        {
            return InventoryItemRarity.Mythic;
        }
        else if ((x -= RATIO_ITEM_SET) < 0)
        {
            return InventoryItemRarity.Set;
        } else
        {
            return InventoryItemRarity.Common;
        }
    }

}

public interface IitemGenerator
{
    InventoryItem RollItem(InventoryItem item);
}
