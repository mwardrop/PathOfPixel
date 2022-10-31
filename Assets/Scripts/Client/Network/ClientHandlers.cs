using DarkRift;
using DarkRift.Client;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;

public class ClientHandlers
{

    private WorldState WorldState;
    private PlayerState PlayerState;

    public ClientHandlers(WorldState worldState, PlayerState playerState)
    {
        WorldState = worldState;
        PlayerState = playerState;
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
                case NetworkTags.EnemyTakeDamage:
                    EnemyTakeDamage(message.Deserialize<EnemyTakeDamageData>());
                    break;
                case NetworkTags.PlayerTakeDamage:
                    PlayerTakeDamage(message.Deserialize<PlayerTakeDamageData>());
                    break;
                case NetworkTags.PlayerAttack:
                    PlayerAttack(message.Deserialize<IntegerData>());
                    break;
                case NetworkTags.EnemyNewTarget:
                    EnemyNewTarget(message.Deserialize<EnemyNewTargetData>());
                    break;
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
                WorldState.Players.First(x => x.ClientId == playerState.ClientId));

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

    }

    public void SpawnEnemy(EnemyStateData enemyStateData)
    {
        EnemyState enemyState = enemyStateData.EnemyState;

        GameObject prefab = ClientManager.Prefabs.PossessedSprite;

        switch (enemyState.Type)
        {
            case EnemyType.Possessed:
                prefab = ClientManager.Prefabs.PossessedSprite;
                break;
        }

        GameObject newEnemy = UnityEngine.Object.Instantiate(
            prefab,
            enemyState.Location,
            Quaternion.identity);

        newEnemy.GetComponent<EnemySprite>().StateGuid = enemyState.EnemyGuid;

    }

    public void MovePlayer(MovePlayerData movePlayerData)
    {
        WorldState.Players.First(x => x.ClientId == movePlayerData.ClientId).TargetLocation = movePlayerData.Target;
    }

    public void PlayerDisconnect(IntegerData clientId)
    {
        WorldState.Players.RemoveAll(x => x.ClientId == clientId.Integer);
    }

    public void EnemyTakeDamage(EnemyTakeDamageData enemyTakeDamageData)
    {
        EnemyState enemy = WorldState
            .Scenes.First(x => x.Name == PlayerState.Scene)
            .Enemies.First(x => x.EnemyGuid == enemyTakeDamageData.EnemyGuid);

        enemy.Health = enemyTakeDamageData.Health;
        enemy.IsDead = enemyTakeDamageData.IsDead;

        GameObject.FindGameObjectsWithTag("Enemy").ToList().
            First(x => x.GetComponent<EnemySprite>().StateGuid == enemyTakeDamageData.EnemyGuid)
            .GetComponent<EnemySprite>().SetState(SpriteState.Hurt);
    }

    public void PlayerTakeDamage(PlayerTakeDamageData playerTakeDamageData)
    {
        WorldState
            .Players.First(x => x.ClientId == playerTakeDamageData.ClientId)
            .Health += playerTakeDamageData.Health;
    }

    public void PlayerAttack(IntegerData integerData)
    {
        int clientId = integerData.Integer;

        if (clientId != PlayerState.ClientId)
        {
            List<GameObject> networkPlayers = GameObject.FindGameObjectsWithTag("NetworkPlayer").ToList();
            GameObject attackingPlayer = networkPlayers.First(x => x.GetComponent<PlayerSprite>().NetworkClientId == integerData.Integer);
            attackingPlayer.GetComponent<CharacterSprite>().SetState(SpriteState.Attack1);
        }
    }

    public void EnemyNewTarget(EnemyNewTargetData enemyNewTargetData)
    {
        var enemyGuid = enemyNewTargetData.EnemyGuid;
        var clientId = enemyNewTargetData.ClientId;
        var sceneName = enemyNewTargetData.SceneName;

        WorldState.Scenes.First(x => x.Name.ToLower() == sceneName.ToLower()).Enemies
            .First(x => x.EnemyGuid == enemyGuid).TargetPlayerId = clientId;

        EnemySprite temp = GameObject.FindGameObjectsWithTag("Enemy").ToList().
            First(x => x.GetComponent<EnemySprite>().StateGuid == enemyGuid)
            .GetComponent<EnemySprite>();
        temp.TargetPlayerId = clientId;

    }

}