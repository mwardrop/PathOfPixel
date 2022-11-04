using DarkRift;
using DarkRift.Client;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class ClientHandlers
{

    private WorldState WorldState;
    private PlayerState PlayerState;
    private ClientStateManager StateManager;

    public ClientHandlers(ClientStateManager stateManager)
    {
        StateManager = stateManager;
        WorldState = stateManager.WorldState;
        PlayerState = stateManager.PlayerState;
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
                    UpdatePlayerState(message.Deserialize<IntegerData>());
                    break;
                    //case NetworkTags.UpdateEnemyLocation:
                    //    UpdateEnemyLocation(message.Deserialize<UpdateEnemyLocationData>());
                    //    break;

            }
        }
    }

    public void SpawnPlayer(PlayerStateData playerStateData)
    {
        
        PlayerState playerState = playerStateData.PlayerState;

        if (WorldState.Players.Where(x => x.ClientId == playerState.ClientId).Count() > 0)
        {
            PropertyCopier<PlayerState, PlayerState>.Copy(
                playerState, 
                WorldState.GetPlayerState(playerState.ClientId));

        } else { 
            WorldState.Players.Add(playerState);
        }

        GameObject newPlayer = UnityEngine.Object.Instantiate(
            ClientManager.Prefabs.WarriorSprite,
            playerState.Location,
            Quaternion.identity);

        if (playerState.ClientId == PlayerState.ClientId)
        {
             newPlayer.tag = "LocalPlayer";
        }
        else
        {
            newPlayer.tag = "NetworkPlayer";
        }

        newPlayer.GetComponent<PlayerSprite>().NetworkClientId = playerState.ClientId;

        ServerManager.Instance.StartCoroutine(TargetableCoroutine(PlayerState));
        IEnumerator TargetableCoroutine(PlayerState playerState)
        {
            yield return new WaitForSeconds(30);
            playerState.isTargetable = true;
        }

    }

    public void SpawnEnemy(EnemyStateData enemyStateData)
    {
        EnemyState enemyState = enemyStateData.EnemyState;

        GameObject prefab = ClientManager.Prefabs.PossessedSprite;

        switch (enemyState.Type)
        {
            case CharacterType.Possessed:
                prefab = ClientManager.Prefabs.PossessedSprite;
                break;
        }

        GameObject newEnemy = UnityEngine.Object.Instantiate(
            prefab,
            enemyState.Location,
            Quaternion.identity);

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
        EnemyState enemy = WorldState.GetEnemyState(enemyTakeDamageData.EnemyGuid, PlayerState.Scene);

        enemy.Health = enemyTakeDamageData.Health;
        enemy.IsDead = enemyTakeDamageData.IsDead;

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

        StateManager.GetPlayerGameObject(clientId)
            .GetComponent<PlayerSprite>().SetState(SpriteState.Attack1);
    }

    public void EnemyNewTarget(EnemyPlayerPairData enemyNewTargetData)
    {
        var enemyGuid = enemyNewTargetData.EnemyGuid;
        var clientId = enemyNewTargetData.ClientId;
        var sceneName = enemyNewTargetData.SceneName;

        WorldState.GetEnemyState(enemyGuid, sceneName).TargetPlayerId = clientId;

        StateManager.GetEnemyGameObject(enemyGuid)
            .GetComponent<EnemySprite>().TargetPlayerId = clientId;
    }

    public void UpdatePlayerState(PlayerStateData playerState)
    {
        StateManager.PlayerState
    }

    public void SetPlayerActiveAttack(IntegerData integerData)
    { 
        PlayerState playerState = StateManager.WorldState.GetPlayerState(integerData.Integer));
        StateManager.StateCalculator.CalculateCharacterState(StateManager.WorldState.GetPlayerState(integerData.Integer));
    }

    //public void UpdateEnemyLocation(UpdateEnemyLocationData updateEnemyLocationData)
    //{
    //    var enemyGuid = updateEnemyLocationData.EnemyGuid;
    //    var location = updateEnemyLocationData.Location;
    //    var sceneName = updateEnemyLocationData.SceneName;

    //    var enemySprite = StateManager.GetEnemyGameObject(enemyGuid)
    //        .GetComponent<EnemySprite>();

    //    if (enemySprite.TargetPlayerId != ClientManager.Instance.Client.ID)
    //    {
    //        // Correct the enemies position if client is too far out of sync with server
    //        if (Vector2.Distance(new Vector2(enemySprite.transform.position.x, enemySprite.transform.position.y), location) > 1)
    //        {
    //            var enemyState = WorldState.GetEnemyState(enemyGuid, sceneName);

    //            enemyState.Location = location;
    //        }
    //    }
    //}

}