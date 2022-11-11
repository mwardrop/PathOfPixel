using DarkRift;
using DarkriftSerializationExtensions;
using Data.Characters;
using Data.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;


public enum EnemyRarity
{
    Common,
    Magic,
    Rare,
    Legendary,
    Mythic,
    Boss
}

[Serializable]
public class EnemyState : CharacterState, ICharacterState, IDarkRiftSerializable
{
    public EnemyRarity Rarity { get; set; }
    public Guid EnemyGuid { get; set; }
    public int TargetPlayerId { get; set; }
    public Vector2 HomeLocation { get; set; }
    public List<KeyValueState> DamageTracker {get; set;}

    public EnemyState():base()
    {
        Initialize();
    }

    public EnemyState(string name, Vector2 location, ICharacter character) : base(character)
    {
        Initialize();

        Name = name;
        Location = new Vector2(location.x, location.y);
        HomeLocation = new Vector2(location.x, location.y);
    }

    private void Initialize()
    {
        EnemyGuid = Guid.NewGuid();
        TargetPlayerId = -1;
        DamageTracker = new List<KeyValueState>();
    }

    public override void Deserialize(DeserializeEvent e)
    {
        base.Deserialize(e);
        Rarity = (EnemyRarity)e.Reader.ReadInt32();
        String tempGuid = e.Reader.ReadString();
        EnemyGuid = Guid.Parse(tempGuid);
        TargetPlayerId = e.Reader.ReadInt32();
        HomeLocation = e.Reader.ReadVector2();
    }

    public override void Serialize(SerializeEvent e)
    {
        base.Serialize(e);
        e.Writer.Write((int)Rarity);
        e.Writer.Write(EnemyGuid.ToString());
        e.Writer.Write(TargetPlayerId);
        e.Writer.WriteVector2(HomeLocation);
    }

}
