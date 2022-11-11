using UnityEngine;
using Data.Characters;
using System;
using UnityEngine.TextCore.Text;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Linq;

public class ServerStateManager
{
    public WorldState WorldState;
    public StateUpdater StateUpdater;
    public StateCalculator StateCalculator;
    public List<ActivatedPlayerSkill> ActivatedPlayerSkills;
    public List<ActivatedEnemySkill> ActivatedEnemySkills;

    private ItemGenerator ItemGenerator;

    public ServerStateManager()
    {
        WorldState = new WorldState();
        StateUpdater = new StateUpdater();
        ItemGenerator = new ItemGenerator();
        StateCalculator = new StateCalculator();
        ActivatedPlayerSkills = new List<ActivatedPlayerSkill>();
        ActivatedEnemySkills = new List<ActivatedEnemySkill>();
    }

    public void Update()
    {
        StateUpdater.Update(WorldState);
    }

    public void LoadScene(string scene)
    {
        SceneState OverworldScene = new SceneState() { Name = scene };

        OverworldScene.Enemies.Add(
            (EnemyState)StateCalculator.CalcCharacterState(new EnemyState(
                "Possessed 1",
                new Vector2(-2f, -4.5f),
                new Possessed()
                ))
            );
        ActivateEnemySkill(OverworldScene.Enemies[0], OverworldScene.Enemies[0].Skills[0].Key, "OverworldScene");
        ActivateEnemySkill(OverworldScene.Enemies[0], OverworldScene.Enemies[0].Skills[1].Key, "OverworldScene");
        //OverworldScene.Enemies[0].ActiveSkills.Add(
        //    new KeyValueState(
        //        OverworldScene.Enemies[0].Skills[0].Key,
        //        OverworldScene.Enemies[0].EnemyGuid.GetHashCode())
        //    { Index = OverworldScene.Enemies[0].Skills[0].Value });

        OverworldScene.Enemies.Add(
         (EnemyState)StateCalculator.CalcCharacterState(new EnemyState(
             "Possessed 2",
             new Vector2(-6f, -6f),
             new Possessed()
             ))
         );

        WorldState.Scenes.Add(OverworldScene);
    }

    public float[] GetAttackDamage(ICharacterState characterState)
    {
        return new float[]
        {
            Random.Range(
                characterState.PhysicalDamage - (characterState.PhysicalDamage / 100) * (100 - characterState.Accuracy),
                characterState.PhysicalDamage),
            Random.Range(
                characterState.FireDamage - (characterState.FireDamage / 100) * (100 - characterState.Accuracy),
                characterState.FireDamage),
            Random.Range(
                characterState.ColdDamage - (characterState.ColdDamage / 100) * (100 - characterState.Accuracy),
                characterState.ColdDamage)
        };
    }

    public void ActivatePlayerSkill(PlayerState playerState, string skill)
    {
        if (ActivatedPlayerSkills.Count(x => x.PlayerState == playerState && x.Skill.GetName() == skill) == 0)
        {
            ActivatedPlayerSkills.Add(new ActivatedPlayerSkill(
                playerState.Scene,
                playerState,
                CreateInstance.Skill(skill, playerState.Skills.First(x => x.Key == skill).Value),
                ActivatedPlayerSkills
                ).Activate());
        }
    }

    public void ActivateEnemySkill(EnemyState enemyState, string skill, string scene)
    {
        if (ActivatedEnemySkills.Count(x => x.EnemyState == enemyState && x.Skill.GetName() == skill) == 0)
        {
            ActivatedEnemySkills.Add(new ActivatedEnemySkill(
                scene,
                enemyState,
                CreateInstance.Skill(skill, enemyState.Skills.First(x => x.Key == skill).Value),
                ActivatedEnemySkills
                ).Activate());
        }
    }

    //public GameObject generateWorldDrop(GameObject dropObject, int itemLevel)
    //{

    //    float x = 0;
    //    float y = 0;

    //    // Set X 50/50 chance to move negative/postive out from dropObject by 1-2
    //    if(Random.Range(0,2) == 0) { x = Random.Range(0.5f, 1.5f); } else { x = Random.Range(-1.5f, -0.5f); }
    //    // Set Y 50/50 chance to move negative/postive out from dropObject by 1-2
    //    if (Random.Range(0, 2) == 0) { y = Random.Range(0.5f, 1.5f); } else {  y = Random.Range(-1.5f, -0.5f); }

    //    return generateWorldDrop(
    //        itemLevel,
    //        dropObject.transform.position.x + x,
    //        dropObject.transform.position.y + y
    //        );
    //}

    //public GameObject generateWorldDrop(int itemLevel, float x, float y)
    //{
    //    InventoryItem item = itemGenerator.createInventoryItem(itemLevel);
    //    worldDrops.Add(item);

    //    GameObject itemDrop =  Instantiate(itemDropPrefab, new Vector3(x, y, 0), Quaternion.identity);
    //    itemDrop.GetComponent<ItemDropSprite>().itemGuid = item.ItemGuid;
    //    itemDrop.GetComponent<ItemDropSprite>().inventoryItem = item;

    //    return itemDrop;

    //}

    //public List<GameObject> generateChestDrops(GameObject chest)
    //{
    //    ChestSprite chestSprite = chest.GetComponent<ChestSprite>();

    //    List<GameObject> drops = new List<GameObject>();

    //    int dropCount = GetWeightedChestDropCount(chestSprite.chestRarity);
    //    for (int i = 0; i < dropCount; i++)
    //    {
    //        drops.Add(generateWorldDrop(chest, chestSprite.chestLevel));
    //    }

    //    return drops;
    //}

    //private int GetWeightedChestDropCount(ChestRarity chestRarity)
    //{
    //    switch (chestRarity)
    //    {
    //        case ChestRarity.Common:
    //            return Random.Range(1, 3);
    //        case ChestRarity.Magic:
    //            return Random.Range(2, 5);
    //        case ChestRarity.Rare:
    //            return Random.Range(4, 7);
    //        case ChestRarity.Legendary:
    //            return Random.Range(6, 8);
    //        case ChestRarity.Mythic:
    //            return Random.Range(7, 10);
    //        default:
    //            return Random.Range(1, 3);
    //    }
    //}

    //public List<GameObject> generateEnemyDrops(GameObject enemy)
    //{
    //    EnemySprite enemySprite = enemy.GetComponent<EnemySprite>();

    //    List<GameObject> drops = new List<GameObject>();

    //    int dropCount = GetWeightedEnemyDropCount(enemySprite.enemyRarity);
    //    for (int i = 0; i < dropCount; i++)
    //    {
    //        drops.Add(generateWorldDrop(enemy, enemySprite.enemyLevel));
    //    }

    //    return drops;
    //}

    //private int GetWeightedEnemyDropCount(EnemyRarity enemyRarity)
    //{
    //    switch (enemyRarity)
    //    {
    //        case EnemyRarity.Common:
    //            return Random.Range(0, 2);
    //        case EnemyRarity.Magic:
    //            return Random.Range(1, 3);
    //        case EnemyRarity.Rare:
    //            return Random.Range(2, 4);
    //        case EnemyRarity.Legendary:
    //            return Random.Range(3, 5);
    //        case EnemyRarity.Mythic:
    //            return Random.Range(4, 6);
    //        case EnemyRarity.Boss:
    //            return Random.Range(7, 12);
    //        default:
    //            return Random.Range(0, 2);
    //    }
    //}
}
