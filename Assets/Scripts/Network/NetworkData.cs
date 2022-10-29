using DarkRift;
using DarkriftSerializationExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NetworkTags
{
    LoginRequest = 0,
    LoginRequestAccepted = 1,
    LoginRequestDenied = 2,
    SpawnRequest = 3,
    SpawnPlayer = 4,
    MoveRequest = 5,
    MoveRequestAccepted = 6
}

public struct LoginRequestData : IDarkRiftSerializable
{
    public string Username;
    public string Password;

    public LoginRequestData(string username, string password)
    {
        Username = username;
        Password = password;
    }

    public void Deserialize(DeserializeEvent e)
    {
        Username = e.Reader.ReadString();
        Password = e.Reader.ReadString();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(Username);
        e.Writer.Write(Password);
    }
}

public struct LoginResponseData : IDarkRiftSerializable
{
    public ushort Id;
    public WorldState State;

    public LoginResponseData(ushort id, WorldState state)
    {
        Id = id;
        State = state;
    }

    public void Deserialize(DeserializeEvent e)
    {
        Id = e.Reader.ReadUInt16();
        State = e.Reader.ReadSerializable<WorldState>();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(Id);
        e.Writer.Write(State);
    }
}

public struct SpawnPlayerData : IDarkRiftSerializable
{
    public string Scene;
    public string Username;
    public Vector2 Target;


    public SpawnPlayerData(string scene, string username, Vector2 target)
    {
        Scene = scene;
        Username = username;
        Target = target;
    }

    public void Deserialize(DeserializeEvent e)
    {
        Scene = e.Reader.ReadString();
        Username = e.Reader.ReadString();
        Target = e.Reader.ReadVector2();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(Scene);
        e.Writer.Write(Username);
        e.Writer.WriteVector2(Target);
    }
}