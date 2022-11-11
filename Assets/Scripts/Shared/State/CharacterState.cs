using DarkRift;
using DarkriftSerializationExtensions;
using Data;
using Data.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CharacterType
{
    Warrior,
    Mage,
    Possessed
}

public class CharacterState : ICharacterState, IDarkRiftSerializable
{
    public CharacterType Type { get; set; }
    public string Name { get; set; }
    public float Health { get; set; }
    public float MaxHealth { get; set; }
    public float HealthRegen { get; set; }
    public float Mana { get; set; }
    public float MaxMana { get; set; }
    public float ManaRegen { get; set; }
    public float PhysicalDamage { get; set; }
    public float BleedChance { get; set; }
    public float FireDamage { get; set; }
    public float BurnChance { get; set; }
    public float ColdDamage { get; set; }
    public float FreezeChance { get; set; }
    public float FireResistance { get; set; }
    public float ColdResistance { get; set; }
    public float Armor { get; set; }
    public float Dodge { get; set; }
    public float Accuracy { get; set; }
    public float CritChance { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public Vector2 Location { get; set; }
    public Vector2 TargetLocation { get; set; }
    public float MoveSpeed { get; set; }
    public bool IsDead { get; set; }
    public List<KeyValueState> Attacks { get; set; }
    public List<KeyValueState> Skills { get; set; }
    public List<KeyValueState> Passives { get; set; }
    public string ActiveAttack { get; set; }
    public List<KeyValueState> ActiveSkills { get; set; }

    public float IncomingPhysicalDamage { get; set; }
    public float IncomingFireDamage { get; set; }
    public float IncomingColdDamage { get; set; }

    public CharacterState()
    {
        Initialize();
    }

    public CharacterState(ICharacter character)
    {
        Initialize();

        PropertyCopier<ICharacter, CharacterState>.Copy(
            character,
            this);

        character.Attacks.ForEach(x => Attacks.Add(new KeyValueState() { Key = x.GetName(), Value = x.Level }));
        character.Skills.ForEach(x => Skills.Add(new KeyValueState() { Key = x.GetName(), Value = x.Level }));
        character.Passives.ForEach(x => Passives.Add(new KeyValueState() { Key = x.GetName(), Value = x.Level }));

        Type = (CharacterType)Enum.Parse(typeof(CharacterType), character.GetName());
        ActiveAttack = Attacks.First().Key;
    }

    private void Initialize()
    {
        Attacks = new List<KeyValueState>();
        Skills = new List<KeyValueState>();
        Passives = new List<KeyValueState>();
        ActiveSkills = new List<KeyValueState>();
        
    }

    public virtual void Deserialize(DeserializeEvent e)
    {
        Type = (CharacterType)e.Reader.ReadInt32();
        Name = e.Reader.ReadString();
        Health = e.Reader.ReadSingle();
        MaxHealth = e.Reader.ReadSingle();
        HealthRegen = e.Reader.ReadSingle();
        Mana = e.Reader.ReadSingle();
        MaxMana = e.Reader.ReadSingle();
        ManaRegen = e.Reader.ReadSingle();
        PhysicalDamage = e.Reader.ReadSingle();
        BleedChance = e.Reader.ReadSingle();
        FireDamage = e.Reader.ReadSingle();
        BurnChance = e.Reader.ReadSingle();
        ColdDamage = e.Reader.ReadSingle();
        FreezeChance = e.Reader.ReadSingle();
        FireResistance = e.Reader.ReadSingle();
        ColdResistance = e.Reader.ReadSingle();
        Armor = e.Reader.ReadSingle();
        Dodge = e.Reader.ReadSingle();
        Accuracy = e.Reader.ReadSingle();
        CritChance = e.Reader.ReadSingle();
        Level = e.Reader.ReadInt32();
        Experience = e.Reader.ReadInt32();
        Location = e.Reader.ReadVector2();
        TargetLocation = e.Reader.ReadVector2();
        MoveSpeed = e.Reader.ReadSingle();
        IsDead = e.Reader.ReadBoolean();
        KeyValueState[] tempAttacks = e.Reader.ReadSerializables<KeyValueState>();
        Attacks = tempAttacks.ToList();
        KeyValueState[] tempSkills = e.Reader.ReadSerializables<KeyValueState>();
        Skills = tempSkills.ToList();
        KeyValueState[] tempPassives = e.Reader.ReadSerializables<KeyValueState>();
        Passives = tempPassives.ToList();
        ActiveAttack = e.Reader.ReadString();
        KeyValueState[] tempActiveSkills = e.Reader.ReadSerializables<KeyValueState>();
        ActiveSkills = tempActiveSkills.ToList();
    }

    public virtual void Serialize(SerializeEvent e)
    {
        e.Writer.Write((int)Type);
        e.Writer.Write(Name);
        e.Writer.Write(Health);
        e.Writer.Write(MaxHealth);
        e.Writer.Write(HealthRegen);
        e.Writer.Write(Mana);
        e.Writer.Write(MaxMana);
        e.Writer.Write(ManaRegen);
        e.Writer.Write(PhysicalDamage);
        e.Writer.Write(BleedChance);
        e.Writer.Write(FireDamage);
        e.Writer.Write(BurnChance);
        e.Writer.Write(ColdDamage);
        e.Writer.Write(FreezeChance);
        e.Writer.Write(FireResistance);
        e.Writer.Write(ColdResistance);
        e.Writer.Write(Armor);
        e.Writer.Write(Dodge);
        e.Writer.Write(Accuracy);
        e.Writer.Write(CritChance);
        e.Writer.Write(Level);
        e.Writer.Write(Experience);
        e.Writer.WriteVector2(Location);
        e.Writer.WriteVector2(TargetLocation);
        e.Writer.Write(MoveSpeed);
        e.Writer.Write(IsDead);
        e.Writer.Write(Attacks.ToArray());
        e.Writer.Write(Skills.ToArray());
        e.Writer.Write(Passives.ToArray());
        e.Writer.Write(ActiveAttack);
        e.Writer.Write(ActiveSkills.ToArray());
    }
}

public interface ICharacterState
{
    public CharacterType Type { get; set; }
    public string Name { get; set; }
    public float Health { get; set; }
    public float MaxHealth { get; set; }
    public float HealthRegen { get; set; }
    public float Mana { get; set; }
    public float MaxMana { get; set; }
    public float ManaRegen { get; set; }
    public float PhysicalDamage { get; set; }
    public float BleedChance { get; set; }
    public float FireDamage { get; set; }
    public float BurnChance { get; set; }
    public float ColdDamage { get; set; }
    public float FreezeChance { get; set; }
    public float FireResistance { get; set; }
    public float ColdResistance { get; set; }
    public float Armor { get; set; }
    public float Dodge { get; set; }
    public float Accuracy { get; set; }
    public float CritChance { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public Vector2 Location { get; set; }
    public Vector2 TargetLocation { get; set; }
    public float MoveSpeed { get; set; }
    public bool IsDead { get; set; }
    public List<KeyValueState> Attacks { get; set; }
    public List<KeyValueState> Skills { get; set; }
    public List<KeyValueState> Passives { get; set; }
    public string ActiveAttack { get; set; }
    public List<KeyValueState> ActiveSkills { get; set; }

    public float IncomingPhysicalDamage { get; set; }
    public float IncomingFireDamage { get; set; }
    public float IncomingColdDamage { get; set; }
}