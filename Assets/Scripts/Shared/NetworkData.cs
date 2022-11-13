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
    EnemyAttack = 16,
    UpdatePlayerState = 17,
    CalculatePlayerState = 18,
    SetPlayerActiveAttack = 19,
    SpendAttackPoint = 20,
    SpendSkillPoint = 21,
    SpendPassivePoint = 22,
    SetPlayerDirection = 23,
    SetPlayerHotbarItem = 24,
    ActivatePlayerSkill = 25,
    DeactivatePlayerSkill = 26,
    ActivateEnemySkill = 27,
    DeactivateEnemySkill = 28,
    UpdatePlayerLocation = 29,
    UpdatePlayerExperience = 30,
    UpdatePlayerRegen = 31,
    UpdateEnemyRegen = 32
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
    public string Scene;

    public EnemyStateData(EnemyState enemyState, string scene)
    {
        EnemyState = enemyState;
        Scene = scene;
    }

    public void Deserialize(DeserializeEvent e)
    {
        EnemyState = e.Reader.ReadSerializable<EnemyState>();
        Scene = e.Reader.ReadString();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(EnemyState);
        e.Writer.Write(Scene);
    }
}

public struct Vector2Data : IDarkRiftSerializable
{
    public Vector2 Target;

    public Vector2Data(Vector2 target)
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

public struct StringData : IDarkRiftSerializable
{
    public string String;

    public StringData(string _string)
    {
        String = _string;
    }

    public void Deserialize(DeserializeEvent e)
    {
        String = e.Reader.ReadString();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(String);
    }
}

public struct StringIntegerData : IDarkRiftSerializable
{
    public string String;
    public int Integer;

    public StringIntegerData(string _string, int integer)
    {
        String = _string;
        Integer = integer;
    }

    public void Deserialize(DeserializeEvent e)
    {
        String = e.Reader.ReadString();
        Integer = e.Reader.ReadInt32();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(String);
        e.Writer.Write(Integer);
    }
}

public struct IntegerPairData : IDarkRiftSerializable
{
    public int Integer1;
    public int Integer2;

    public IntegerPairData(int integer1, int integer2)
    {
        Integer1 = integer1;
        Integer2 = integer2;
    }

    public void Deserialize(DeserializeEvent e)
    {
        Integer1 = e.Reader.ReadInt32();
        Integer2 = e.Reader.ReadInt32();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(Integer1);
        e.Writer.Write(Integer2);
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

public struct GuidIntegerData : IDarkRiftSerializable
{
    public System.Guid Guid;
    public int Integer;

    public GuidIntegerData(System.Guid guid, int integer)
    {
        Guid = guid;
        Integer = integer;
    }

    public void Deserialize(DeserializeEvent e)
    {
        string tempGuid = e.Reader.ReadString();
        Guid = new System.Guid(tempGuid);
        Integer = e.Reader.ReadInt32();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(Guid.ToString());
        e.Writer.Write(Integer);
    }
}

public struct EnemyTakeDamageData : IDarkRiftSerializable
{
    public System.Guid EnemyGuid;
    public float Health;
    public bool IsDead;
    public string Scene;

    public EnemyTakeDamageData(System.Guid enemyGuid, float health, string scene, bool isDead = false)
    {
        EnemyGuid = enemyGuid;
        Health = health;
        IsDead = isDead;
        Scene = scene;
    }

    public void Deserialize(DeserializeEvent e)
    {
        string tempGuid = e.Reader.ReadString();
        EnemyGuid = new System.Guid(tempGuid);
        Health = e.Reader.ReadSingle();
        IsDead = e.Reader.ReadBoolean();
        Scene = e.Reader.ReadString();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(EnemyGuid.ToString());
        e.Writer.Write(Health);
        e.Writer.Write(IsDead);
        e.Writer.Write(Scene);
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

public struct UpdatePlayerLocationData : IDarkRiftSerializable
{
    public int ClientId;
    public Vector2 Location;

    public UpdatePlayerLocationData(int clientId, Vector2 location)
    {
        ClientId = clientId;
        Location = location;

    }

    public void Deserialize(DeserializeEvent e)
    {
        ClientId = e.Reader.ReadInt32();
        Location = e.Reader.ReadVector2();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(ClientId);
        e.Writer.WriteVector2(Location);

    }
}

public struct KeyValueStateData : IDarkRiftSerializable
{
    public KeyValueState KeyValueState;

    public KeyValueStateData(KeyValueState keyValueState)
    {
        KeyValueState = keyValueState;
    }

    public void Deserialize(DeserializeEvent e)
    {
        KeyValueState = e.Reader.ReadSerializable<KeyValueState>();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(KeyValueState);
    }
}

public struct SkillActivationData : IDarkRiftSerializable
{
    public int ActivatingCharacter;
    public int Receivingharacter;
    public string Name;
    public int Level;
    public string Scene;

    public SkillActivationData(int activatingCharacter, int receivingCharacter, string name, int level, string scene)
    {
        ActivatingCharacter = activatingCharacter;
        Receivingharacter = receivingCharacter;
        Name = name;
        Level = level;
        Scene = scene;
    }

    public void Deserialize(DeserializeEvent e)
    {
        ActivatingCharacter = e.Reader.ReadInt32();
        Receivingharacter = e.Reader.ReadInt32();
        Name = e.Reader.ReadString();
        Level = e.Reader.ReadInt32();
        Scene = e.Reader.ReadString();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(ActivatingCharacter);
        e.Writer.Write(Receivingharacter);
        e.Writer.Write(Name);
        e.Writer.Write(Level);
        e.Writer.Write(Scene);
    }
}

public struct EnemyRegenData : IDarkRiftSerializable
{
    public System.Guid Guid;
    public float Health;
    public float Mana;
    public string Scene;

    public EnemyRegenData(System.Guid guid, float health, float mana, string scene)
    {
        Guid = guid;
        Health = health;
        Mana = mana;
        Scene = scene;
    }

    public void Deserialize(DeserializeEvent e)
    {
        string tempGuid = e.Reader.ReadString();
        Guid = new System.Guid(tempGuid);
        Health = e.Reader.ReadSingle();
        Mana = e.Reader.ReadSingle();
        Scene = e.Reader.ReadString();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(Guid.ToString());
        e.Writer.Write(Health);
        e.Writer.Write(Mana);
        e.Writer.Write(Scene);
    }
}

public struct PlayerRegenData : IDarkRiftSerializable
{
    public int Id;
    public float Health;
    public float Mana;

    public PlayerRegenData(int id, float health, float mana)
    {
        Id = id;
        Health = health;
        Mana = mana;
    }

    public void Deserialize(DeserializeEvent e)
    {
        Id = e.Reader.ReadInt32();
        Health = e.Reader.ReadSingle();
        Mana = e.Reader.ReadSingle();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(Id);
        e.Writer.Write(Health);
        e.Writer.Write(Mana);
    }
}