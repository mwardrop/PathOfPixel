using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherItemGenerator : IitemGenerator
{
    public InventoryItem RollItem(InventoryItem other)
    {
        return other;
    }
}
