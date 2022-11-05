
using Data.Characters;
using System;
using System.Linq;
using UnityEngine.Playables;

public class StateCalculator
{

    public ICharacterState CalcCharacterState(ICharacterState characterState)
    {
        // Get Initial Character Type Details for given level
        ICharacter character = CreateInstance.Character(characterState.Type.ToString(), characterState.Level);

        // Apply State to Character Details
        CalcMaxHealth(characterState, character);
        CalcHealthRegen(characterState, character);
        CalcMaxMana(characterState, character);
        CalcManaRegen(characterState, character);
        CalcPhysicalDamage(characterState, character);
        CalcBleedChance(characterState, character);
        CalcFireDamage(characterState, character);
        CalcBurnChance(characterState, character);
        CalcColdDamage(characterState, character);
        CalcFreezeChance(characterState, character);
        CalcFireResistance(characterState, character);
        CalcColdResistance(characterState, character);
        CalcArmor(characterState, character);
        CalcDodge(characterState, character);
        CalcAccuracy(characterState, character);
        CalcCritChance(characterState, character);

        // Map Character to State Object
        PropertyCopier<ICharacter, ICharacterState>.Copy(
            character,
            characterState);

        return characterState;

    }

    private void CalcMaxHealth(ICharacterState characterState, ICharacter character) { }
    private void CalcHealthRegen(ICharacterState characterState, ICharacter character) { }
    private void CalcMaxMana(ICharacterState characterState, ICharacter character) { }
    private void CalcManaRegen(ICharacterState characterState, ICharacter character) { }
    private void CalcPhysicalDamage(ICharacterState characterState, ICharacter character) 
    {
        var activeAttack = CreateInstance.Attack(characterState.ActiveAttack, 
            characterState.Attacks.First(x => x.Key == characterState.ActiveAttack).Value);

        character.PhysicalDamage += activeAttack.PhysicalDamage;

    }
    private void CalcBleedChance(ICharacterState characterState, ICharacter character) { }
    private void CalcFireDamage(ICharacterState characterState, ICharacter character) { }
    private void CalcBurnChance(ICharacterState characterState, ICharacter character) { }
    private void CalcColdDamage(ICharacterState characterState, ICharacter character) { }
    private void CalcFreezeChance(ICharacterState characterState, ICharacter character) { }
    private void CalcFireResistance(ICharacterState characterState, ICharacter character) { }
    private void CalcColdResistance(ICharacterState characterState, ICharacter character) { }
    private void CalcArmor(ICharacterState characterState, ICharacter character) { }
    private void CalcDodge(ICharacterState characterState, ICharacter character) { }
    private void CalcAccuracy(ICharacterState characterState, ICharacter character) { }
    private void CalcCritChance(ICharacterState characterState, ICharacter character) { }



}

