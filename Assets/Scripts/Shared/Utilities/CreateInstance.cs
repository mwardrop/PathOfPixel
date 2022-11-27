
using Data.Attacks;
using Data.Skills;
using Data.Characters;
using System;
using UnityEngine;
using UnityEngine.Playables;
using Data.Passives;

public static class CreateInstance
{

    public static ICharacter Character(string name, int level = 1, CharacterRarity rarity = CharacterRarity.Common)
    {
        return (ICharacter)Activator.CreateInstance(
            Type.GetType($"Data.Characters.{name}"),
            level, rarity);
    }

    public static IAttack Attack(string name, int level = 1)
    {
        return (IAttack)Activator.CreateInstance(
            Type.GetType($"Data.Attacks.{name}"),
            level);
    }

    public static ISkill Skill(string name, int level = 1)
    {
        return (ISkill)Activator.CreateInstance(
            Type.GetType($"Data.Skills.{name}"),
            level);
    }

    public static IPassive Passive(string name, int level = 1)
    {
        return (IPassive)Activator.CreateInstance(
            Type.GetType($"Data.Passives.{name}"),
            level);
    }

    public static GameObject Prefab(GameObject prefab, Vector2 location)
    {
        return UnityEngine.Object.Instantiate(
            prefab,
            location,
            Quaternion.identity);
    }
}


