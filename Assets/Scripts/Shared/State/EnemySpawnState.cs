using DarkRift;
using Data.Characters;
using System;
using UnityEngine;

[Serializable]
public class EnemySpawnState : SpawnState, IDarkRiftSerializable
{
    public EnemyCharacters Character;
    public int Minimum;
    public int Maximum;
    public CharacterRarity CharacterRarity;
    public string CharacterName;
    public int MinLevel;
    public int MaxLevel;
    public bool ActivateSkills { get; set; }
    public float ChanceOfChest { get; set; }

    public EnemySpawnState():base()
    {
        Character = EnemyCharacters.Possessed;
        Minimum = 0;
        Maximum = 0;
        CharacterRarity = CharacterRarity.Common;
        CharacterName = Character.ToString();
        MinLevel = 1;
        MaxLevel = 1;
        ChanceOfChest = 0.0f;
    }

    public EnemySpawnState
        (
            string name, 
            Vector2 startBounds, 
            Vector2 endBounds, 
            EnemyCharacters character, 
            int minimum, 
            int maximum, 
            CharacterRarity rarity,
            string characterName,
            int minLevel,
            int maxLevel,
            bool activateSkills,
            float chanceOfChest
        ) :base(name, startBounds, endBounds)
    {
        Character = character;
        Minimum = minimum;
        Maximum = maximum;
        CharacterRarity = rarity;
        CharacterName = characterName;
        MinLevel = minLevel;
        MaxLevel = maxLevel;
        ActivateSkills = activateSkills;
        ChanceOfChest = chanceOfChest;
    }

    public override void Deserialize(DeserializeEvent e)
    {
        base.Deserialize(e);
        Character = (EnemyCharacters)e.Reader.ReadInt32();
        Minimum = e.Reader.ReadInt32();
        Maximum = e.Reader.ReadInt32();
        CharacterRarity = (CharacterRarity)e.Reader.ReadInt32();
        CharacterName = e.Reader.ReadString();
        MinLevel = e.Reader.ReadInt32();
        MaxLevel= e.Reader.ReadInt32();
        ActivateSkills = e.Reader.ReadBoolean();
    }

    public override void Serialize(SerializeEvent e)
    {
        base.Serialize(e);
        e.Writer.Write((int)Character);
        e.Writer.Write(Minimum);
        e.Writer.Write(Maximum);
        e.Writer.Write((int)CharacterRarity);
        e.Writer.Write(CharacterName);
        e.Writer.Write(MinLevel);
        e.Writer.Write(MaxLevel);
        e.Writer.Write(ActivateSkills);
    }
}