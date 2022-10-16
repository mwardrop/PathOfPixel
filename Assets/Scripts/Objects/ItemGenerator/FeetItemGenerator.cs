using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetItemGenerator : IitemGenerator
{

    private int ImageAssetCount = 4;

    public InventoryItem RollItem(InventoryItem feet)
    {

        feet.itemImageId = Random.Range(0, ImageAssetCount - 1);

        return feet;
    }
}
