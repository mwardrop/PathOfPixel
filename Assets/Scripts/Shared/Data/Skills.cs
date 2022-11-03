

namespace Data
{
    public interface ISkill
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class BaseSkill: ISkill
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    } 

    public class SummerSolsticeSkill: BaseSkill, ISkill
    {
        public SummerSolsticeSkill()
        {
            Id = 1;
            Name = "Summer Solstice";
            Description = "Add +1 Fire Damage to Attacks per Level.";
        }
    }

    public class WinterSolsticeSkill : BaseSkill, ISkill
    {
        public WinterSolsticeSkill()
        {
            Id = 2;
            Name = "Winter Solstice";
            Description = "Add +1 Cold Damage to Attacks per Level.";
        }
    }

    public class FrenzySkill : BaseSkill, ISkill
    {
        public FrenzySkill()
        {
            Id = 3;
            Name = "Frenzy";
            Description = "Add +1 Physical Damage to Attacks per Level.";
        }
    }


    public class FreezingWinterSkill : BaseSkill, ISkill
    {
        public FreezingWinterSkill()
        {
            Id = 4;
            Name = "Freezing Winter";
            Description = "1% Chance to Freeze on Attack per Level.";
        }
    }

    public class BurningSummerSkill : BaseSkill, ISkill
    {
        public BurningSummerSkill()
        {
            Id = 5;
            Name = "Burning Summer";
            Description = "1% Chance to Burn on Attack per Level.";
        }
    }

    public class BleedingFurySkill : BaseSkill, ISkill
    {
        public BleedingFurySkill()
        {
            Id = 6;
            Name = "Bleeding Fury";
            Description = "1% Chance to Bleed on Attack per Level.";
        }
    }

}
