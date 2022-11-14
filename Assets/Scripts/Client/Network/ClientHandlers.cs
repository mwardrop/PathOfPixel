using DarkRift;
using DarkRift.Client;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using static UnityEditor.Progress;

public class ClientHandlers
{

    private WorldState WorldState;
    private PlayerState PlayerState {  get
        {
            return StateManager.PlayerState;
        } 
    }
    private ClientStateManager StateManager;

    public ClientHandlers(ClientStateManager stateManager)
    {
        StateManager = stateManager;
        WorldState = stateManager.WorldState;
    }

    public void OnNetworkMessage(object sender, MessageReceivedEventArgs e)
    {
        using (Message message = e.GetMessage()) { 

            switch ((NetworkTags)message.Tag)
            {
                case NetworkTags.SpawnPlayer:
                    SpawnPlayer(message.Deserialize<PlayerStateData>());
                    break;
                case NetworkTags.MovePlayer:
                    MovePlayer(message.Deserialize<MovePlayerData>());
                    break;
                case NetworkTags.PlayerDisconnect:
                    PlayerDisconnect(message.Deserialize<IntegerData>());
                    break;
                case NetworkTags.SpawnEnemy:
                    SpawnEnemy(message.Deserialize<EnemyStateData>());
                    break;
                case NetworkTags.PlayerTakeDamage:
                    PlayerTakeDamage(message.Deserialize<PlayerTakeDamageData>());
                    break;
                case NetworkTags.PlayerAttack:
                    PlayerAttack(message.Deserialize<IntegerData>());
                    break;
                case NetworkTags.EnemyAttack:
                    EnemyAttack(message.Deserialize<GuidData>());
                    break;
                case NetworkTags.EnemyNewTarget:
                    EnemyNewTarget(message.Deserialize<EnemyPlayerPairData>());
                    break;
                case NetworkTags.EnemyTakeDamage:
                    EnemyTakeDamage(message.Deserialize<EnemyTakeDamageData>());
                    break;
                case NetworkTags.UpdatePlayerState:
                    UpdatePlayerState(message.Deserialize<PlayerStateData>());
                    break;
                case NetworkTags.CalculatePlayerState:
                    CalculatePlayerState(message.Deserialize<IntegerData>());
                    break;
                case NetworkTags.SetPlayerActiveAttack:
                    SetPlayerActiveAttack(message.Deserialize<StringIntegerData>());
                    break;
                case NetworkTags.SetPlayerDirection:
                    SetPlayerDirection(message.Deserialize<IntegerPairData>());
                    break;
                case NetworkTags.ActivatePlayerSkill:
                    ActivatePlayerSkill(message.Deserialize<SkillActivationData>());
                    break;
                case NetworkTags.DeactivatePlayerSkill:
                    DeactivatePlayerSkill(message.Deserialize<SkillActivationData>());
                    break;
                case NetworkTags.ActivateEnemySkill:
                    ActivateEnemySkill(message.Deserialize<SkillActivationData>());
                    break;
                case NetworkTags.DeactivateEnemySkill:
                    DeactivateEnemySkill(message.Deserialize<SkillActivationData>());
                    break;
                case NetworkTags.UpdatePlayerExperience:
                    UpdatePlayerExperience(message.Deserialize<IntegerPairData>());
                    break;
                case NetworkTags.UpdatePlayerRegen:
                    UpdatePlayerRegen(message.Deserialize<PlayerRegenData>());
                    break;
                case NetworkTags.UpdateEnemyRegen:
                    UpdateEnemyRegen(message.Deserialize<EnemyRegenData>());
                    break;
                case NetworkTags.ItemDropped:
                    ItemDropped(message.Deserialize<ItemDropData>());
                    break;

            }
        }
    }

    public void SpawnPlayer(PlayerStateData playerStateData)
    {
        
        PlayerState playerState = playerStateData.PlayerState;

        var existsInState = WorldState.Players.Count(x => x.ClientId == playerState.ClientId) != 0;

        if (existsInState)
        {
            PropertyCopier<PlayerState, PlayerState>.Copy(
                playerState, 
                WorldState.GetPlayerState(playerState.ClientId));

        } else { 
            WorldState.Players.Add(playerState);
        }

        StateManager.StateCalculator.CalcCharacterState(playerState);

        GameObject prefab = ClientManager.Prefabs.PossessedSprite;

        switch (playerState.Type.ToLower())
        {
            case "warrior":
                prefab = ClientManager.Prefabs.WarriorSprite;
                break;
        }

        GameObject newPlayer = CreateInstance.Prefab(prefab, playerState.Location);          

        if (playerState.ClientId == ClientManager.Instance.Client.ID)
        {
             newPlayer.tag = "LocalPlayer";
        }
        else
        {
            newPlayer.tag = "NetworkPlayer";
        }

        newPlayer.GetComponent<PlayerSprite>().NetworkClientId = playerState.ClientId;

        ServerManager.Instance.StartCoroutine(TargetableCoroutine(playerState));
        IEnumerator TargetableCoroutine(PlayerState playerState)
        {
            yield return new WaitForSeconds(30);
            playerState.isTargetable = true;
        }

    }

