

using System.Linq;

namespace Data
{
    public interface ISkill
    {
        public int Level { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int IconId { get; set; }

        public string GetName();
    }

    public class BaseSkill: ISkill
    {
        public int Level { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int IconId { get; set; }

        public string GetName()
        {
            return this.GetType().ToString().Split(".").Last();
        }

        public BaseSkill(int level)
        {
            Level = level;
        }
    } 

    public class SummerSolsticeSkill: BaseSkill, ISkill
    {
        public SummerSolsticeSkill(int level = 1) : base(level)
        {
            Name = "Summer Solstice";
            Description = "Add +1 Fire Damage to Attacks per Level.";
        }
    }

    public class WinterSolsticeSkill : BaseSkill, ISkill
    {
        public WinterSolsticeSkill(int level = 1) : base(level)
        {
            Name = "Winter Solstice";
            Description = "Add +1 Cold Damage to Attacks per Level.";
        }
    }

    public class FrenzySkill : BaseSkill, ISkill
    {
        public FrenzySkill(int level = 1) : base(level)
        {
            ;
            Name = "Frenzy";
            Description = "Add +1 Physical Damage to Attacks per Level.";
        }
    }


    public class FreezingWinterSkill : BaseSkill, ISkill
    {
        public FreezingWinterSkill(int level = 1) : base(level)
        {
            Name = "Freezing Winter";
            Description = "1% Chance to Freeze on Attack per Level.";
        }
    }

    public class BurningSummerSkill : BaseSkill, ISkill
    {
        public BurningSummerSkill(int level = 1) : base(level)
        {
            Name = "Burning Summer";
            Description = "1% Chance to Burn on Attack per Level.";
        }
    }

    public class BleedingFurySkill : BaseSkill, ISkill
    {
        public BleedingFurySkill(int level = 1) : base(level)
        {
            Name = "Bleeding Fury";
            Description = "1% Chance to Bleed on Attack per Level.";
        }
    }

}
