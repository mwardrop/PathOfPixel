using Data.Attacks;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Data.Characters
{ 
    public interface ICharacter
    {
        public float Health { get; set; }
        public int Level { get; set; }
        public float HealthRegen { get; set; }
        public float Mana { get; set; }
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

        public List<IAttack> Attacks { get; set; }
        public List<ISkill> Skills { get; set; }
        public List<IPassive> Passives { get; set; }

        public string GetName();
    }

    public class BaseCharacter : ICharacter
    {
        public float Health { get; set; }
        public int Level { get; set; }
        public float HealthRegen { get; set; }
        public float Mana { get; set; }
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

        public List<IAttack> Attacks { get; set; }
        public List<ISkill> Skills { get; set; }
        public List<IPassive> Passives { get; set; }

        public string GetName()
        {
            return this.GetType().ToString().Split(".").Last();
        }

        public BaseCharacter(int level = 1)
        {
            Level = level;
        }

    }

    public class Warrior: BaseCharacter, ICharacter
    {
        public Warrior(int level = 1) : base(level)
        {
            Health = 100 + (level * 10);
            HealthRegen = 1 + (level * 2);
            Mana = 20 + level;
            ManaRegen = 1 + level;
            PhysicalDamage = 5 + (level * 5);
            Armor = 100 + (level * 10);
            MoveSpeed = 3f;
            BleedChance = 1;
            Accuracy = 20;
            

            Attacks = new List<IAttack>() {
                new SweepAttack(1),
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
    }

    public class Possessed : BaseCharacter, ICharacter
    {
        public Possessed(int level = 1) : base(level)
        {
            Health = 10000; //10 + (level * 5);
            HealthRegen = 1;
            Mana = 100;
            ManaRegen = 1;
            PhysicalDamage = 0; // 5 + (level * 2);
            MoveSpeed = 1f;

            Attacks = new List<IAttack>() {
                new SweepAttack(){Level = 1}
            };

            Skills = new List<ISkill>();

            Passives = new List<IPassive>();
        }
    }

}
