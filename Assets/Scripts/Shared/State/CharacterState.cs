using DarkRift;
using DarkriftSerializationExtensions;
using UnityEngine;

public class CharacterState: ICharacterState, IDarkRiftSerializable
{
    public string Name { get; set; }
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
    public int Level { get; set; }
    public int Experience { get; set; }
    public Vector2 Location { get; set; }
    public Vector2 TargetLocation { get; set; }
    public float MoveSpeed { get; set; }
    public bool IsDead { get; set; }

    public float IncomingPhysicalDamage { get; set; }
    public float IncomingFireDamage { get; set; }
    public float IncomingColdDamage { get; set; }

    // TODO: MOVE TO STATEMANAGER
    //public DamageTransfer TakeDamage(DamageTransfer damage)
    //{
    //    if (damage.physicalDamage > 0)
    //    {
    //        damage.physicalDamage -= ((Armor / 1000) / 100 * damage.physicalDamage);
    //    }
    //    if (damage.fireDamage > 0)
    //    {
    //        damage.fireDamage -= ((FireResistance / 100) * damage.fireDamage);
    //    }
    //    if (damage.coldDamage > 0)
    //    {
    //        damage.coldDamage -= ((ColdResistance / 100) * damage.coldDamage);
    //    }
    //    if (damage.knockback > 0)
    //    {
    //        damage.knockback -= ((Armor / 1000) / 100 * damage.knockback);
    //    }

    //    IncomingDamage += (damage.physicalDamage + damage.fireDamage + damage.coldDamage);

    //    return damage;
    //}

    //// TODO: MOVE TO STATEMANAGER
    //public float ApplyIncomingDamage()
    //{
    //    if (IncomingDamage > 0)
    //    {
    //        float _incomingDamage = IncomingDamage;
    //        Health -= IncomingDamage;
    //        IncomingDamage = 0;
    //        return _incomingDamage;
    //    }
    //    return IncomingDamage;
    //}

    public virtual void Deserialize(DeserializeEvent e)
    {
        Name = e.Reader.ReadString();
        Health = e.Reader.ReadSingle();
        HealthRegen = e.Reader.ReadSingle();
        Mana = e.Reader.ReadSingle();
        ManaRegen = e.Reader.ReadSingle();
        PhysicalDamage = e.Reader.ReadSingle();
        FireDamage = e.Reader.ReadSingle();
        ColdDamage = e.Reader.ReadSingle();
        FireResistance = e.Reader.ReadSingle();
        ColdResistance = e.Reader.ReadSingle();
        Armor = e.Reader.ReadSingle();
        Dodge = e.Reader.ReadSingle();
        Level = e.Reader.ReadInt32();
        Experience = e.Reader.ReadInt32();
        Location = e.Reader.ReadVector2();
        TargetLocation = e.Reader.ReadVector2();
        MoveSpeed = e.Reader.ReadSingle();
        IsDead = e.Reader.ReadBoolean();
    }

    public virtual void Serialize(SerializeEvent e)
    {
        e.Writer.Write(Name);
        e.Writer.Write(Health);
        e.Writer.Write(HealthRegen);
        e.Writer.Write(Mana);
        e.Writer.Write(ManaRegen);
        e.Writer.Write(PhysicalDamage);
        e.Writer.Write(FireDamage);
        e.Writer.Write(ColdDamage);
        e.Writer.Write(FireResistance);
        e.Writer.Write(ColdResistance);
        e.Writer.Write(Armor);
        e.Writer.Write(Dodge);
        e.Writer.Write(Level);
        e.Writer.Write(Experience);
        e.Writer.WriteVector2(Location);
        e.Writer.WriteVector2(TargetLocation);
        e.Writer.Write(MoveSpeed);
        e.Writer.Write(IsDead);
    }
}

public interface ICharacterState
{
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
    public int Level { get; set; }
    public int Experience { get; set; }
    public Vector2 Location { get; set; }
    public Vector2 TargetLocation { get; set; }
    public float MoveSpeed { get; set; }
    public bool IsDead { get; set; }

    public float IncomingPhysicalDamage { get; set; }
    public float IncomingFireDamage { get; set; }
    public float IncomingColdDamage { get; set; }
    //public DamageTransfer TakeDamage(DamageTransfer damage);
    //public float ApplyIncomingDamage();
}

//public class DamageTransfer
//{
//    public float physicalDamage = 0;
//    public float fireDamage = 0;
//    public float coldDamage = 0;
//    public float knockback = 0;
//}