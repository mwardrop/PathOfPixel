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
    EnemyHitPlayer = 10,
    PlayerTakeDamage = 11,
    EnemyTakeDamage = 12,
    EnemyNewTarget = 13,
    UpdateEnemyLocation = 14,
    PlayerHitEnemy = 15,
    EnemyAttack = 16
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
    public float Health;
    public bool IsDead;

    public EnemyTakeDamageData(System.Guid enemyGuid, float health, bool isDead = false)
    {
        EnemyGuid = enemyGuid;
        Health = health;
        IsDead = isDead;
    }

    public void Deserialize(DeserializeEvent e)
    {
        string tempGuid = e.Reader.ReadString();
        EnemyGuid = new System.Guid(tempGuid);
        Health = e.Reader.ReadSingle();
        IsDead = e.Reader.ReadBoolean();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(EnemyGuid.ToString());
        e.Writer.Write(Health);
        e.Writer.Write(IsDead);
    }
}

public struct PlayerTakeDamageData : IDarkRiftSerializable
{
    public int ClientId;
    public float Health;
    public bool IsDead;

    public PlayerTakeDamageData(int clientId, float health, bool isDead = false)
    {
        ClientId = clientId;
        Health = health;
        IsDead = isDead;
    }

    public void Deserialize(DeserializeEvent e)
    {;
        ClientId = e.Reader.ReadInt32();
        Health = e.Reader.ReadSingle();
        IsDead = e.Reader.ReadBoolean();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(ClientId);
        e.Writer.Write(Health);
        e.Writer.Write(IsDead);
    }
}

public struct EnemyPlayerPairData : IDarkRiftSerializable
{
    public System.Guid EnemyGuid;
    public int ClientId;
    public string SceneName;

    public EnemyPlayerPairData(System.Guid enemyGuid, int clientId, string sceneName)
    {
        EnemyGuid = enemyGuid;
        ClientId = clientId;
        SceneName = sceneName;
    }

    public void Deserialize(DeserializeEvent e)
    {
        string tempGuid = e.Reader.ReadString();
        EnemyGuid = new System.Guid(tempGuid);
        ClientId = e.Reader.ReadInt32();
        SceneName = e.Reader.ReadString();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(EnemyGuid.ToString());
        e.Writer.Write(ClientId);
        e.Writer.Write(SceneName);
    }
}


public struct UpdateEnemyLocationData : IDarkRiftSerializable
{
    public System.Guid EnemyGuid;
    public Vector2 Location;
    public string SceneName;

    public UpdateEnemyLocationData(System.Guid enemyGuid, Vector2 location , string sceneName)
    {
        EnemyGuid = enemyGuid;
        Location = location;
        SceneName = sceneName;
    }

    public void Deserialize(DeserializeEvent e)
    {
        string tempGuid = e.Reader.ReadString();
        EnemyGuid = new System.Guid(tempGuid);
        Location = e.Reader.ReadVector2();
        SceneName = e.Reader.ReadString();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(EnemyGuid.ToString());
        e.Writer.WriteVector2(Location);
        e.Writer.Write(SceneName);
    }
}