
using Data.Characters;
using System;
using UnityEngine.Playables;

public class StateCalculator
{

    public ICharacterState CalculateCharacterState(ICharacterState characterState)
    {
        // Get Initial Character Type Details for given level
        ICharacter character = (ICharacter)Activator.CreateInstance(
            Type.GetType($"Data.Characters.{characterState.Type.ToString()}"),
            characterState.Level);

        // Apply State to Character Details
        UpdateMaxHealth(characterState, character);
        UpdateHealthRegen(characterState, character);
        UpdateMaxMana(characterState, character);
        UpdateManaRegen(characterState, character);
        UpdatePhysicalDamage(characterState, character);
        UpdateBleedChance(characterState, character);
        UpdateFireDamage(characterState, character);
        UpdateBurnChance(characterState, character);
        UpdateColdDamage(characterState, character);
        UpdateFreezeChance(characterState, character);
        UpdateFireResistance(characterState, character);
        UpdateColdResistance(characterState, character);
        UpdateArmor(characterState, character);
        UpdateDodge(characterState, character);
        UpdateAccuracy(characterState, character);
        UpdateCritChance(characterState, character);

        // Map Character to State Object
        PropertyCopier<ICharacter, ICharacterState>.Copy(
            character,
            characterState);

        return characterState;

    }

    private void UpdateMaxHealth(ICharacterState characterState, ICharacter character) { }
    private void UpdateHealthRegen(ICharacterState characterState, ICharacter character) { }
    private void UpdateMaxMana(ICharacterState characterState, ICharacter character) { }
    private void UpdateManaRegen(ICharacterState characterState, ICharacter character) { }
    private void UpdatePhysicalDamage(ICharacterState characterState, ICharacter character) { }
    private void UpdateBleedChance(ICharacterState characterState, ICharacter character) { }
    private void UpdateFireDamage(ICharacterState characterState, ICharacter character) { }
    private void UpdateBurnChance(ICharacterState characterState, ICharacter character) { }
    private void UpdateColdDamage(ICharacterState characterState, ICharacter character) { }
    private void UpdateFreezeChance(ICharacterState characterState, ICharacter character) { }
    private void UpdateFireResistance(ICharacterState characterState, ICharacter character) { }
    private void UpdateColdResistance(ICharacterState characterState, ICharacter character) { }
    private void UpdateArmor(ICharacterState characterState, ICharacter character) { }
    private void UpdateDodge(ICharacterState characterState, ICharacter character) { }
    private void UpdateAccuracy(ICharacterState characterState, ICharacter character) { }
    private void UpdateCritChance(ICharacterState characterState, ICharacter character) { }



}

