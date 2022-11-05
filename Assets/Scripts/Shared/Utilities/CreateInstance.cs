
using Data.Attacks;
using Data.Characters;
using System;

public static class CreateInstance
{

    public static ICharacter Character(string name, int level = 1)
    {
        return (ICharacter)Activator.CreateInstance(
            Type.GetType($"Data.Characters.{name}"),
            level);
    }

    public static IAttack Attack(string name, int level = 1)
    {
        return (IAttack)Activator.CreateInstance(
            Type.GetType($"Data.Attacks.{name}"),
            level);
    }
}


