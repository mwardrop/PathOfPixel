using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class ClientActions {

    public ClientStateManager StateManager;

    public ClientActions(ClientStateManager stateManager)
    {
        StateManager = stateManager;
    }

    public void Spawn()
    {
        ClientManager.SendNetworkMessage(NetworkTags.SpawnRequest);
    }

    public void Move(Vector2 target)
    {
        ClientManager.SendNetworkMessage(NetworkTags.MoveRequest, new Vector2Data(target));
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
                    StateManager.PlayerState.Scene));
        }
    }

    public void EnemyAttack(EnemySprite enemy)
    {
        if (ClientManager.IsHost)
        {
            ClientManager.SendNetworkMessage(NetworkTags.EnemyAttack, new GuidData(enemy.StateGuid));
        }
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
                    StateManager.PlayerState.Scene));
        }
    }

    public void UpdateEnemyLocation(System.Guid enemyGuid, Vector2 location, string scene)
    {
        StateManager.WorldState
            .GetEnemyStateByGuid(enemyGuid, scene).Location = location;

        if (ClientManager.IsHost)
        {
            ClientManager.SendNetworkMessage(
                NetworkTags.UpdateEnemyLocation,
                new UpdateEnemyLocationData(enemyGuid, location, scene));
        }
    }

    public void UpdatePlayerLocation(int clientId, Vector2 location)
    {

        StateManager.WorldState
            .GetPlayerState(clientId).Location = location;

        if (ClientManager.IsHost)
        {
            ClientManager.SendNetworkMessage(
                NetworkTags.UpdatePlayerLocation,
                new UpdatePlayerLocationData(clientId, location));
        }
    }

    public void UpdatePlayerState()
    {
        ClientManager.SendNetworkMessage(
            NetworkTags.UpdatePlayerState);
    }

    public void CalculatePlayerState()
    {
        ClientManager.SendNetworkMessage(
            NetworkTags.CalculatePlayerState);
    }

    public void SetPlayerActiveAttack(string key) 
    {
        ClientManager.SendNetworkMessage(
            NetworkTags.SetPlayerActiveAttack,
            new StringData(key));
    }

    public void SpendAttackPoint(string name)
    {
        if (StateManager.PlayerState.AttackPoints > 0)
        {
            StateManager.PlayerState.AttackPoints -= 1;
            StateManager.PlayerState.Attacks
                .First(x => x.Key == name).Value += 1;

            StateManager.StateCalculator.CalcCharacterState(StateManager.PlayerState);

            ClientManager.SendNetworkMessage(
                NetworkTags.SpendAttackPoint,
                new StringData(name));
        }
    }

    public void SpendSkillPoint(string name)
    {
        if (StateManager.PlayerState.SkillPoints > 0)
        {
            StateManager.PlayerState.SkillPoints -= 1;
            StateManager.PlayerState.Skills
                .First(x => x.Key == name).Value += 1;

            StateManager.StateCalculator.CalcCharacterState(StateManager.PlayerState);

            ClientManager.SendNetworkMessage(
                NetworkTags.SpendSkillPoint,
                new StringData(name));
        }
    }

    public void SpendPassivePoint(string name)
    {
        if (StateManager.PlayerState.PassivePoints > 0)
        {
            StateManager.PlayerState.PassivePoints -= 1;
            StateManager.PlayerState.Passives
                .First(x => x.Key == name).Value += 1;

            StateManager.StateCalculator.CalcCharacterState(StateManager.PlayerState);

            ClientManager.SendNetworkMessage(
                NetworkTags.SpendPassivePoint,
                new StringData(name));
        }
    }

    public void SetPlayerDirection(SpriteDirection direction)
    {
        ClientManager.SendNetworkMessage(
            NetworkTags.SetPlayerDirection,
            new IntegerData((int)direction));
    }

    public void SetPlayerHotbarItem(string typeKey, IconType type, int index)
    {

        if (StateManager.PlayerState.HotbarItems.Count(x => x.Index == index) > 0)
        {
            StateManager.PlayerState.HotbarItems.RemoveAll(x => x.Index == index);
        }

        var keyValueState = new KeyValueState(typeKey, (int)type) { Index = index };
        StateManager.PlayerState.HotbarItems.Add(keyValueState);

        ClientManager.SendNetworkMessage(
            NetworkTags.SetPlayerHotbarItem,
            new KeyValueStateData(
                keyValueState
            ));
    }

    public void ActivatePlayerSkill(string name)
    {
        ClientManager.SendNetworkMessage(
            NetworkTags.ActivatePlayerSkill,
            new StringData(
               name
            ));
    }

    public void ItemPickedUp(ItemState itemState, string scene)
    {
        ClientManager.SendNetworkMessage(
            NetworkTags.ItemPickedUp,
            new ItemDropData(itemState, scene));
    }

    public void InventoryUpdate(InventorySlots inventorySlot, InventorySlots equipmentSlot)
    {
        ClientManager.SendNetworkMessage
            (NetworkTags.InventoryUpdate, 
            new IntegerPairData((int)inventorySlot, (int)equipmentSlot));
    }
}
