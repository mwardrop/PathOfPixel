using DarkRift;
using DarkRift.Server;
using Data.Characters;
using Extensions;
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
                case NetworkTags.InventoryUpdate:
                    InventoryUpdate(message.Deserialize<IntegerPairData>());
                    break;
                case NetworkTags.InitiateTrade:
                    InitiateTrade(message.Deserialize<IntegerData>());
                    break;
                case NetworkTags.CancelTrade:
                    CancelTrade();
                    break;
                case NetworkTags.AcceptTrade:
                    AcceptTrade();
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

            var scene = StateManager.WorldState.Scenes.First(x => x.Name.ToLower() == "overworldscene");
            var spawn = scene.PlayerSpawns.First();
            var x = Random.Range(spawn.MinBounds.x, spawn.MaxBounds.x);
            var y = Random.Range(spawn.MinBounds.y, spawn.MaxBounds.y);

            PlayerState = StateManager.SpawnPlayer(
                scene, 
                Client.ID, 
                Username, 
                new Vector2(x, y), 
                new Warrior(1, CharacterRarity.Player));
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

    private void ActivatePlayerSkill(StringData stringData)
    {
        if(PlayerState.HotbarItems.Count(x => x.Key == stringData.String) > 0)
        {
            StateManager.ActivatePlayerSkill(PlayerState, stringData.String);
        }
    }

    private void ItemPickedUp(ItemDropData itemDropData)
    {
        if (PlayerState.Inventory.Items.Count() <= GameConstants.InventorySize) {
            StateManager.WorldState.Scenes
                .First(x => x.Name.ToLower() == itemDropData.Scene.ToLower()).ItemDrops
                .RemoveAll(x => x.ItemGuid == itemDropData.ItemState.ItemGuid);

            var freeSlot = StateManager.FindEmptyInventorySlot(PlayerState);
            var newInventoryItem = new InventoryItemState(freeSlot, itemDropData.ItemState);

            PlayerState.Inventory.Items.Add(newInventoryItem);

            BroadcastNetworkMessage(
                NetworkTags.ItemPickedUp,
                new ItemPickupData(
                    newInventoryItem, 
                    Client.ID, 
                    itemDropData.ItemState.ItemGuid, 
                    itemDropData.Scene
                ));
        }

    }

    private void InventoryUpdate(IntegerPairData integerData)
    {

        var sourceSlot = (InventorySlots)integerData.Integer1;
        var destinationSlot = (InventorySlots)integerData.Integer2;

        InventoryItemState sourceItem;

        // Remove source Item from Inventory/Equiped/Trade
        if ((int)sourceSlot < 50 || (int)sourceSlot > 60) // Source is inventory/trade
        {
            sourceItem = PlayerState.Inventory.Items.First(x => x.Slot == sourceSlot);
            PlayerState.Inventory.Items.Remove(sourceItem);
        } else { // Source is equiped item
            sourceItem = PlayerState.Inventory.Equiped.First(x => x.Slot == sourceSlot);
            PlayerState.Inventory.Equiped.Remove(sourceItem);
        }

        if ((int)destinationSlot >= 0) // Do nothing and let item be destroyed if External (-1) destination slot
        {
            // Equiping an item
            if ((int)destinationSlot > 50 && (int)destinationSlot < 60 && (int)sourceSlot < 60)
            {
                if (sourceItem.Item.ItemType.ToString().ToLower() != destinationSlot.ToString().OnlyLetters().ToLower())
                {
                    if ((int)sourceSlot < 50)
                    {
                        PlayerState.Inventory.Items.Add(sourceItem);
                    }
                    else
                    {
                        PlayerState.Inventory.Equiped.Add(sourceItem);
                    }
                    return; // Only allow equiping in correct equipment slot, No State Change
                }

                var existingEquipedItems = PlayerState.Inventory.Equiped.Where(x => x.Slot == destinationSlot);
                if (existingEquipedItems.Count() > 0)
                {
                    // Existing equiped item, swap
                    var existingItem = existingEquipedItems.First();
                    PlayerState.Inventory.Equiped.Remove(existingItem);

                    existingItem.Slot = sourceSlot;
                    PlayerState.Inventory.Items.Add(existingItem);

                }

                sourceItem.Slot = destinationSlot;
                PlayerState.Inventory.Equiped.Add(sourceItem);
            }
            // Unequiping an item
            else if ((int)destinationSlot < 50 && ((int)sourceSlot > 50 && (int)sourceSlot < 60))
            {

                var existingDestinationItems = PlayerState.Inventory.Equiped.Where(x => x.Slot == destinationSlot);
                if (existingDestinationItems.Count() == 0)
                {
                    // Empty Destination Slot, unequip the item
                    sourceItem.Slot = destinationSlot;
                    PlayerState.Inventory.Items.Add(sourceItem);
                }
                else
                {
                    // Filled Destination Slot, try and swap
                    var existingItem = existingDestinationItems.First();
                    if (sourceItem.Item.ItemType == existingItem.Item.ItemType)
                    {
                        // Same equipment type swap
                        existingItem.Slot = sourceSlot;
                        sourceItem.Slot = destinationSlot;

                        PlayerState.Inventory.Items.Remove(existingItem);
                        PlayerState.Inventory.Equiped.Add(existingItem);

                        PlayerState.Inventory.Items.Add(sourceItem);

                    }
                    else
                    {
                        PlayerState.Inventory.Equiped.Add(sourceItem);
                        return; // Only allow swapping of same item types, No State Change
                    }
                }

            }
            // Moving an item
            else if (((int)destinationSlot < 50 || (int)destinationSlot > 60) && ((int)sourceSlot < 50  || (int)sourceSlot > 60))
            {
                var existingDestinationItems = PlayerState.Inventory.Items.Where(x => x.Slot == destinationSlot);
                if (existingDestinationItems.Count() == 0)
                {
                    // Empty destination slot
                    sourceItem.Slot = destinationSlot;
                }
                else
                {
                    // Filled Destination Slot, try and swap
                    var existingItem = existingDestinationItems.First();

                    existingItem.Slot = sourceSlot;
                    sourceItem.Slot = destinationSlot;
                }
                PlayerState.Inventory.Items.Add(sourceItem);

                var existingTrades = StateManager.ActiveTrades.Where(x => x.RequestingPlayerId == PlayerState.ClientId || x.RecievingPlayerId == PlayerState.ClientId);

                if (existingTrades.Any())
                {

                    var currentTrade = existingTrades.First();
                    var isRequestingPlayer = currentTrade.RequestingPlayerId == PlayerState.ClientId;

                    // Add Item to Trade
                    if ((int)destinationSlot > 60 && (int)sourceSlot < 50)
                    {
                        if(isRequestingPlayer) {
                            currentTrade.RequestingOffer.Add(sourceItem);
                        } else {
                            currentTrade.RecievingOffer.Add(sourceItem);
                        }
                    }

                    // Remove Item from Trade
                    if ((int)destinationSlot < 50 && (int)sourceSlot > 60)
                    {
                        if (isRequestingPlayer) {
                            currentTrade.RequestingOffer.Remove(sourceItem);
                        } else {
                            currentTrade.RecievingOffer.Remove(sourceItem);
                        }
                    }

                    currentTrade.RequestingAccepted = false;
                    currentTrade.RecievingAccepted = false;

                    BroadcastNetworkMessage(NetworkTags.UpdateTrade,
                        new TradeData(currentTrade.RequestingPlayerId, currentTrade.RecievingPlayerId, currentTrade));
                }

            } else {
                // No valid inventory change found, put item back
                if ((int)sourceSlot < 50 || (int)sourceSlot > 60) { // Source is inventory/trade
                    PlayerState.Inventory.Items.Add(sourceItem);
                } else { // Source is equiped
                    PlayerState.Inventory.Equiped.Add(sourceItem);
                }
            }

        }

        BroadcastNetworkMessage(NetworkTags.InventoryUpdate,
            new InventoryUpdateData(PlayerState.Inventory, PlayerState.ClientId));

    }

    private void InitiateTrade(IntegerData integerData)
    {
        var existingTrade = StateManager.ActiveTrades.Count(x => x.RequestingPlayerId == PlayerState.ClientId || x.RecievingPlayerId == PlayerState.ClientId);

        if(existingTrade  == 0)
        {
            var trade = new ActiveTradeState(PlayerState.ClientId, integerData.Integer);
            StateManager.ActiveTrades.Add(trade);

            BroadcastNetworkMessage(NetworkTags.InitiateTrade,
                new TradeData(PlayerState.ClientId, integerData.Integer, trade));
        }
    }

    private void CancelTrade()
    {
        var existingTrades = StateManager.ActiveTrades.Where(x => x.RequestingPlayerId == PlayerState.ClientId || x.RecievingPlayerId == PlayerState.ClientId).ToList();
    
        foreach(var trade in existingTrades)
        {
            StateManager.ActiveTrades.Remove(trade);

            var requestingPlayer = StateManager.WorldState.GetPlayerState(trade.RequestingPlayerId);

            foreach(var item in trade.RequestingOffer)
            {
                var freeSlot = StateManager.FindEmptyInventorySlot(requestingPlayer);
                item.Slot = (InventorySlots)freeSlot;
            }

            BroadcastNetworkMessage(NetworkTags.CancelTrade,
                new IntegerData(trade.RequestingPlayerId));

            BroadcastNetworkMessage(NetworkTags.InventoryUpdate,
                new InventoryUpdateData(requestingPlayer.Inventory, trade.RequestingPlayerId));

            if (trade.RecievingPlayerId > -1)
            {
                var recievingPlayer = StateManager.WorldState.GetPlayerState(trade.RecievingPlayerId);

                foreach (var item in trade.RecievingOffer)
                {
                    var freeSlot = StateManager.FindEmptyInventorySlot(recievingPlayer);
                    item.Slot = (InventorySlots)freeSlot;
                }

                BroadcastNetworkMessage(NetworkTags.CancelTrade,
                    new IntegerData(trade.RecievingPlayerId));

                BroadcastNetworkMessage(NetworkTags.InventoryUpdate,
                    new InventoryUpdateData(recievingPlayer.Inventory, trade.RecievingPlayerId));

            }

        }
    
    }

    private void AcceptTrade()
    {
        var existingTrades = StateManager.ActiveTrades.Where(x => x.RequestingPlayerId == PlayerState.ClientId || x.RecievingPlayerId == PlayerState.ClientId).ToList();

        if (existingTrades.Any())
        {
            var currentTrade = existingTrades.First();
            var isRequestingPlayer = currentTrade.RequestingPlayerId == PlayerState.ClientId;
            if(isRequestingPlayer) {
                currentTrade.RequestingAccepted = true;
            } else {
                currentTrade.RecievingAccepted = true;
            }

            BroadcastNetworkMessage(NetworkTags.UpdateTrade,
                new TradeData(currentTrade.RequestingPlayerId, currentTrade.RecievingPlayerId, currentTrade));

            if (currentTrade.RequestingAccepted  == true && currentTrade.RecievingAccepted == true)
            {
                StateManager.ActiveTrades.RemoveAll(x => x.RequestingPlayerId == currentTrade.RequestingPlayerId || x.RecievingPlayerId == currentTrade.RecievingPlayerId);

                var recievingPlayer = StateManager.WorldState.GetPlayerState(currentTrade.RecievingPlayerId);
                var requestingPlayer = StateManager.WorldState.GetPlayerState(currentTrade.RequestingPlayerId);

                foreach(var item in currentTrade.RequestingOffer)
                {
                    var freeSlot = StateManager.FindEmptyInventorySlot(recievingPlayer);
                    item.Slot = (InventorySlots)freeSlot;
                    recievingPlayer.Inventory.Items.Add(item);
                    requestingPlayer.Inventory.Items.Remove(item);
                }

                foreach (var item in currentTrade.RecievingOffer)
                {
                    var freeSlot = StateManager.FindEmptyInventorySlot(requestingPlayer);
                    item.Slot = (InventorySlots)freeSlot;
                    recievingPlayer.Inventory.Items.Remove(item);
                    requestingPlayer.Inventory.Items.Add(item);
                }

                BroadcastNetworkMessage(NetworkTags.InventoryUpdate,
                    new InventoryUpdateData(recievingPlayer.Inventory, recievingPlayer.ClientId));

                BroadcastNetworkMessage(NetworkTags.InventoryUpdate,
                    new InventoryUpdateData(requestingPlayer.Inventory, requestingPlayer.ClientId));

            }
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