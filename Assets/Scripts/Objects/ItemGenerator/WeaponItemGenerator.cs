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

    public InventoryItem RollItem(InventoryItem weapon)
    {

        weapon.ItemSubType = GetWeightedItemSubType();

        switch (weapon.ItemSubType)
        {
            case InventoryItemSubType.WeaponSword:
                weapon.ItemImageId = Random.Range(0, SwordImageAssetCount - 1);
                break;
            case InventoryItemSubType.WeaponStaff:
                weapon.ItemImageId = Random.Range(0, StaffImageAssetCount - 1);
                break;
            case InventoryItemSubType.WeaponAxe:
                weapon.ItemImageId = Random.Range(0, AxeImageAssetCount - 1);
                break;
            case InventoryItemSubType.WeaponHammer:
                weapon.ItemImageId = Random.Range(0, HammerImageAssetCount - 1);
                break;
        }

        return weapon;
    }

    private InventoryItemSubType GetWeightedItemSubType()
    {
        int x = Random.Range(0, RATIO_ITEM_TOTAL);

        if ((x -= RATIO_ITEM_SWORD) < 0)
        {
            return InventoryItemSubType.WeaponSword;
        }
        else if ((x -= RATIO_ITEM_STAFF) < 0)
        {
            return InventoryItemSubType.WeaponStaff;
        }
        else if ((x -= RATIO_ITEM_AXE) < 0)
        {
            return InventoryItemSubType.WeaponAxe;
        }
        else if ((x -= RATIO_ITEM_HAMMER) < 0)
        {
            return InventoryItemSubType.WeaponHammer;
        }
        else
        {
            return InventoryItemSubType.WeaponSword;
        }
    }

}
