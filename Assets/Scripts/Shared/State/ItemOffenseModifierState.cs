using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemOffenseModifierType
{
    PhysicalDamage,
    FireDamage,
    ColdDamage
}

[Serializable]
public class ItemOffenseModifier : ScriptableObject
{

    public ItemOffenseModifierType modifierType;
    public float lowValue;
    public float highValue;

}
