using DarkRift;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NetworkTags
{
    LoginRequest = 0,
    LoginRequestAccepted = 1,
    LoginRequestDenied = 2,
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

public struct LoginInfoData : IDarkRiftSerializable
{
    public ushort Id;
    public WorldState State;

    public LoginInfoData(ushort id, WorldState state)
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