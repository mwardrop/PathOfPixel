using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherItemGenerator : IitemGenerator
{
    // Ratios for getting specific item sub types to drop
    public static readonly int RATIO_ITEM_RING = 45;
    public static readonly int RATIO_ITEM_AMULET = 45;
    public static readonly int RATIO_ITEM_GLOVES = 10;


    public static readonly int RATIO_ITEM_TOTAL = RATIO_ITEM_RING
                                           + RATIO_ITEM_AMULET
                                           + RATIO_ITEM_GLOVES;


    private int GlovesImageAssetCount = 1;
    private int AmuletImageAssetCount = 3;
    private int RingImageAssetCount = 3;

    public ItemState RollItem(ItemState other)
    {

        other.ItemSubType = GetWeightedItemSubType();

        switch (other.ItemSubType) {
            case ItemStateSubType.OtherGloves:
                other.ItemImageId = Random.Range(0, GlovesImageAssetCount - 1);
                break;
            case ItemStateSubType.OtherRing:
                other.ItemImageId = Random.Range(0, RingImageAssetCount - 1);
                break;
            case ItemStateSubType.OtherAmulet:
                other.ItemImageId = Random.Range(0, AmuletImageAssetCount - 1);
                break;
        }

        return other;
    }

    private ItemStateSubType GetWeightedItemSubType()
    {
        int x = Random.Range(0, RATIO_ITEM_TOTAL);

        if ((x -= RATIO_ITEM_RING) < 0)
        {
            return ItemStateSubType.OtherRing;
        }
        else if ((x -= RATIO_ITEM_AMULET) < 0)
        {
            return ItemStateSubType.OtherAmulet;
        }
        else if ((x -= RATIO_ITEM_GLOVES) < 0)
        {
            return ItemStateSubType.OtherGloves;
        }
        else
        {
            return ItemStateSubType.OtherRing;
        }
    }

}
