using DarkRift;
using DarkRift.Server;
using Data.Characters;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ServerConnection
{
    public string Username { get; }
    public IClient Client { get; }

    public PlayerState PlayerState;

    public ServerStateManager StateManager;

    public ServerConnection(IClient client, LoginRequestData data, ServerStateManager stateManager)
    {

        Client = client;
        Username = data.Username;
        StateManager = stateManager;

        Client.MessageReceived += OnMessage;

        // TODO : User should be able to create new and load existing PlayerStates (Warrior, Mage / Load, New)
        PlayerState = new PlayerState();

        PropertyCopier<ICharacter, PlayerState>.Copy(
            new Warrior(),
            PlayerState);

        PlayerState.Name = data.Username;
        PlayerState.Level = 1;
        PlayerState.Experience = 0;
        PlayerState.Type = PlayerType.Warrior;
        PlayerState.Scene = "OverworldScene";
        PlayerState.ClientId = client.ID;
        PlayerState.isTargetable = false;

    }

    public void Disconnect()
    {
        BroadcastNetworkMessage(
            NetworkTags.PlayerDisconnect,
            new IntegerData(Client.ID));

        StateManager.WorldState.Players.RemoveAll(x => x.ClientId == Client.ID);
        ServerManager.Instance.Connections.Remove(Client.ID);

    }

    private void OnMessage(object sender, MessageReceivedEventArgs e)
    {
        IClient client = (IClient)sender;
        using (Message message = e.GetMessage())
        {
            switch ((NetworkTags)message.Tag)
            {
                case NetworkTags.SpawnRequest:
                    SpawnPlayer();
                    break;
                case NetworkTags.MoveRequest:
                    MovePlayer(message.Deserialize<TargetData>());
                    break;
                case NetworkTags.PlayerAttack:
                    PlayerAttack();
                    break;
                case NetworkTags.PlayerHitEnemy:
                    PlayerHitEnemy(message.Deserialize<EnemyPlayerPairData>());
                    break;
                case NetworkTags.EnemyAttack:
                    EnemyAttack(message.Deserialize<GuidData>());
                    break;
                case NetworkTags.EnemyHitPlayer:
                    EnemyHitPlayer(message.Deserialize<EnemyPlayerPairData>());
                    break;
                case NetworkTags.UpdateEnemyLocation:
                    UpdateEnemyLocation(message.Deserialize<UpdateEnemyLocationData>());
                    break;
            }
        }
    }

    private void PlayerAttack()
    {
        BroadcastNetworkMessage(
            NetworkTags.PlayerAttack,
            new IntegerData(Client.ID)
        );    
    }

    private void PlayerHitEnemy(EnemyPlayerPairData enemyPlayerPairData)
    {
        EnemyState enemy = StateManager.WorldState.GetEnemyState(
            enemyPlayerPairData.EnemyGuid,
            enemyPlayerPairData.SceneName);

        PlayerState player = StateManager.WorldState.GetPlayerState(enemyPlayerPairData.ClientId);

        float[] damage = StateManager.GetCharacterDamage(PlayerState);

        enemy.IncomingPhysicalDamage += damage[0];
        enemy.IncomingFireDamage += damage[1];
        enemy.IncomingColdDamage += damage[2];
    }

    private void EnemyAttack(GuidData guidData)
    {
        BroadcastNetworkMessage(
            NetworkTags.EnemyAttack,
            guidData
        );

    }

    private void EnemyHitPlayer(EnemyPlayerPairData enemyPlayerPairData)
    {
        EnemyState enemy = StateManager.WorldState.GetEnemyState(
            enemyPlayerPairData.EnemyGuid, 
            enemyPlayerPairData.SceneName);
        PlayerState player = StateManager.WorldState.GetPlayerState(enemyPlayerPairData.ClientId);

        float[] damage = StateManager.GetCharacterDamage(enemy);

        player.IncomingPhysicalDamage += damage[0];
        player.IncomingFireDamage += damage[1];
        player.IncomingColdDamage += damage[2];

    }

    private void SpawnPlayer()
    {
        PlayerState.TargetLocation = PlayerState.Location = new Vector2(Random.Range(-3, 3), Random.Range(-3, 3));
        PlayerState.isTargetable = false;

        BroadcastNetworkMessage(
            NetworkTags.SpawnPlayer,
            new PlayerStateData(PlayerState)
        );

        if (StateManager.WorldState.Players.Count(x => x.ClientId == PlayerState.ClientId) == 0)
        {
            StateManager.WorldState.Players.Add(PlayerState);
        }

        ServerManager.Instance.StartCoroutine(TargetableCoroutine(PlayerState));
        IEnumerator TargetableCoroutine(PlayerState playerState)
        {
            yield return new WaitForSeconds(30);
            playerState.isTargetable = true;
        }

    }

    private void MovePlayer(TargetData targetData)
    {

        Vector2 target = targetData.Target;

        PlayerState.TargetLocation = PlayerState.Location = target;
        PlayerState.isTargetable = true;

        BroadcastNetworkMessage(
            NetworkTags.MovePlayer,
            new MovePlayerData(Client.ID, PlayerState.TargetLocation)
        );
    }

    private void SendNetworkMessage(NetworkTags networkTag, IDarkRiftSerializable payload)
    {
        ServerManager.SendNetworkMessage(Client, networkTag, payload);
    }

    private void BroadcastNetworkMessage(NetworkTags networkTag, IDarkRiftSerializable payload)
    {
        ServerManager.BroadcastNetworkMessage(networkTag, payload);
    }

    private void UpdateEnemyLocation(UpdateEnemyLocationData updateEnemyLocationData)
    {
        StateManager.WorldState.GetEnemyState(
            updateEnemyLocationData.EnemyGuid,
            updateEnemyLocationData.SceneName).Location = updateEnemyLocationData.Location;

        //ServerManager.BroadcastNetworkMessage(
        //    NetworkTags.UpdateEnemyLocation,
        //    updateEnemyLocationData);

    }
}