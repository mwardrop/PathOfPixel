using DarkRift;
using DarkriftSerializationExtensions;
using System;
using UnityEngine;

public enum EnemyType
{
    Possessed
}

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
    public EnemyType Type;
    public EnemyRarity Rarity;
    public Guid EnemyGuid;
    public int TargetPlayerId;
    public Vector2 HomeLocation;

    public EnemyState()
    {
        EnemyGuid = Guid.NewGuid();
    }

    public override void Deserialize(DeserializeEvent e)
    {
        base.Deserialize(e);
        Type = (EnemyType)e.Reader.ReadInt32();
        Rarity = (EnemyRarity)e.Reader.ReadInt32();
        String tempGuid = e.Reader.ReadString();
        EnemyGuid = Guid.Parse(tempGuid);
        TargetPlayerId = e.Reader.ReadInt32();
        HomeLocation = e.Reader.ReadVector2();
    }

    public override void Serialize(SerializeEvent e)
    {
        base.Serialize(e);
        e.Writer.Write((int)Type);
        e.Writer.Write((int)Rarity);
        e.Writer.Write(EnemyGuid.ToString());
        e.Writer.Write(TargetPlayerId);
        e.Writer.WriteVector2(HomeLocation);
    }

}
