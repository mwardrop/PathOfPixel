using UnityEngine;
using Data.Characters;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;
using DarkRift;
using System.IO;

public class ServerStateManager
{
    public WorldState WorldState;
    public StateUpdater StateUpdater;
    public StateCalculator StateCalculator;
    public List<ActivatedPlayerSkill> ActivatedPlayerSkills;
    public List<ActivatedEnemySkill> ActivatedEnemySkills;
    public List<ActiveTradeState> ActiveTrades;

    private ItemGenerator ItemGenerator;

    public ServerStateManager()
    {
        WorldState = new WorldState();      
        ItemGenerator = new ItemGenerator();
        StateCalculator = new StateCalculator();
        ActivatedPlayerSkills = new List<ActivatedPlayerSkill>();
        ActivatedEnemySkills = new List<ActivatedEnemySkill>();
        ActiveTrades = new List<ActiveTradeState>();

        LoadScene("OverworldScene"); // TODO: Load all scenes on server side

        StateUpdater = new StateUpdater(this);
    }

    public void LoadScene(string scene)
    {
        var byteArray = File.ReadAllBytes($"{Application.persistentDataPath}/Scenes/{scene}.dat");
        var reader = DarkRiftReader.CreateFromArray(byteArray, 0, byteArray.Length);
        var sceneState = reader.ReadSerializable<SceneState>();

        WorldState.Scenes.Add(sceneState);
    }

