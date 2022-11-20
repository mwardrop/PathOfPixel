using DarkRift;
using DarkRift.Server;
using Data.Characters;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class ServerConnection
{
    public string Username { get; }
    public string Password { get; }
    public IClient Client { get; set; }

    public PlayerState PlayerState;

    public ServerStateManager StateManager;

    public ServerConnection(string username, string password, ServerStateManager stateManager, PlayerState playerState) {
        Username = username;
        Password = password;
        StateManager = stateManager;
        PlayerState = playerState;
    }

    public ServerConnection(IClient client, string username, string password, ServerStateManager stateManager)
    {

        Client = client;
        Username = username;
        Password = password;
        StateManager = stateManager;

        Client.MessageReceived += OnMessage;

    }

    public ServerConnection Restore(IClient client)
    {
        Client = client;
        PlayerState.ClientId = client.ID;

        Client.MessageReceived += OnMessage;

        return this;
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
                case NetworkTags.UpdatePlayerLocation:
                    UpdatePlayerLocation(message.Deserialize<UpdatePlayerLocationData>());
                    break;
                case NetworkTags.ItemPickedUp:
                    ItemPickedUp(message.Deserialize<ItemDropData>());
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
        EnemyState enemy = StateManager.WorldState.GetEnemyStateByGuid(
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
        EnemyState enemy = StateManager.WorldState.GetEnemyStateByGuid(
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
        if(PlayerState == null) { 
            PlayerState = StateManager.SpawnPlayer(
                "OverworldScene", 
                Client.ID, 
                Username, 
                new Vector2(Random.Range(-3, 3), Random.Range(-3, 3)), 
                new Warrior());
        } else
        {
            PlayerState = StateManager.SpawnPlayer(PlayerState);
        }
    }

    private void MovePlayer(Vector2Data targetData)
    {

        Vector2 target = targetData.Target;

        PlayerState.TargetLocation = target;
        PlayerState.isTargetable = true;

        BroadcastNetworkMessage(
            NetworkTags.MovePlayer,
            new MovePlayerData(Client.ID, PlayerState.TargetLocation)
        );
    }

    private void UpdateEnemyLocation(UpdateEnemyLocationData updateEnemyLocationData)
    {
        StateManager.WorldState.GetEnemyStateByGuid(
            updateEnemyLocationData.EnemyGuid,
            updateEnemyLocationData.SceneName).Location = updateEnemyLocationData.Location;
    }

    private void UpdatePlayerLocation(UpdatePlayerLocationData updatePlayerLocationData)
    {
        StateManager.WorldState.GetPlayerState(
            updatePlayerLocationData.ClientId
        ).Location = updatePlayerLocationData.Location;
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

    public void ItemPickedUp(ItemDropData itemDropData)
    {
        if (PlayerState.Inventory.Items.Count() <= GameConstants.InventorySize) {
            StateManager.WorldState.Scenes
                .First(x => x.Name.ToLower() == itemDropData.Scene.ToLower()).ItemDrops
                .RemoveAll(x => x.ItemGuid == itemDropData.ItemState.ItemGuid);

            var freeSlot = 0;
            foreach(InventoryItemState inventoryItem in PlayerState.Inventory.Items.OrderBy(x => x.Slot))
            {
                if(inventoryItem.Slot != freeSlot)
                {
                    return;
                }
                freeSlot++;
            }

            var newInventoryItem = new InventoryItemState(freeSlot, itemDropData.ItemState);
            PlayerState.Inventory.Items.Add(newInventoryItem);

            SendNetworkMessage(
                NetworkTags.ItemPickedUp,
                new ItemPickupData(
                    newInventoryItem, 
                    Client.ID, 
                    itemDropData.ItemState.ItemGuid, 
                    itemDropData.Scene
                ));
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