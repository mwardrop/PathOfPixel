
using Data.Characters;
using System;
using System.Linq;
using UnityEngine.Playables;

public class StateCalculator
{

    public ICharacterState CalcCharacterState(ICharacterState characterState)
    {
        // Reset Calculated fields
        characterState.PhysicalDamage =
        characterState.FireDamage =
        characterState.ColdDamage =
        characterState.IncreasedDodge =
        characterState.IncreasedMoveSpeed =
        characterState.IncreasedMaxMana =
        characterState.IncreasedManaRegen =
        characterState.IncreasedMaxHealth =
        characterState.IncreasedHealthRegen =
        characterState.ReservedMana = 0;

        // Reset Character State
        PropertyCopier<ICharacter, ICharacterState>.Copy(
            CreateInstance.Character(characterState.Type.ToString(), characterState.Level),
            characterState);

        var activeAttack = CreateInstance.Attack(characterState.ActiveAttack,
            characterState.Attacks.First(x => x.Key == characterState.ActiveAttack).Value);

        foreach (KeyValueState _skill in characterState.ActiveSkills)
        {
            var skill = CreateInstance.Skill(_skill.Key, _skill.Index);
            skill.UpdateCharacterState(characterState);
        }

        foreach (KeyValueState _passive in characterState.Passives)
        {
            var passive = CreateInstance.Passive(_passive.Key, _passive.Value);
            passive.UpdateCharacterState(characterState);
        }

        // Apply % increases to base values
        characterState.PhysicalDamage += ((characterState.PhysicalDamage / 100) * characterState.IncreasedPhysicalDamage);
        characterState.FireDamage += ((characterState.FireDamage / 100) * characterState.IncreasedFireDamage);
        characterState.ColdDamage += ((characterState.ColdDamage / 100) * characterState.IncreasedColdDamage);
        characterState.IncreasedDodge += ((characterState.Dodge / 100) * characterState.IncreasedDodge);
        characterState.IncreasedMoveSpeed += ((characterState.MoveSpeed / 100) * characterState.IncreasedMoveSpeed);
        characterState.IncreasedMaxMana += ((characterState.MaxMana / 100) * characterState.IncreasedMaxMana);
        characterState.IncreasedManaRegen += ((characterState.ManaRegen / 100) * characterState.IncreasedManaRegen);
        characterState.IncreasedMaxHealth += ((characterState.MaxHealth / 100) * characterState.IncreasedMaxHealth);
        characterState.IncreasedHealthRegen += ((characterState.HealthRegen / 100) * characterState.IncreasedHealthRegen);
        characterState.ReservedMana = ((characterState.MaxMana / 100) * characterState.ReservedMana);

        return characterState;

    }


}

