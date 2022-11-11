using DarkRift;
using DarkRift.Server;
using Data.Characters;
using System;
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

    public ServerConnection(IClient client, string username, ServerStateManager stateManager)
    {

        Client = client;
        Username = username;
        StateManager = stateManager;

        Client.MessageReceived += OnMessage;

        // TODO : User should be able to create new and load existing PlayerStates (Warrior, Mage / Load, New)
        PlayerState = (PlayerState)StateManager.StateCalculator.CalcCharacterState(
            new PlayerState(
                Client.ID, 
                username, 
                "OverworldScene",
                new Warrior()
            )
        );
        PlayerState.AttackPoints = PlayerState.SkillPoints = PlayerState.PassivePoints = 50;
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
                    MovePlayer(message.Deserialize<Vector2Data>());
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
                case NetworkTags.UpdatePlayerState:
                    UpdatePlayerState();
                    break;
                case NetworkTags.CalculatePlayerState:
                    CalculatePlayerState();
                    break;
                case NetworkTags.SetPlayerActiveAttack:
                    SetPlayerActiveAttack(message.Deserialize<StringData>());
                    break;
                case NetworkTags.SpendAttackPoint:
                    SpendAttackPoint(message.Deserialize<StringData>());
                    break;
                case NetworkTags.SpendSkillPoint:
                    SpendSkillPoint(message.Deserialize<StringData>());
                    break;
                case NetworkTags.SpendPassivePoint:
                    SpendPassivePoint(message.Deserialize<StringData>());
                    break;
                case NetworkTags.SetPlayerDirection:
                    SetPlayerDirection(message.Deserialize<IntegerData>());
                    break;
                case NetworkTags.SetPlayerHotbarItem:
                    SetPlayerHotbarItem(message.Deserialize<KeyValueStateData>());
                    break;
                case NetworkTags.ActivatePlayerSkill:
                    ActivatePlayerSkill(message.Deserialize<StringData>());
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

        var damage = StateManager.GetAttackDamage(PlayerState);

        // Track damage per player for exp reward
        if(enemy.DamageTracker.Count(x => x.Key == PlayerState.ClientId.ToString()) == 1) {
            enemy.DamageTracker
                .First(x => x.Key == PlayerState.ClientId.ToString()).Value += (int)Math.Round(damage.Sum());
        } else {
            enemy.DamageTracker
                .Add(new KeyValueState(PlayerState.ClientId.ToString(), (int)Math.Round(damage.Sum())));
        }

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

        var damage = StateManager.GetAttackDamage(enemy);

        player.IncomingPhysicalDamage += damage[0];
        player.IncomingFireDamage += damage[1];
        player.IncomingColdDamage += damage[2];

    }

    private void SpawnPlayer()
    {
        PlayerState.TargetLocation = PlayerState.Location = new Vector2(Random.Range(-3, 3), Random.Range(-3, 3));
        PlayerState.isTargetable = false;

        if (StateManager.WorldState.Players.Count(x => x.ClientId == PlayerState.ClientId) == 0)
        {
            StateManager.WorldState.Players.Add(PlayerState);
        }

        BroadcastNetworkMessage(
            NetworkTags.SpawnPlayer,
            new PlayerStateData(PlayerState)
        );

        ServerManager.Instance.StartCoroutine(TargetableCoroutine(PlayerState));
        IEnumerator TargetableCoroutine(PlayerState playerState)
        {
            yield return new WaitForSeconds(30);
            playerState.isTargetable = true;
        }

    }

    private void MovePlayer(Vector2Data targetData)
    {

        Vector2 target = targetData.Target;

        PlayerState.TargetLocation = PlayerState.Location = target;
        PlayerState.isTargetable = true;

        BroadcastNetworkMessage(
            NetworkTags.MovePlayer,
            new MovePlayerData(Client.ID, PlayerState.TargetLocation)
        );
    }

    private void UpdateEnemyLocation(UpdateEnemyLocationData updateEnemyLocationData)
    {
        StateManager.WorldState.GetEnemyState(
            updateEnemyLocationData.EnemyGuid,
            updateEnemyLocationData.SceneName).Location = updateEnemyLocationData.Location;
    }

    private void UpdatePlayerState()
    {
        StateManager.StateCalculator.CalcCharacterState(PlayerState);

        BroadcastNetworkMessage(
            NetworkTags.UpdatePlayerState,
            new PlayerStateData(PlayerState)
        );
    }

    private void CalculatePlayerState()
    {

        StateManager.StateCalculator.CalcCharacterState(PlayerState);

        BroadcastNetworkMessage(
            NetworkTags.CalculatePlayerState,
            new IntegerData(PlayerState.ClientId)
        );
    }

    private void SetPlayerActiveAttack(StringData stringData)
    {

        PlayerState.ActiveAttack = stringData.String;

        StateManager.StateCalculator.CalcCharacterState(PlayerState);

        BroadcastNetworkMessage(
            NetworkTags.SetPlayerActiveAttack,
            new StringIntegerData(stringData.String, PlayerState.ClientId)
        );
    }

    private void SpendAttackPoint(StringData stringData)
    {
        if (PlayerState.AttackPoints > 0)
        {
            PlayerState.AttackPoints -= 1;
            PlayerState.Attacks
                .First(x => x.Key == stringData.String).Value += 1;
            StateManager.StateCalculator.CalcCharacterState(PlayerState);

        }
    }

    private void SpendSkillPoint(StringData stringData)
    {
        if (PlayerState.SkillPoints > 0)
        {
            PlayerState.SkillPoints -= 1;
            PlayerState.Skills
                .First(x => x.Key == stringData.String).Value += 1;
            StateManager.StateCalculator.CalcCharacterState(PlayerState);

        }
    }

    private void SpendPassivePoint(StringData stringData)
    {
        if (PlayerState.PassivePoints > 0)
        {
            PlayerState.PassivePoints -= 1;
            PlayerState.Passives
                .First(x => x.Key == stringData.String).Value += 1;
            StateManager.StateCalculator.CalcCharacterState(PlayerState);

        }
    }

    private void SetPlayerDirection(IntegerData integerData)
    {
        // TODO : Probably need to track this in State if we want to do server side combat hit detection
        BroadcastNetworkMessage(
             NetworkTags.SetPlayerDirection,
             new IntegerPairData(Client.ID, integerData.Integer)
         );
    }

    private void SetPlayerHotbarItem(KeyValueStateData keyValueStateData) 
    {

        if(PlayerState.HotbarItems.Count(x => x.Index == keyValueStateData.KeyValueState.Index) > 0)
        {
            PlayerState.HotbarItems.RemoveAll(x => x.Index == keyValueStateData.KeyValueState.Index);
        }
        PlayerState.HotbarItems.Add(keyValueStateData.KeyValueState);
    }

    public void ActivatePlayerSkill(StringData stringData)
    {
        if(PlayerState.HotbarItems.Count(x => x.Key == stringData.String) > 0)
        {
            StateManager.ActivatePlayerSkill(PlayerState, stringData.String);
        }
    }

    private void SendNetworkMessage(NetworkTags networkTag, IDarkRiftSerializable payload)
    {
        ServerManager.SendNetworkMessage(Client, networkTag, payload);
    }

    private void BroadcastNetworkMessage(NetworkTags networkTag, IDarkRiftSerializable payload)
    {
        ServerManager.BroadcastNetworkMessage(networkTag, payload);
    }

}