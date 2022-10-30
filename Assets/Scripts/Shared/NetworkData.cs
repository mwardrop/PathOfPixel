using DarkRift;
using DarkriftSerializationExtensions;
using UnityEngine;

public enum NetworkTags
{
    LoginRequest = 0,
    LoginRequestAccepted = 1,
    LoginRequestDenied = 2,
    SpawnRequest = 3,
    SpawnPlayer = 4,
    MoveRequest = 5,
    MovePlayer = 6,
    PlayerDisconnect = 7,
    SpawnEnemy = 8,
    PlayerAttack = 9,
    EnemyAttack = 10,
    PlayerTakeDamage = 11,
    EnemyTakeDamage = 12
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

public struct PlayerStateData : IDarkRiftSerializable
{
    public PlayerState PlayerState;

    public PlayerStateData(PlayerState playerState)
    {
        PlayerState = playerState;
    }

    public void Deserialize(DeserializeEvent e)
    {
        PlayerState = e.Reader.ReadSerializable<PlayerState>();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(PlayerState);
    }
}

public struct EnemyStateData : IDarkRiftSerializable
{
    public EnemyState EnemyState;

    public EnemyStateData(EnemyState enemyState)
    {
        EnemyState = enemyState;
    }

    public void Deserialize(DeserializeEvent e)
    {
        EnemyState = e.Reader.ReadSerializable<EnemyState>();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(EnemyState);
    }
}

public struct TargetData : IDarkRiftSerializable
{
    public Vector2 Target;

    public TargetData(Vector2 target)
    {
        Target = target;
    }

    public void Deserialize(DeserializeEvent e)
    {
        Target = e.Reader.ReadVector2();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.WriteVector2(Target);
    }
}

public struct IntegerData: IDarkRiftSerializable
{
    public int Integer;

    public IntegerData(int integer)
    {
        Integer = integer;
    }

    public void Deserialize(DeserializeEvent e)
    {
        Integer = e.Reader.ReadInt32();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(Integer);
    }
}

public struct MovePlayerData : IDarkRiftSerializable
{
    public int ClientId;
    public Vector2 Target;

    public MovePlayerData(int clientId, Vector2 target)
    {
        ClientId = clientId;
        Target = target;
    }

    public void Deserialize(DeserializeEvent e)
    {
        ClientId = e.Reader.ReadInt32();
        Target = e.Reader.ReadVector2();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(ClientId);
        e.Writer.WriteVector2(Target);
    }
}

public struct GuidData : IDarkRiftSerializable
{
    public System.Guid Guid;

    public GuidData(System.Guid guid)
    {
        Guid = guid;
    }

    public void Deserialize(DeserializeEvent e)
    {
        string tempGuid = e.Reader.ReadString();
        Guid = new System.Guid(tempGuid);
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(Guid.ToString());
    }
}

public struct EnemyTakeDamageData : IDarkRiftSerializable
{
    public System.Guid EnemyGuid;
    public int Damage;

    public EnemyTakeDamageData(System.Guid enemyGuid, int damage)
    {
        EnemyGuid = enemyGuid;
        Damage = damage;
    }

    public void Deserialize(DeserializeEvent e)
    {
        string tempGuid = e.Reader.ReadString();
        EnemyGuid = new System.Guid(tempGuid);
        Damage = e.Reader.ReadInt32();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(EnemyGuid.ToString());
        e.Writer.Write(Damage);
    }
}

public struct PlayerTakeDamageData : IDarkRiftSerializable
{
    public int ClientId;
    public int Damage;

    public PlayerTakeDamageData(int clientID, int damage)
    {
        ClientId = clientID;
        Damage = damage;
    }

    public void Deserialize(DeserializeEvent e)
    {;
        ClientId = e.Reader.ReadInt32();
        Damage = e.Reader.ReadInt32();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(ClientId);
        e.Writer.Write(Damage);
    }
}