    public EnemyState SpawnEnemy(SceneState scene, EnemySpawnState spawn, string name, Vector2 location, ICharacter character, bool activateSkills = false)
    {
        var newEnemy = new EnemyState(
            name,
            spawn.Name,
            location,
            character
        );

        if(activateSkills)
        {
            foreach(KeyValueState skill in newEnemy.Skills)
            {
                ActivateEnemySkill(newEnemy, skill.Key, scene.Name);
            }
        }

        StateCalculator.CalcCharacterState(newEnemy);

        scene.Enemies.Add(newEnemy);

        ServerManager.BroadcastNetworkMessage(
            NetworkTags.SpawnEnemy, 
            new EnemyStateData(newEnemy, scene.Name));

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
    public PlayerState SpawnPlayer(SceneState scene, int clientId, string username, Vector2 location, ICharacter character)
    {

        if (WorldState.Players.Count(x => x.ClientId == clientId) == 0)
        {
            var newPlayer =
                new PlayerState(
                    clientId,
                    username,
                    scene.Name,
                    character,
                    location
            );

            newPlayer.AttackPoints = newPlayer.SkillPoints = newPlayer.PassivePoints = 50;
            //newPlayer.TargetLocation = newPlayer.Location = new Vector2(Random.Range(-3, 3), Random.Range(-3, 3));
            newPlayer.isTargetable = false;
            newPlayer.HotbarItems.Add(new KeyValueState("SweepAttack", 0) { Index = 1 });
            newPlayer.HotbarItems.Add(new KeyValueState("FrenzySkill", 1) { Index = 6 });

            StateCalculator.CalcCharacterState(newPlayer);

            WorldState.Players.Add(newPlayer);

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

    public void ApplyIncomingDamageToEnemy(EnemyState enemy, SceneState scene)
    {
        if (enemy.IncomingPhysicalDamage > 0 || enemy.IncomingFireDamage > 0 || enemy.IncomingColdDamage > 0)
        {

            enemy.Health -= enemy.IncomingPhysicalDamage;
            enemy.Health -= enemy.IncomingFireDamage;
            enemy.Health -= enemy.IncomingColdDamage;

            enemy.IsDead = enemy.Health <= 0 ? true : false;

            enemy.IncomingPhysicalDamage = 0;
            enemy.IncomingFireDamage = 0;
            enemy.IncomingColdDamage = 0;

            ServerManager.BroadcastNetworkMessage(
                NetworkTags.EnemyTakeDamage,
                new EnemyTakeDamageData(enemy.EnemyGuid, enemy.Health, scene.Name, enemy.IsDead));

            if (enemy.IsDead)
            {

                ServerManager.Instance.StartCoroutine(DestroyCoroutine());
                IEnumerator DestroyCoroutine()
                {
                    yield return null;

                    GenerateEnemyDeathRewards(enemy, scene.Name);

                    yield return new WaitForSeconds(60);
                    WorldState.Scenes
                        .First(x => x.Name.ToLower() == scene.Name.ToLower()).Enemies
                        .Remove(enemy);

                }
            }
        }

    }

    public void ApplyIncomingDamageToPlayer(PlayerState player)
    {
        if (player.IncomingPhysicalDamage > 0 || player.IncomingFireDamage > 0 || player.IncomingColdDamage > 0)
        {

            player.Health -= player.IncomingPhysicalDamage;
            player.Health -= player.IncomingFireDamage;
            player.Health -= player.IncomingColdDamage;

            player.IsDead = player.Health <= 0 ? true : false;

            player.IncomingPhysicalDamage = 0;
            player.IncomingFireDamage = 0;
            player.IncomingColdDamage = 0;

            ServerManager.BroadcastNetworkMessage(
                NetworkTags.PlayerTakeDamage,
                new PlayerTakeDamageData(player.ClientId, player.Health, player.IsDead));
        }

    }

    public void SetEnemyTarget(EnemyState enemy, List<PlayerState> players, SceneState scene)
    {
        if (players.Count > 0)
        {
            Dictionary<int, float> distanceFromPlayers = new Dictionary<int, float>();

            foreach (PlayerState player in players)
            {
                if (player.Scene.ToLower() == scene.Name.ToLower() && player.isTargetable)
                {
                    distanceFromPlayers.Add(player.ClientId, Vector2.Distance(player.Location, enemy.Location));
                }
            }

            if (distanceFromPlayers.Count > 0)
            {

                int targetId = distanceFromPlayers.OrderByDescending(x => x.Value).Last().Key;

                if (enemy.TargetPlayerId != targetId)
                {
                    enemy.TargetPlayerId = targetId;

                    ServerManager.BroadcastNetworkMessage(
                        NetworkTags.EnemyNewTarget,
                        new EnemyPlayerPairData(enemy.EnemyGuid, targetId, scene.Name));
                }
            }
        }

    }

    public void ApplyRegenToCharacter(ICharacterState character)
    {
        if (character.Health != character.MaxHealth)
        {
            character.Health = Math.Min(character.Health + character.HealthRegen, character.MaxHealth);
        }
        if (character.Mana != character.MaxMana)
        {
            character.Mana = Math.Min(character.Mana + character.ManaRegen, character.MaxMana);
        }
    }

    public void GenerateEnemyDeathRewards(EnemyState enemy, string scene)
    {
        // Distribute Experience based on damage done
        var totalDamage = enemy.DamageTracker.Sum(x => x.Value);
        foreach (KeyValueState playerDamage in enemy.DamageTracker)
        {
            var player = WorldState.GetPlayerState(Int32.Parse(playerDamage.Key));
            player.Experience += enemy.Experience * ((playerDamage.Value * 100) / totalDamage) / 100;

            ServerManager.BroadcastNetworkMessage(
                NetworkTags.UpdatePlayerExperience,
                new IntegerPairData(player.ClientId, player.Experience));
        }


        List<ItemState> itemDrops = new List<ItemState>();

        int GetWeightedEnemyDropCount(CharacterRarity characterType)
        {
            switch (characterType)
            {
                case Data.Characters.CharacterRarity.Common:
                    return Random.Range(0, 2);
                case Data.Characters.CharacterRarity.Magic:
                    return Random.Range(1, 3);
                case Data.Characters.CharacterRarity.Rare:
                    return Random.Range(2, 4);
                case Data.Characters.CharacterRarity.Legendary:
                    return Random.Range(3, 5);
                case Data.Characters.CharacterRarity.Mythic:
                    return Random.Range(4, 6);
                case Data.Characters.CharacterRarity.Boss:
                    return Random.Range(7, 12);
                default:
                    return Random.Range(0, 2);
            }
        }

        int dropCount = GetWeightedEnemyDropCount(enemy.Rarity);
        for (int i = 0; i < dropCount; i++)
        {
            GenerateItemDrop(scene, enemy.Location, enemy.Level);
        }

    }

    public ItemState GenerateItemDrop(string scene, Vector2 location, int itemLevel)
    {
        ItemState item = ItemGenerator.CreateItem(Random.Range(itemLevel - 5, itemLevel + 5));

        float x = 0;
        float y = 0;

        // Set X 50/50 chance to move negative/postive out from dropObject by 1-2
        if (Random.Range(0, 2) == 0) { x = Random.Range(0.5f, 1.5f); } else { x = Random.Range(-1.5f, -0.5f); }
        // Set Y 50/50 chance to move negative/postive out from dropObject by 1-2
        if (Random.Range(0, 2) == 0) { y = Random.Range(0.5f, 1.5f); } else { y = Random.Range(-1.5f, -0.5f); }

        item.Location = new Vector2(location.x + x, location.y + y);
        
        WorldState.Scenes.First(x => x.Name.ToLower() == scene.ToLower()).ItemDrops.Add(item);

        ServerManager.BroadcastNetworkMessage(
            NetworkTags.ItemDropped,
            new ItemDropData(item, scene));

        return item;
    }

    public int FindEmptyInventorySlot(PlayerState playerState)
    {
        var freeSlot = 0;

        foreach (InventoryItemState inventoryItem in playerState.Inventory.Items.OrderBy(x => x.Slot))
        {
            if ((int)inventoryItem.Slot != freeSlot)
            {
                return freeSlot;
            }
            freeSlot++;
        }

        return freeSlot;

    }



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
