using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayerType
{
    Warrior,
    Mage
}

[CreateAssetMenu(fileName = "PlayerState", menuName = "PathOfPixel/PlayerState", order = 3)]
public class PlayerState: ScriptableObject {

    public float health;
    public float physicalDamage;
    public float fireDamage;
    public float coldDamage;
    public float fireResistance;
    public float coldResistance;
    public float mana;
    public PlayerType type;

    public List<InventoryItem> inventory;

    public void OnEnable()
    {
        if (inventory == null) { inventory = new List<InventoryItem>(); }
    }
}
