using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "PathOfPixel/GameState", order = 1)]
public class GameState : ScriptableObject
{
    public GameObject itemDropPrefab;
    public PlayerState playerState;
    private ItemGenerator itemGenerator = new ItemGenerator();

    public List<InventoryItem> worldDrops;

    public void OnEnable()
    {
        worldDrops.Clear();
    }

    public GameObject generateWorldDrop(GameObject dropObject, int itemLevel)
    {
        return generateWorldDrop(
            itemLevel,
            dropObject.transform.position.x + UnityEngine.Random.Range(-2.0f, 2.0f),
            dropObject.transform.position.y + UnityEngine.Random.Range(-2.0f, 2.0f)
            );
    }

    public GameObject generateWorldDrop(int itemLevel, float x, float y)
    {
        InventoryItem item = itemGenerator.createInventoryItem(itemLevel);
        worldDrops.Add(item);

        GameObject itemDrop =  Instantiate(itemDropPrefab, new Vector3(x, y, 0), Quaternion.identity);
        itemDrop.GetComponent<ItemDropSprite>().itemGuid = item.itemGuid;

        return itemDrop;

    }

    public List<GameObject> generateChestDrops(GameObject chest)
    {
        ChestSprite chestSprite = chest.GetComponent<ChestSprite>();

        List<GameObject> drops = new List<GameObject>();

        int dropCount = GetWeightedChestDropCount(chestSprite.chestRarity);
        for (int i = 0; i < dropCount; i++)
        {
            drops.Add(generateWorldDrop(chest, chestSprite.chestLevel));
        }

        return drops;
    }

    private int GetWeightedChestDropCount(ChestRarity chestRarity)
    {
        switch (chestRarity)
        {
            case ChestRarity.Common:
                return Random.Range(1, 3);
            case ChestRarity.Magic:
                return Random.Range(2, 5);
            case ChestRarity.Rare:
                return Random.Range(4, 7);
            case ChestRarity.Legendary:
                return Random.Range(6, 8);
            case ChestRarity.Mythic:
                return Random.Range(7, 10);
            default:
                return Random.Range(1, 3);
        }
    }

    public List<GameObject> generateEnemyDrops(GameObject enemy)
    {
        EnemySprite enemySprite = enemy.GetComponent<EnemySprite>();

        List<GameObject> drops = new List<GameObject>();

        int dropCount = GetWeightedEnemyDropCount(enemySprite.enemyRarity);
        for (int i = 0; i < dropCount; i++)
        {
            drops.Add(generateWorldDrop(enemy, enemySprite.enemyLevel));
        }

        return drops;
    }

    private int GetWeightedEnemyDropCount(EnemyRarity enemyRarity)
    {
        switch (enemyRarity)
        {
            case EnemyRarity.Common:
                return Random.Range(0, 2);
            case EnemyRarity.Magic:
                return Random.Range(1, 3);
            case EnemyRarity.Rare:
                return Random.Range(2, 4);
            case EnemyRarity.Legendary:
                return Random.Range(3, 5);
            case EnemyRarity.Mythic:
                return Random.Range(4, 6);
            case EnemyRarity.Boss:
                return Random.Range(7, 12);
            default:
                return Random.Range(0, 2);
        }
    }
}
