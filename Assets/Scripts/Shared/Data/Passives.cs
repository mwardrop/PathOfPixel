namespace Data
{
    public interface IPassive
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class BasePassive : IPassive
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class AggressionPassive : BasePassive, IPassive
    {
        public AggressionPassive()
        {
            Id = 1;
            Name = "Aggression";
            Description = "Increases Physical Damage by 1% per Level.";
        }
    }

    public class ImmolationPassive : BasePassive, IPassive
    {
        public ImmolationPassive()
        {
            Id = 2;
            Name = "Immolation";
            Description = "Increases Fire Damage by 1% per Level.";
        }
    }

    public class HypothermiaPassive : BasePassive, IPassive
    {
        public HypothermiaPassive()
        {
            Id = 3;
            Name = "Hypothermia";
            Description = "Increases Cold Damage by 1% per Level.";
        }
    }

    public class StaminaPassive : BasePassive, IPassive
    {
        public StaminaPassive()
        {
            Id = 4;
            Name = "Stamina";
            Description = "Increases Dodge Chance and Movement Speed by 1% per Level.";
        }
    }

    public class IntelligencePassive : BasePassive, IPassive
    {
        public IntelligencePassive()
        {
            Id = 5;
            Name = "Intelligence";
            Description = "Increases Maximum Mana and Mana Regeneration by 1% per Level.";
        }
    }

    public class StrengthPassive : BasePassive, IPassive
    {
        public StrengthPassive()
        {
            Id = 6;
            Name = "Strength";
            Description = "Increases Maximum Health and Health Regeneration by 1% per Level.";
        }
    }

}
