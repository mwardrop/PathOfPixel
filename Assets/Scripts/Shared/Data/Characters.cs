using Data.Attacks;
using System.Collections.Generic;

namespace Data.Characters
{ 
    public interface ICharacter
    {
        public int Id { get; set; }
        public float Health { get; set; }
        public float HealthRegen { get; set; }
        public float Mana { get; set; }
        public float ManaRegen { get; set; }
        public float PhysicalDamage { get; set; }
        public float FireDamage { get; set; }
        public float ColdDamage { get; set; }
        public float FireResistance { get; set; }
        public float ColdResistance { get; set; }
        public float Armor { get; set; }
        public float Dodge { get; set; }
        public float MoveSpeed { get; set; }

        public List<IAttack> Attacks { get; set; }
        public List<ISkill> Skills { get; set; }
        public List<IPassive> Passives { get; set; }

        public ICharacter ApplyCharacterState(CharacterState character);
    }

    public class BaseCharacter : ICharacter
    {
        public int Id { get; set; }
        public float Health { get; set; }
        public float HealthRegen { get; set; }
        public float Mana { get; set; }
        public float ManaRegen { get; set; }
        public float PhysicalDamage { get; set; }
        public float FireDamage { get; set; }
        public float ColdDamage { get; set; }
        public float FireResistance { get; set; }
        public float ColdResistance { get; set; }
        public float Armor { get; set; }
        public float Dodge { get; set; }
        public float MoveSpeed { get; set; }

        public List<IAttack> Attacks { get; set; }
        public List<ISkill> Skills { get; set; }
        public List<IPassive> Passives { get; set; }

        public virtual ICharacter ApplyCharacterState(CharacterState character) { return this; }

    }

    public class Warrior: BaseCharacter, ICharacter
    {
        public Warrior()
        {
            Id = 1;
            Health = 10000;
            HealthRegen = 1;
            Mana = 100;
            ManaRegen = 1;
            PhysicalDamage = 5;
            FireDamage = 0;
            ColdDamage = 0;
            FireResistance = 0;
            ColdResistance = 0;
            Armor = 0;
            Dodge = 0;
            MoveSpeed = 3f;

            Attacks = new List<IAttack>() {
                new SweepAttack(),
                new SlamAttack()
            };

            Skills = new List<ISkill>()
            {
                new SummerSolsticeSkill(),
                new WinterSolsticeSkill(),
                new FrenzySkill(),
                new BurningSummerSkill(),
                new FreezingWinterSkill(),
                new BleedingFurySkill()
            };

            Passives = new List<IPassive>()
            {
                new AggressionPassive(),
                new ImmolationPassive(),
                new HypothermiaPassive()          
            };
        }

        public override ICharacter ApplyCharacterState(CharacterState character)
        {
            var playerState = (PlayerState)character;

            return new Warrior()
            {
                PhysicalDamage = PhysicalDamage + (playerState.Level  * 5),
            };
        }

    }

    public class Possessed : BaseCharacter, ICharacter
    {
        public Possessed()
        {
            Id = 2;
            Health = 3000;
            HealthRegen = 1;
            Mana = 100;
            ManaRegen = 1;
            PhysicalDamage = 5;
            FireDamage = 0;
            ColdDamage = 0;
            FireResistance = 0;
            ColdResistance = 0;
            Armor = 0;
            Dodge = 0;
            MoveSpeed = 1f;

            Attacks = new List<IAttack>() {
                new SweepAttack()
            };

            Skills = new List<ISkill>();

            Passives = new List<IPassive>();
        }

        public override ICharacter ApplyCharacterState(CharacterState character)
        {
            var enemy = (PlayerState)character;

            return new Warrior()
            {
                PhysicalDamage = PhysicalDamage + (character.Level * 1),
            };
        }

    }

}