    public void SpawnEnemy(EnemyStateData enemyStateData)
    {
        EnemyState enemyState = enemyStateData.EnemyState;

        var existsInState = WorldState.Scenes
            .First(x => x.Name.ToLower() == enemyStateData.Scene.ToLower()).Enemies
            .Count(x => x.EnemyGuid == enemyStateData.EnemyState.EnemyGuid) != 0;

        if (existsInState)
        {
            PropertyCopier<EnemyState, EnemyState>.Copy(
                enemyState,
                WorldState.GetEnemyStateByGuid(enemyState.EnemyGuid, enemyStateData.Scene));

        }
        else
        {
            WorldState.Scenes.First(x => x.Name.ToLower() == enemyStateData.Scene.ToLower())
                .Enemies.Add(enemyState);
        }

        GameObject prefab = ClientManager.Prefabs.PossessedSprite;

        switch (enemyState.Type.ToLower())
        {
            case "possessed":
                prefab = ClientManager.Prefabs.PossessedSprite;
                break;
        }

        GameObject newEnemy = CreateInstance.Prefab(prefab, enemyState.Location);

        EnemySprite newEnemySprite = newEnemy.GetComponent<EnemySprite>();
        newEnemySprite.StateGuid = enemyState.EnemyGuid;
        newEnemySprite.TargetPlayerId = enemyState.TargetPlayerId;

    }

    public void MovePlayer(MovePlayerData movePlayerData)
    {
        var playerState = WorldState.GetPlayerState(movePlayerData.ClientId);

        playerState.TargetLocation = movePlayerData.Target;
        playerState.isTargetable = true;

    }

    public void PlayerDisconnect(IntegerData clientId)
    {
        WorldState.Players.RemoveAll(x => x.ClientId == clientId.Integer);
    }

    public void EnemyTakeDamage(EnemyTakeDamageData enemyTakeDamageData)
    {
        EnemyState enemy = WorldState.GetEnemyStateByGuid(enemyTakeDamageData.EnemyGuid, enemyTakeDamageData.Scene);

        enemy.Health = enemyTakeDamageData.Health;
        enemy.IsDead = enemyTakeDamageData.IsDead;

        if(enemy.IsDead)
        {
            ClientManager.Instance.StartCoroutine(DestroyCoroutine());
            IEnumerator DestroyCoroutine()
            {
                yield return new WaitForSeconds(60);
                StateManager.WorldState.Scenes
                    .First(x => x.Name.ToLower() == enemyTakeDamageData.Scene.ToLower()).Enemies
                    .Remove(enemy);

            }
        }

        StateManager.GetEnemyGameObject(enemyTakeDamageData.EnemyGuid)
            .GetComponent<EnemySprite>().SetState(SpriteState.Hurt);
    }

    public void EnemyAttack(GuidData guidData)
    {
        StateManager.GetEnemyGameObject(guidData.Guid)
            .GetComponent<EnemySprite>().SetState(SpriteState.Attack1);

    }

    public void PlayerTakeDamage(PlayerTakeDamageData playerTakeDamageData)
    {
        PlayerState player = WorldState.GetPlayerState(playerTakeDamageData.ClientId);

        player.Health = playerTakeDamageData.Health;
        player.IsDead = playerTakeDamageData.IsDead;

        StateManager.GetPlayerGameObject(player.ClientId)
            .GetComponent<PlayerSprite>().SetState(SpriteState.Hurt);
    }

    public void PlayerAttack(IntegerData integerData)
    {
        int clientId = integerData.Integer;

        PlayerState playerState = StateManager.WorldState.GetPlayerState(clientId);

        var attack = CreateInstance.Attack(playerState.ActiveAttack);

        StateManager.GetPlayerGameObject(clientId)
            .GetComponent<PlayerSprite>().SetState(
            (SpriteState)Enum.Parse(typeof(SpriteState), $"Attack{attack.AnimationId}"));
    }

    public void EnemyNewTarget(EnemyPlayerPairData enemyNewTargetData)
    {
        var enemyGuid = enemyNewTargetData.EnemyGuid;
        var clientId = enemyNewTargetData.ClientId;
        var sceneName = enemyNewTargetData.SceneName;

        WorldState.GetEnemyStateByGuid(enemyGuid, sceneName).TargetPlayerId = clientId;

        StateManager.GetEnemyGameObject(enemyGuid)
            .GetComponent<EnemySprite>().TargetPlayerId = clientId;
    }

