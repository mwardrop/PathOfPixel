using System.Linq;

namespace Data.Passives
{
    public interface IPassive
    {
        public int Level { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int IconId { get; set; }

        public string GetName();
        public ICharacterState UpdateCharacterState(ICharacterState characterState);
    }

    public class BasePassive : IPassive
    {
        public int Level { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int IconId { get; set; }

        public string GetName()
        {
            return this.GetType().ToString().Split(".").Last();
        }

        public BasePassive(int level)
        {
            Level = level;
        }

        public virtual ICharacterState UpdateCharacterState(ICharacterState characterState) { return characterState; }

    }

    public class AggressionPassive : BasePassive, IPassive
    {
        public AggressionPassive(int level = 1) : base(level)
        {
            Name = "Aggression";
            Description = "Increases Physical Damage by 1% per Level.";
            IconId = 1;
        }
        public override ICharacterState UpdateCharacterState(ICharacterState characterState)
        {
            characterState.IncreasedPhysicalDamage += 1 * Level;
            return characterState;
        }
    }

    public class ImmolationPassive : BasePassive, IPassive
    {
        public ImmolationPassive(int level = 1) : base(level)
        {
            Name = "Immolation";
            Description = "Increases Fire Damage by 1% per Level.";
            IconId = 2;
        }
        public override ICharacterState UpdateCharacterState(ICharacterState characterState)
        {
            characterState.IncreasedFireDamage += 1 * Level;
            return characterState;
        }
    }

    public class HypothermiaPassive : BasePassive, IPassive
    {
        public HypothermiaPassive(int level = 1) : base(level)
        {
            Name = "Hypothermia";
            Description = "Increases Cold Damage by 1% per Level.";
            IconId = 3;
        }
        public override ICharacterState UpdateCharacterState(ICharacterState characterState)
        {
            characterState.IncreasedColdDamage += 1 * Level;
            return characterState;
        }
    }

    public class StaminaPassive : BasePassive, IPassive
    {
        public StaminaPassive(int level = 1) : base(level)
        {
            Name = "Stamina";
            Description = "Increases Dodge Chance and Movement Speed by 1% per Level.";
            IconId = 4;
        }
        public override ICharacterState UpdateCharacterState(ICharacterState characterState)
        {
            characterState.IncreasedDodge += 1 * Level;
            characterState.IncreasedMoveSpeed += 1 * Level;
            return characterState;
        }
    }

    public class IntelligencePassive : BasePassive, IPassive
    {
        public IntelligencePassive(int level = 1) : base(level)
        {
            Name = "Intelligence";
            Description = "Increases Maximum Mana and Mana Regeneration by 1% per Level.";
            IconId = 5;
        }
        public override ICharacterState UpdateCharacterState(ICharacterState characterState)
        {
            characterState.IncreasedMaxMana += 1 * Level;
            characterState.IncreasedManaRegen += 1 * Level;
            return characterState;
        }
    }

    public class StrengthPassive : BasePassive, IPassive
    {
        public StrengthPassive(int level = 1) : base(level)
        {
            Name = "Strength";
            Description = "Increases Maximum Health and Health Regeneration by 1% per Level.";
            IconId = 6;
        }
        public override ICharacterState UpdateCharacterState(ICharacterState characterState)
        {
            characterState.IncreasedMaxHealth += 1 * Level;
            characterState.IncreasedHealthRegen += 1 * Level;
            return characterState;
        }
    }

    public class HealthPassive : BasePassive, IPassive
    {
        public HealthPassive(int level = 1) : base(level)
        {
            Name = "Health";
            Description = "Adds +5 Health per Level.";
            IconId = 7;
        }
        public override ICharacterState UpdateCharacterState(ICharacterState characterState)
        {
            characterState.MaxHealth += 5 * Level;
            return characterState;
        }
    }

    public class ManaPassive : BasePassive, IPassive
    {
        public ManaPassive(int level = 1) : base(level)
        {
            Name = "Mana";
            Description = "Adds +5 Mana per Level.";
            IconId = 8;
        }
        public override ICharacterState UpdateCharacterState(ICharacterState characterState)
        {
            characterState.MaxMana += 5 * Level;
            return characterState;
        }
    }

}
