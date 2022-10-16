using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadItemGenerator : IitemGenerator
{

    private int ImageAssetCount = 5;

    public InventoryItem RollItem(InventoryItem head)
    {

        head.itemImageId = Random.Range(0, ImageAssetCount - 1);

        return head;
    }
}
