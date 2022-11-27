using Data.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public string ZoneName;
    public string CharacterName;
    public CharacterRarity CharacterRarity = CharacterRarity.Common;
    public EnemyCharacters Character;
    public int Minimum;
    public int Maximum;
    public int MaxLevel;
    public int MinLevel;
    public bool ActivateSkills;
    public float ChanceOfChest = 0.0f;
}
