using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadItemGenerator : IitemGenerator
{

    private int ImageAssetCount = 5;

    public ItemState RollItem(ItemState head)
    {

        head.ItemImageId = Random.Range(0, ImageAssetCount - 1);

        return head;
    }
}
