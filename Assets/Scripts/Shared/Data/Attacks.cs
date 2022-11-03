using System.Linq;

namespace Data.Attacks
{
    public interface IAttack
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int AnimationId { get; set; }
        public float PhysicalDamage { get; set; }
        public float FireDamage { get; set; }
        public float ColdDamage { get; set; }
        public bool Bleed { get; set; }
        public bool Freeze { get; set; }
        public bool Burn { get; set; }
        public float Knockback { get; set; }
        public int Cooldown { get; set; }
        public int ManaCost { get; set; }

        public IAttack ApplyCharacterState(CharacterState character);

    }

    public class BaseAttack : IAttack
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int AnimationId { get; set; }
        public float PhysicalDamage { get; set; }
        public float FireDamage { get; set; }
        public float ColdDamage { get; set; }
        public bool Bleed { get; set; }
        public bool Freeze { get; set; }
        public bool Burn { get; set; }
        public float Knockback { get; set; }
        public int Cooldown { get; set; }
        public int ManaCost { get; set; }

        public virtual IAttack ApplyCharacterState(CharacterState character) { return this; }

        public int GetAttackLevel(CharacterState character)
        {
            return character.Attacks.First(x => x.ObjectId == Id).Level;
        }
    }

    public class SweepAttack: BaseAttack, IAttack
    {
    
        public SweepAttack()
        {
            Id = 1;
            Name = "Sweep";
            Description = "Sweep your weapon across enemies.";
            AnimationId = 1;
            PhysicalDamage = 5;
            FireDamage = 0;
            ColdDamage = 0;
            Bleed = false;
            Freeze = false;
            Burn = false;
            Knockback = 0;
            Cooldown = 0;
            ManaCost = 1;
        }

        public override IAttack ApplyCharacterState(CharacterState character)
        {
            return new SweepAttack()
            {
                PhysicalDamage = PhysicalDamage + character.Level + (GetAttackLevel(character) * 5)
            };
        }
    }

    public class SlamAttack : BaseAttack, IAttack
    { 

        public SlamAttack()
        {
            Id = 2;
            Name = "Slam";
            Description = "Slam your weapon into the ground, knocking back enemies.";
            AnimationId = 2;
            PhysicalDamage = 2;
            FireDamage = 0;
            ColdDamage = 0;
            Bleed = false;
            Freeze = false;
            Burn = false;
            Knockback = 5;
            Cooldown = 5;
            ManaCost = 5;
        }

        public override IAttack ApplyCharacterState(CharacterState character)
        {
            return new SweepAttack()
            {
                PhysicalDamage = PhysicalDamage + character.Level + (GetAttackLevel(character) * 1)
            };
        }
    }
}
