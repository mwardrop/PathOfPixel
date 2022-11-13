using System.Linq;

namespace Data.Attacks
{
    public interface IAttack
    {
        public int Level { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int AnimationId { get; set; }
        public int IconId { get; set; }
        public float PhysicalDamage { get; set; }
        public float FireDamage { get; set; }
        public float ColdDamage { get; set; }
        public bool Bleed { get; set; }
        public bool Freeze { get; set; }
        public bool Burn { get; set; }
        public float Knockback { get; set; }
        public int Cooldown { get; set; }
        public int ManaCost { get; set; }

        public string GetName();
    }

    public class BaseAttack : IAttack
    {
        public int Level { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int AnimationId { get; set; }
        public int IconId { get; set; }
        public float PhysicalDamage { get; set; }
        public float FireDamage { get; set; }
        public float ColdDamage { get; set; }
        public bool Bleed { get; set; }
        public bool Freeze { get; set; }
        public bool Burn { get; set; }
        public float Knockback { get; set; }
        public int Cooldown { get; set; }
        public int ManaCost { get; set; }

        public string GetName()
        {
            return this.GetType().ToString().Split(".").Last();
        }

        public BaseAttack(int level)
        {
            Level = level;
        }
    }

    public class SweepAttack: BaseAttack, IAttack
    {
    
        public SweepAttack(int level = 1) : base(level)
        {
            Name = "Sweep";
            Description = "Sweep your weapon across enemies.";
            AnimationId = 1;
            IconId = 1;
            PhysicalDamage = 5 * level;
            FireDamage = 0;
            ColdDamage = 0;
            Bleed = false;
            Freeze = false;
            Burn = false;
            Knockback = 0;
            Cooldown = 0;
            ManaCost = 1;
        }
    }

    public class SlamAttack : BaseAttack, IAttack
    { 

        public SlamAttack(int level = 1) : base(level)
        {
            Name = "Slam";
            Description = "Slam your weapon into the ground, knocking back enemies.";
            AnimationId = 2;
            IconId = 1;
            PhysicalDamage = 2 * level;
            FireDamage = 0;
            ColdDamage = 0;
            Bleed = false;
            Freeze = false;
            Burn = false;
            Knockback = 5;
            Cooldown = 5;
            ManaCost = 5;
            IconId = 2;
        }
    }
}
