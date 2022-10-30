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


    public ItemState createInventoryItem(int itemLevel)
    {
        
        return createInventoryItem(
            itemLevel,
            (ItemStateType)Random.Range(0, typeof(ItemStateType).GetEnumValues().Length)
        );
    }

    public ItemState createInventoryItem(int itemLevel, ItemStateType itemType)
    {

        return createInventoryItem(
            itemLevel,
            itemType,
            GetWeightedItemRarity()
        );
    }

    public ItemState createInventoryItem(int itemLevel, ItemStateType itemType, ItemStateRarity itemRarity)
    {
        ItemState item = new ItemState();
        item.ItemLevel = Random.Range(itemLevel, itemLevel + 5);
        item.ItemType = itemType;
        item.ItemRarity = itemRarity;

        switch(item.ItemType)
        {
            case ItemStateType.Chest:
                return chestItemGenerator.RollItem(item);
            case ItemStateType.Feet:
                return feetItemGenerator.RollItem(item);
            case ItemStateType.Head:
                return headItemGenerator.RollItem(item);
            case ItemStateType.Legs:
                return legsItemGenerator.RollItem(item);
            case ItemStateType.Other:
                return otherItemGenerator.RollItem(item);
            case ItemStateType.Weapon:
                return weaponItemGenerator.RollItem(item);
            default:
                return otherItemGenerator.RollItem(item);

        }
    }

    private ItemStateRarity GetWeightedItemRarity()
    {
        int x = Random.Range(0, RATIO_ITEM_TOTAL);

        if ((x -= RATIO_ITEM_COMMON) < 0)
        {
            return ItemStateRarity.Common;
        }
        else if ((x -= RATIO_ITEM_MAGIC) < 0)
        {
            return ItemStateRarity.Magic;
        }
        else if ((x -= RATIO_ITEM_RARE) < 0)
        {
            return ItemStateRarity.Rare;
        }
        else if ((x -= RATIO_ITEM_LEGENDARY) < 0)
        {
            return ItemStateRarity.Legendary;
        }
        else if ((x -= RATIO_ITEM_MYTHIC) < 0)
        {
            return ItemStateRarity.Mythic;
        }
        else if ((x -= RATIO_ITEM_SET) < 0)
        {
            return ItemStateRarity.Set;
        } else
        {
            return ItemStateRarity.Common;
        }
    }

}

public interface IitemGenerator
{
    ItemState RollItem(ItemState item);
}