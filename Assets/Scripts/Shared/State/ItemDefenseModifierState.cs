using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemDefenseModifierType
{
    Armor,
    Dodge,
    FireResistance,
    ColdResistance,
}

public class ItemDefenseModifier : ScriptableObject
{
    public ItemDefenseModifierType modifierType;
    public float value;
}
