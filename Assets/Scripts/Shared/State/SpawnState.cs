using DarkRift;
using DarkriftSerializationExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SpawnState : IDarkRiftSerializable
{
    public string Name;
    public Vector2 MinBounds;
    public Vector2 MaxBounds;

    public SpawnState()
    {
        MinBounds = new Vector2();
        MaxBounds = new Vector2();
    }

    public SpawnState(string name, Vector2 startBounds, Vector2 endBounds)
    {
        Name = name;
        MinBounds = startBounds;
        MaxBounds = endBounds;
    }

    public virtual void Deserialize(DeserializeEvent e)
    {
        Name = e.Reader.ReadString();
        MinBounds = e.Reader.ReadVector2();
        MaxBounds = e.Reader.ReadVector2();
    }

    public virtual void Serialize(SerializeEvent e)
    {
        e.Writer.Write(Name);
        e.Writer.WriteVector2(MinBounds);
        e.Writer.WriteVector2(MaxBounds);
    }
}


