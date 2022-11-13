

using System.Linq;
using Unity.VisualScripting;

namespace Data.Skills
{
    public interface ISkill
    {
        public int Level { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int IconId { get; set; }
        public float Duration { get; set; }
        public float Radius { get; set; }
        public float ManaReservation { get; set; }

        public string GetName();
        public ICharacterState UpdateCharacterState(ICharacterState characterState);
    }

    public class BaseSkill: ISkill
    {
        public int Level { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int IconId { get; set; }
        public float Duration { get; set; }
        public float Radius { get; set; }
        public float ManaReservation { get; set; }

        public string GetName()
        {
            return this.GetType().ToString().Split(".").Last();
        }

        public BaseSkill(int level)
        {
            Level = level;
            ManaReservation = 20;
        }

        public virtual ICharacterState UpdateCharacterState(ICharacterState characterState) {
            characterState.ReservedMana += ManaReservation;
            return characterState; }
    }

    public class SummerSolsticeSkill : BaseSkill, ISkill
    {
        public SummerSolsticeSkill(int level = 1) : base(level)
        {
            Name = "Summer Solstice";
            Description = "Add +1 Fire Damage to Attacks per Level for you and Allies within 5 Radius.";
            IconId = 1;
            Radius = 5;
            Duration = 60;
        }
        public override ICharacterState UpdateCharacterState(ICharacterState characterState)
        {
            base.UpdateCharacterState(characterState);
            characterState.FireDamage += 1 * Level;
            return characterState;
        }
    }

    public class WinterSolsticeSkill : BaseSkill, ISkill
    {
        public WinterSolsticeSkill(int level = 1) : base(level)
        {
            Name = "Winter Solstice";
            Description = "Add +1 Cold Damage to Attacks per Level for you and Allies within 5 Radius.";
            IconId = 2;
            Radius = 5;
            Duration = 60;
        }
        public override ICharacterState UpdateCharacterState(ICharacterState characterState)
        {
            base.UpdateCharacterState(characterState);
            characterState.ColdDamage += 1 * Level;
            return characterState;
        }
    }

    public class FrenzySkill : BaseSkill, ISkill
    {
        public FrenzySkill(int level = 1) : base(level)
        {
            ;
            Name = "Frenzy";
            Description = "Add +1 Physical Damage to Attacks per Level for you and Allies within 5 Radius.";
            IconId = 3;
            Radius = 5;
            Duration = 60;
        }
        public override ICharacterState UpdateCharacterState(ICharacterState characterState)
        {
            base.UpdateCharacterState(characterState);
            characterState.PhysicalDamage += 1 * Level;
            return characterState;
        }
    }


    public class FreezingWinterSkill : BaseSkill, ISkill
    {
        public FreezingWinterSkill(int level = 1) : base(level)
        {
            Name = "Freezing Winter";
            Description = "Adds 1% Chance to Freeze on Attack per Level for you and Allies within 5 Radius.";
            IconId = 4;
            Radius = 5;
            Duration = 30;
        }
        public override ICharacterState UpdateCharacterState(ICharacterState characterState)
        {
            base.UpdateCharacterState(characterState);
            characterState.FreezeChance += 1 * Level;
            return characterState;
        }
    }

    public class BurningSummerSkill : BaseSkill, ISkill
    {
        public BurningSummerSkill(int level = 1) : base(level)
        {
            Name = "Burning Summer";
            Description = "Adds 1% Chance to Burn on Attack per Level for you and Allies within 5 Radius.";
            IconId = 5;
            Radius = 5;
            Duration = 30;
        }
        public override ICharacterState UpdateCharacterState(ICharacterState characterState)
        {
            base.UpdateCharacterState(characterState);
            characterState.BurnChance += 1 * Level;
            return characterState;
        }
    }

    public class BleedingFurySkill : BaseSkill, ISkill
    {
        public BleedingFurySkill(int level = 1) : base(level)
        {
            Name = "Bleeding Fury";
            Description = "Adds 1% Chance to Bleed on Attack per Level for you and Allies within 5 Radius.";
            IconId = 6;
            Radius = 5;
            Duration = 30;
        }
        public override ICharacterState UpdateCharacterState(ICharacterState characterState)
        {
            base.UpdateCharacterState(characterState);
            characterState.BleedChance += 1 * Level;
            return characterState;
        }
    }

    public class WarmEmbraceSkill : BaseSkill, ISkill
    {
        public WarmEmbraceSkill(int level = 1) : base(level)
        {
            Name = "Warm Embrace";
            Description = "Increases Health Regeneration by 1% per Level for you and Allies within 5 Radius.";
            IconId = 7;
            Radius = 5;
            Duration = 30;
        }
        public override ICharacterState UpdateCharacterState(ICharacterState characterState)
        {
            base.UpdateCharacterState(characterState);
            characterState.IncreasedHealthRegen += 1 * Level;
            return characterState;
        }
    }

    public class ColdEmbraceSkill : BaseSkill, ISkill
    {
        public ColdEmbraceSkill(int level = 1) : base(level)
        {
            Name = "Cold Embrace";
            Description = "Increases Mana Regeneration by 1% per Level for you and Allies within 5 Radius.";
            IconId = 8;
            Radius = 5;
            Duration = 30;
        }
        public override ICharacterState UpdateCharacterState(ICharacterState characterState)
        {
            base.UpdateCharacterState(characterState);
            characterState.IncreasedManaRegen += 1 * Level;
            return characterState;
        }
    }

}
