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
    }

    public class AggressionPassive : BasePassive, IPassive
    {
        public AggressionPassive(int level = 1) : base(level)
        {
            Name = "Aggression";
            Description = "Increases Physical Damage by 1% per Level.";
            IconId = 1;
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
    }

    public class HypothermiaPassive : BasePassive, IPassive
    {
        public HypothermiaPassive(int level = 1) : base(level)
        {
            Name = "Hypothermia";
            Description = "Increases Cold Damage by 1% per Level.";
            IconId = 3;
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
    }

    public class IntelligencePassive : BasePassive, IPassive
    {
        public IntelligencePassive(int level = 1) : base(level)
        {
            Name = "Intelligence";
            Description = "Increases Maximum Mana and Mana Regeneration by 1% per Level.";
            IconId = 5;
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
    }

}