    public void UpdatePlayerState(PlayerStateData playerState)
    {
        throw new System.Exception("UpdatePlayerState Client Handler Not Implemented.");
    }

    public void CalculatePlayerState(IntegerData integerData)
    {
        PlayerState playerState = StateManager.WorldState.GetPlayerState(integerData.Integer);
        StateManager.StateCalculator.CalcCharacterState(playerState);
    }

    public void SetPlayerActiveAttack(StringIntegerData stringIntegerData)
    { 
        PlayerState playerState = StateManager.WorldState.GetPlayerState(stringIntegerData.Integer);

        playerState.ActiveAttack = stringIntegerData.String;
        StateManager.StateCalculator.CalcCharacterState(playerState);
    }

    public void SetPlayerDirection(IntegerPairData integerData)
    {
        StateManager.GetPlayerGameObject(integerData.Integer1)
            .GetComponent<CharacterSprite>()
            .SetDirection((SpriteDirection)integerData.Integer2);
    }

    public void ActivatePlayerSkill(SkillActivationData skillActivationData)
    {
        var playerState = StateManager.WorldState.GetPlayerState(skillActivationData.Receivingharacter);

        if (playerState.ActiveSkills.Count(x => x.Key == skillActivationData.Name && x.Value == skillActivationData.ActivatingCharacter) == 0)
        {
            playerState.ActiveSkills.Add(new KeyValueState(
                skillActivationData.Name,
                skillActivationData.ActivatingCharacter) {  
                    Index = skillActivationData.Level 
            });

            StateManager.StateCalculator.CalcCharacterState(playerState);
        }
    }

    public void DeactivatePlayerSkill(SkillActivationData skillActivationData)
    {
        var playerState = StateManager.WorldState.GetPlayerState(skillActivationData.Receivingharacter);

        playerState.ActiveSkills.RemoveAll(x => x.Key == skillActivationData.Name && x.Value == skillActivationData.ActivatingCharacter);

        StateManager.StateCalculator.CalcCharacterState(playerState);
       
    }

    public void ActivateEnemySkill(SkillActivationData skillActivationData)
    {
        var enemyState = StateManager.WorldState.GetEnemyStateByHashCode(skillActivationData.Receivingharacter, skillActivationData.Scene);

        if (enemyState.ActiveSkills.Count(x => x.Key == skillActivationData.Name && x.Value == skillActivationData.ActivatingCharacter) == 0)
        {
            enemyState.ActiveSkills.Add(new KeyValueState(
                skillActivationData.Name,
                skillActivationData.ActivatingCharacter)
            {
                Index = skillActivationData.Level
            });

            StateManager.StateCalculator.CalcCharacterState(enemyState);
        }
    }

    public void DeactivateEnemySkill(SkillActivationData skillActivationData)
    {
        var enemyState = StateManager.WorldState.GetEnemyStateByHashCode(skillActivationData.Receivingharacter, skillActivationData.Scene);

        enemyState.ActiveSkills.RemoveAll(x => x.Key == skillActivationData.Name && x.Value == skillActivationData.ActivatingCharacter);

        StateManager.StateCalculator.CalcCharacterState(enemyState);

    }

    public void UpdatePlayerExperience(IntegerPairData integerPairData)
    {
        StateManager.WorldState.GetPlayerState(integerPairData.Integer1).Experience = integerPairData.Integer2;
        
    }

    public void UpdateEnemyRegen(EnemyRegenData enemyRegenData)
    {
        var enemy = StateManager.WorldState.GetEnemyStateByGuid(enemyRegenData.Guid, enemyRegenData.Scene);
        enemy.Health = enemyRegenData.Health;
        enemy.Mana = enemyRegenData.Mana;
    }

    public void UpdatePlayerRegen(PlayerRegenData playerRegenData)
    {
        var player = StateManager.WorldState.GetPlayerState(playerRegenData.Id);
        player.Health = playerRegenData.Health;
        player.Mana = playerRegenData.Mana;
    }

    public void ItemDropped(ItemDropData itemDropData)
    {
        var itemState = itemDropData.ItemState;
        StateManager.WorldState.Scenes
            .First(x => x.Name.ToLower() == itemDropData.Scene.ToLower()).ItemDrops
            .Add(itemState);

        GameObject itemDrop = CreateInstance.Prefab(ClientManager.Prefabs.ItemDropSprite, itemState.Location);
        var itemDropSprite = itemDrop.GetComponent<ItemDropSprite>();
        itemDropSprite.itemGuid = itemState.ItemGuid;
        itemDropSprite.ItemDrop = itemState;
    }

}