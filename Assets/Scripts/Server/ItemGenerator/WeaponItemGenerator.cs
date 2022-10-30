using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItemGenerator: IitemGenerator
{
    // Ratios for getting specific item sub types to drop
    public static readonly int RATIO_ITEM_SWORD = 50;
    public static readonly int RATIO_ITEM_STAFF = 20;
    public static readonly int RATIO_ITEM_AXE = 20;
    public static readonly int RATIO_ITEM_HAMMER = 10;

    public static readonly int RATIO_ITEM_TOTAL = RATIO_ITEM_SWORD
                                           + RATIO_ITEM_STAFF
                                           + RATIO_ITEM_AXE
                                           + RATIO_ITEM_HAMMER;


    private int SwordImageAssetCount = 6;
    private int StaffImageAssetCount = 3;
    private int AxeImageAssetCount = 3;
    private int HammerImageAssetCount = 1;

    public ItemState RollItem(ItemState weapon)
    {

        weapon.ItemSubType = GetWeightedItemSubType();

        switch (weapon.ItemSubType)
        {
            case ItemStateSubType.WeaponSword:
                weapon.ItemImageId = Random.Range(0, SwordImageAssetCount - 1);
                break;
            case ItemStateSubType.WeaponStaff:
                weapon.ItemImageId = Random.Range(0, StaffImageAssetCount - 1);
                break;
            case ItemStateSubType.WeaponAxe:
                weapon.ItemImageId = Random.Range(0, AxeImageAssetCount - 1);
                break;
            case ItemStateSubType.WeaponHammer:
                weapon.ItemImageId = Random.Range(0, HammerImageAssetCount - 1);
                break;
        }

        return weapon;
    }

    private ItemStateSubType GetWeightedItemSubType()
    {
        int x = Random.Range(0, RATIO_ITEM_TOTAL);

        if ((x -= RATIO_ITEM_SWORD) < 0)
        {
            return ItemStateSubType.WeaponSword;
        }
        else if ((x -= RATIO_ITEM_STAFF) < 0)
        {
            return ItemStateSubType.WeaponStaff;
        }
        else if ((x -= RATIO_ITEM_AXE) < 0)
        {
            return ItemStateSubType.WeaponAxe;
        }
        else if ((x -= RATIO_ITEM_HAMMER) < 0)
        {
            return ItemStateSubType.WeaponHammer;
        }
        else
        {
            return ItemStateSubType.WeaponSword;
        }
    }

}
