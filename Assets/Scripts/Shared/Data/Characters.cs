using Data.Attacks;
using Data.Skills;
using Data.Passives;
using System.Collections.Generic;
using System.Linq;

namespace Data.Characters
{
    public enum CharacterRarity
    {
        Common = 1,
        Magic = 2,
        Rare = 3,
        Legendary = 4,
        Mythic = 5,
        Boss = 10,
        Player = 0
    }

    public enum PlayerCharacters
    {
        Warrior
    }

    public enum EnemyCharacters
    {
        Possessed
    }

    public interface ICharacter
    {
        public float MaxHealth { get; set; }
        public int Level { get; set; }
        public float HealthRegen { get; set; }
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
        public float MoveSpeed { get; set; }
        public int Experience { get; set; }
        public CharacterRarity Rarity {get; set;}

        public List<IAttack> Attacks { get; set; }
        public List<ISkill> Skills { get; set; }
        public List<IPassive> Passives { get; set; }

        public string GetName();
    }

    public class BaseCharacter : ICharacter
    {
        public float MaxHealth { get; set; }
        public int Level { get; set; }
        public float HealthRegen { get; set; }
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
        public float MoveSpeed { get; set; }
        public int Experience { get; set; }
        public CharacterRarity Rarity { get; set; }

        public List<IAttack> Attacks { get; set; }
        public List<ISkill> Skills { get; set; }
        public List<IPassive> Passives { get; set; }

        public string GetName()
        {
            return this.GetType().ToString().Split(".").Last();
        }

        public BaseCharacter(int level = 1, CharacterRarity rarity = CharacterRarity.Common)
        {
            Level = level;
            Rarity = rarity;
        }

    }

    public class Warrior: BaseCharacter, ICharacter
    {
        public Warrior(int level = 1, CharacterRarity rarity = CharacterRarity.Player) : base(level, rarity)
        {
            MaxHealth = 100 + (level * 10);
            HealthRegen = 1 + (level * 2);
            MaxMana = 20 + level;
            ManaRegen = 1 + level;
            PhysicalDamage = 5 + (level * 5);
            Armor = 100 + (level * 10);
            MoveSpeed = 3f;
            BleedChance = 1;
            Accuracy = 20;
            Experience = 0;// (1000 * level) * level;
            
            Attacks = new List<IAttack>() {
                new SweepAttack(1),
                new SlamAttack(0)
            };

            Skills = new List<ISkill>()
            {
                new SummerSolsticeSkill(0),
                new WinterSolsticeSkill(0),
                new FrenzySkill(1),
                new BurningSummerSkill(0),
                new FreezingWinterSkill(0),
                new BleedingFurySkill(0),
                new WarmEmbraceSkill(0),
                new ColdEmbraceSkill(0)
            };

            Passives = new List<IPassive>()
            {
                new AggressionPassive(0),
                new ImmolationPassive(0),
                new HypothermiaPassive(0),
                new StaminaPassive(0),
                new IntelligencePassive(0),
                new StrengthPassive(0),
                new HealthPassive(0),
                new ManaPassive(0)

            };
        }
    }

    public class Possessed : BaseCharacter, ICharacter
    {
        public Possessed(int level = 1, CharacterRarity rarity = CharacterRarity.Common) : base(level, rarity)
        {
            MaxHealth = 10 + (level * 5) * (int)Rarity;
            HealthRegen = 1;
            MaxMana = 100;
            ManaRegen = 1;
            PhysicalDamage = 5 + (level * 2) * (int)Rarity;
            MoveSpeed = 1f;
            Experience = 10 * level * (int)Rarity;

            Attacks = new List<IAttack>() {
                new SweepAttack(level)
            };

            Skills = new List<ISkill>()
            {
                new WarmEmbraceSkill(level),
                new FrenzySkill(level)
            };

            Passives = new List<IPassive>();
        }
    }

}
