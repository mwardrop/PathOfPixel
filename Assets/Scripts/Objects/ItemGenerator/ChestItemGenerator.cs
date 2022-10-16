using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestItemGenerator : IitemGenerator
{
    private int ImageAssetCount = 8;

    public InventoryItem RollItem(InventoryItem chest)
    {
        chest.itemImageId = Random.Range(0, ImageAssetCount - 1);

        return chest;
    }

}
