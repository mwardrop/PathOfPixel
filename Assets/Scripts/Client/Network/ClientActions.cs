using UnityEngine;

public class ClientActions {

    public void RequestSpawn()
    {
        ClientManager.SendNetworkMessage(NetworkTags.SpawnRequest);
    }

    public void RequestMove(Vector2 target)
    {
        ClientManager.SendNetworkMessage(NetworkTags.MoveRequest, new TargetData(target));
    }

    public void PlayerAttack(GameObject enemy)
    {
        ClientManager.SendNetworkMessage(NetworkTags.PlayerAttack, new GuidData(enemy.GetComponent<EnemySprite>().StateGuid));
    }

    public void EnemyAttack(GameObject enemy)
    {
        ClientManager.SendNetworkMessage(NetworkTags.EnemyAttack, new GuidData(enemy.GetComponent<EnemySprite>().StateGuid));
    }

}
