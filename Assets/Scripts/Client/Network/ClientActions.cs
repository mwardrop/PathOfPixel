using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientActions {

    public void Spawn()
    {
        ClientManager.SendNetworkMessage(NetworkTags.SpawnRequest);
    }

    public void Move(Vector2 target)
    {
        ClientManager.SendNetworkMessage(NetworkTags.MoveRequest, new TargetData(target));
    }

    public void PlayerAttack()
    {
        ClientManager.SendNetworkMessage(NetworkTags.PlayerAttack);
    }

    public void PlayerHitEnemy(GameObject player, GameObject enemy)
    {
        if (ClientManager.IsHost)
        {
            ClientManager.SendNetworkMessage(
                NetworkTags.PlayerHitEnemy,
                new EnemyPlayerPairData(
                    enemy.GetComponent<EnemySprite>().StateGuid,
                    player.GetComponent<PlayerSprite>().NetworkClientId,
                    ClientManager.Instance.StateManager.PlayerState.Scene));
        }
    }

    public void EnemyAttack(EnemySprite enemy)
    {
        ClientManager.SendNetworkMessage(NetworkTags.EnemyAttack, new GuidData(enemy.StateGuid));
    }

    public void EnemyHitPlayer(GameObject player, GameObject enemy)
    {
        if (ClientManager.IsHost)
        {
            ClientManager.SendNetworkMessage(
                NetworkTags.EnemyHitPlayer, 
                new EnemyPlayerPairData(
                    enemy.GetComponent<EnemySprite>().StateGuid,
                    player.GetComponent<PlayerSprite>().NetworkClientId,
                    ClientManager.Instance.StateManager.PlayerState.Scene));
        }
    }

    public void UpdateEnemyLocation(System.Guid enemyGuid, Vector2 location, string scene)
    {
        ClientManager.Instance.StateManager.WorldState.Scenes
            .First(X => X.Name.ToLower() == SceneManager.GetActiveScene().name.ToLower())
            .Enemies.First(x => x.EnemyGuid == enemyGuid).Location = location;

        if (ClientManager.IsHost)
        {
            ClientManager.SendNetworkMessage(
                NetworkTags.UpdateEnemyLocation,
                new UpdateEnemyLocationData(enemyGuid, location, scene));
        }
    }

}
