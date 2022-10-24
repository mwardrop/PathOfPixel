using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegsItemGenerator : IitemGenerator
{

    private int ImageAssetCount = 5;

    public InventoryItem RollItem(InventoryItem legs)
    {

        legs.ItemImageId = Random.Range(0, ImageAssetCount - 1);

        return legs;
    }
}
