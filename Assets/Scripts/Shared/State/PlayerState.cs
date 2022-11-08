using DarkRift;
using Data.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class PlayerState: CharacterState, ICharacterState, IDarkRiftSerializable
{
    public List<ItemState> Inventory;

    public string Scene { get; set; }
    public int ClientId { get; set; }
    public bool isTargetable { get; set; }
    public int AttackPoints { get; set; }
    public int SkillPoints { get; set; }
    public int PassivePoints { get; set; }
    public List<String> HotbarItems { get; set; }

    public PlayerState(): base()
    {
        Initialize();
    }

    public PlayerState(int clientId, string username, string scene, ICharacter character) : base(character)
    {
        Initialize();

        Name = username;
        Scene = scene;
        ClientId = clientId;

    }

    private void Initialize()
    {
        Inventory = new List<ItemState>();
        HotbarItems = new List<String>();
    }

    public override void Deserialize(DeserializeEvent e)
    {
        base.Deserialize(e);
        Scene = e.Reader.ReadString();
        ClientId = e.Reader.ReadInt32();
        ItemState[] tempInventory = e.Reader.ReadSerializables<ItemState>();
        Inventory = tempInventory.ToList();
        isTargetable = e.Reader.ReadBoolean();
        AttackPoints = e.Reader.ReadInt32();
        SkillPoints = e.Reader.ReadInt32();
        PassivePoints = e.Reader.ReadInt32();
        String[] tempHotbarItems = e.Reader.ReadStrings();
        HotbarItems = tempHotbarItems.ToList();
    }

    public override void Serialize(SerializeEvent e)
    {
        base.Serialize(e);
        e.Writer.Write(Scene);
        e.Writer.Write(ClientId);
        e.Writer.Write(Inventory.ToArray());
        e.Writer.Write(isTargetable);
        e.Writer.Write(AttackPoints);
        e.Writer.Write(SkillPoints);
        e.Writer.Write(PassivePoints);
        e.Writer.Write(HotbarItems.ToArray());
    }

}
