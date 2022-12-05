using DarkRift;
using DarkriftSerializationExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum CurrencyType
{
    StandardSmall = 0,
    StandardLarge = 1,
    PremiumSmall = 2,
    PremiumLarge = 3
}

public class CurrencyState : IDarkRiftSerializable
{
    public CurrencyType Type { get; set; }
    public int PlayerId { get; set; }
    public int Amount { get; set; }
    public Vector2 Location { get; set; }
    public Guid CurrencyGuid { get; set; }

    public CurrencyState()
    {
        CurrencyGuid = Guid.NewGuid();
    }

    public void Deserialize(DeserializeEvent e)
    {
        Type = (CurrencyType)e.Reader.ReadInt32();
        PlayerId = e.Reader.ReadInt32();
        Amount = e.Reader.ReadInt32();
        Location = e.Reader.ReadVector2();
        String tempGuid = e.Reader.ReadString();
        CurrencyGuid = Guid.Parse(tempGuid);
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write((int)Type);
        e.Writer.Write(PlayerId);
        e.Writer.Write(Amount);
        e.Writer.WriteVector2(Location);
        e.Writer.Write(CurrencyGuid.ToString());
    }
}

