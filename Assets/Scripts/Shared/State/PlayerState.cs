using DarkRift;
using DarkRift.Server;
using Data.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor.PackageManager;

[Serializable]
public class PlayerState: CharacterState, ICharacterState, IDarkRiftSerializable
{
    public List<ItemState> Inventory;

    public string Scene;
    public int ClientId;
    public bool isTargetable;

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
    }

    public override void Deserialize(DeserializeEvent e)
    {
        base.Deserialize(e);
        Scene = e.Reader.ReadString();
        ClientId = e.Reader.ReadInt32();
        ItemState[] tempInventory = e.Reader.ReadSerializables<ItemState>();
        Inventory = tempInventory.ToList();
        isTargetable = e.Reader.ReadBoolean();

    }

    public override void Serialize(SerializeEvent e)
    {
        base.Serialize(e);
        e.Writer.Write(Scene);
        e.Writer.Write(ClientId);
        e.Writer.Write(Inventory.ToArray());
        e.Writer.Write(isTargetable);
    }

}
