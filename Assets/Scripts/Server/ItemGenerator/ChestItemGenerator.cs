using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestItemGenerator : IitemGenerator
{
    private int ImageAssetCount = 8;

    public ItemState RollItem(ItemState chest)
    {
        chest.ItemImageId = Random.Range(0, ImageAssetCount - 1);

        return chest;
    }

}
