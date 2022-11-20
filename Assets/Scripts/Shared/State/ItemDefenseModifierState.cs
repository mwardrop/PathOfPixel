using System;
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

[Serializable]
public class ItemDefenseModifier : ScriptableObject
{
    public ItemDefenseModifierType modifierType;
    public float value;
}
