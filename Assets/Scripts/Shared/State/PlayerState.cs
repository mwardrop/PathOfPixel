using DarkRift;
using Data.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class PlayerState: CharacterState, ICharacterState, IDarkRiftSerializable
{
    public InventoryState Inventory;

    public string Scene { get; set; }
    public int ClientId { get; set; }
    public bool isTargetable { get; set; }
    public int AttackPoints { get; set; }
    public int SkillPoints { get; set; }
    public int PassivePoints { get; set; }
    public List<KeyValueState> HotbarItems { get; set; }

    public PlayerState(): base()
    {
        Initialize();
    }

    public PlayerState(int clientId, string username, string scene, ICharacter character, Vector2 location) : base(character)
    {
        Initialize();

        Name = username;
        Scene = scene;
        ClientId = clientId;
        Location = Location;

    }

    private void Initialize()
    {
        Inventory = new InventoryState();
        HotbarItems = new List<KeyValueState>();
    }

    public override void Deserialize(DeserializeEvent e)
    {
        base.Deserialize(e);
        Scene = e.Reader.ReadString();
        ClientId = e.Reader.ReadInt32();
        Inventory = e.Reader.ReadSerializable<InventoryState>();
        isTargetable = e.Reader.ReadBoolean();
        AttackPoints = e.Reader.ReadInt32();
        SkillPoints = e.Reader.ReadInt32();
        PassivePoints = e.Reader.ReadInt32();
        KeyValueState[] tempHotbarItems = e.Reader.ReadSerializables<KeyValueState>();
        HotbarItems = tempHotbarItems.ToList();
    }

    public override void Serialize(SerializeEvent e)
    {
        base.Serialize(e);
        e.Writer.Write(Scene);
        e.Writer.Write(ClientId);
        e.Writer.Write(Inventory);
        e.Writer.Write(isTargetable);
        e.Writer.Write(AttackPoints);
        e.Writer.Write(SkillPoints);
        e.Writer.Write(PassivePoints);
        e.Writer.Write(HotbarItems.ToArray());
    }

}
