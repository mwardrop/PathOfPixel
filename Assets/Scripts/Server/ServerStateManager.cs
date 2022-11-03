using UnityEngine;
using Data.Characters;

public class ServerStateManager
{
    public WorldState WorldState = new WorldState();
    public StateUpdater StateUpdater;

    private ItemGenerator itemGenerator = new ItemGenerator();

    public ServerStateManager()
    {

        StateUpdater = new StateUpdater();
        SceneState OverworldScene = new SceneState() { Name = "OverworldScene" };

        var enemyState = new EnemyState();

        PropertyCopier<ICharacter, PlayerState>.Copy(
            new Possessed(),
            enemyState);

        enemyState.Name = "Possessed 1";
        enemyState.Level = 1;
        enemyState.Experience = 0;
        enemyState.Type = EnemyType.Possessed;
        enemyState.Scene = "OverworldScene";
        enemyState.Location = new Vector2(-2f, -4.5f);
        enemyState.HomeLocation = new Vector2(-2f, -4.5f);
        enemyState.TargetPlayerId = -1;

        OverworldScene.Enemies.Add(enemyState);

        //OverworldScene.Enemies.Add(new EnemyState()
        //{
        //    Name = "Possessed 2",
        //    Health = 30,
        //    HealthRegen = 1,
        //    Mana = 100,
        //    ManaRegen = 1,
        //    PhysicalDamage = 5,
        //    FireDamage = 0,
        //    ColdDamage = 0,
        //    FireResistance = 0,
        //    ColdResistance = 0,
        //    Armor = 0,
        //    Dodge = 0,
        //    Level = 1,
        //    Experience = 0,
        //    Type = EnemyType.Possessed,
        //    Location = new Vector2(6f, -4.5f),
        //    MoveSpeed = 2.0f,
        //    TargetPlayerId = -1
        //});

        WorldState.Scenes.Add(OverworldScene);
    }

    public void Update()
    {
        StateUpdater.Update(WorldState);
    }

    public float[] GetCharacterDamage(CharacterState characterState)
    {
        ICharacter character = new BaseCharacter();

        if(characterState.GetType() == typeof(PlayerState))
        {
            switch (((PlayerState)characterState).Type)
            {
                case PlayerType.Warrior:
                    character =  new Warrior().ApplyCharacterState(characterState);
                    break;

            }
        } else
        {
            switch (((EnemyState)characterState).Type)
            {
                case EnemyType.Possessed:
                    character = new Possessed().ApplyCharacterState(characterState);
                    break;
            }
        }

        return new float[3] { 
            character.PhysicalDamage, 
            character.FireDamage, 
            character.ColdDamage };
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
