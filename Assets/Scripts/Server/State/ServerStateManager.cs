using UnityEngine;
using Data.Characters;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

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

        WorldState.Scenes.Add(OverworldScene);

        SpawnEnemy("OverworldScene", "Possessed 1", new Vector2(-15, -10), new Possessed(), true);
        SpawnEnemy("OverworldScene", "Possessed 2", new Vector2(-10, -10), new Possessed(), false);
        SpawnEnemy("OverworldScene", "Possessed 3", new Vector2(-5, -10), new Possessed(), true);
        SpawnEnemy("OverworldScene", "Possessed 4", new Vector2(0, -10), new Possessed(), false);
        SpawnEnemy("OverworldScene", "Possessed 5", new Vector2(5, -10), new Possessed(), true);
        SpawnEnemy("OverworldScene", "Possessed 6", new Vector2(10, -10), new Possessed(), false);
        SpawnEnemy("OverworldScene", "Possessed 7", new Vector2(15, -10), new Possessed(), true);
        SpawnEnemy("OverworldScene", "Possessed 8", new Vector2(20, -10), new Possessed(), false);
    }

    public EnemyState SpawnEnemy(string scene, string name, Vector2 location, ICharacter character, bool activateSkills = false)
    {
        var OverworldScene = WorldState.Scenes.First(x => x.Name.ToLower() == scene.ToLower());

        var newEnemy = new EnemyState(
            name,
            location,
            character
        );

        if(activateSkills)
        {
            foreach(KeyValueState skill in newEnemy.Skills)
            {
                ActivateEnemySkill(newEnemy, skill.Key, scene);
            }
        }

        StateCalculator.CalcCharacterState(newEnemy);

        OverworldScene.Enemies.Add(newEnemy);

        ServerManager.BroadcastNetworkMessage(
            NetworkTags.SpawnEnemy, 
            new EnemyStateData(newEnemy, scene));

        return newEnemy;
    }

    // Spawn existing player
    public PlayerState SpawnPlayer(PlayerState playerState)
    {
        WorldState.Players.Add(playerState);

        playerState.isTargetable = false;

        if(playerState.IsDead)
        {
            playerState.TargetLocation = playerState.Location = new Vector2(Random.Range(-3, 3), Random.Range(-3, 3));
            playerState.Health = playerState.MaxHealth;
            playerState.Mana = playerState.MaxMana;
            playerState.IsDead = false;
            playerState.Experience = 0;

        }

        ServerManager.BroadcastNetworkMessage(
            NetworkTags.SpawnPlayer,
            new PlayerStateData(playerState)
        );

        ServerManager.Instance.StartCoroutine(TargetableCoroutine(playerState));
        IEnumerator TargetableCoroutine(PlayerState playerState)
        {
            yield return new WaitForSeconds(30);
            playerState.isTargetable = true;
        }

        return playerState;
    }

    // Spawn new player
    public PlayerState SpawnPlayer(string scene, int clientId, string username, Vector2 location, ICharacter character)
    {

        if (WorldState.Players.Count(x => x.ClientId == clientId) == 0)
        {
            var newPlayer =
                new PlayerState(
                    clientId,
                    username,
                    scene,
                    character,
                    location
            );

            newPlayer.AttackPoints = newPlayer.SkillPoints = newPlayer.PassivePoints = 50;

            StateCalculator.CalcCharacterState(newPlayer);

            WorldState.Players.Add(newPlayer);

            newPlayer.TargetLocation = newPlayer.Location = new Vector2(Random.Range(-3, 3), Random.Range(-3, 3));
            newPlayer.Health = newPlayer.MaxHealth;
            newPlayer.Mana = newPlayer.MaxMana;
            newPlayer.isTargetable = false;


            ServerManager.BroadcastNetworkMessage(
                NetworkTags.SpawnPlayer,
                new PlayerStateData(newPlayer)
            );

            ServerManager.Instance.StartCoroutine(TargetableCoroutine(newPlayer));
            IEnumerator TargetableCoroutine(PlayerState playerState)
            {
                yield return new WaitForSeconds(30);
                playerState.isTargetable = true;
            }

            return newPlayer;
        }

        return null;

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
