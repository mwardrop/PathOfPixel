

using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class StateUpdater
{
    public StateUpdater()
    {
        //managerInstance.InvokeRepeating("Tick", 0, 1);
    }

    public void Tick()
    {

    }

    public void Update(WorldState worldstate)
    {
        foreach (PlayerState player in worldstate.Players)
        {
            UpdatePlayer(player, worldstate);
        }
        foreach (SceneState scene in worldstate.Scenes)
        {
            UpdateScene(scene, worldstate);
        }
    }

    private void UpdateScene(SceneState scene, WorldState world)
    {
        foreach (EnemyState enemy in scene.Enemies)
        {
            UpdateEnemy(enemy, scene, world);
        }
    }

    private void UpdatePlayer(PlayerState player, WorldState world)
    {
        //ApplyIncomingDamageToPlayer(player);
    }

    private void UpdateEnemy(EnemyState enemy, SceneState scene, WorldState world)
    {
        ApplyIncomingDamageToEnemy(enemy);

        SetEnemyTarget(enemy, world.Players, scene);

    }

    private void ApplyIncomingDamageToEnemy(EnemyState enemy)
    {
        if (enemy.IncomingPhysicalDamage > 0 || enemy.IncomingFireDamage > 0 || enemy.IncomingColdDamage > 0) {

            enemy.Health -= enemy.IncomingPhysicalDamage;
            enemy.Health -= enemy.IncomingFireDamage;
            enemy.Health -= enemy.IncomingColdDamage;

            enemy.IsDead = enemy.Health <= 0 ? true : false;

            enemy.IncomingPhysicalDamage = 0;
            enemy.IncomingFireDamage = 0;
            enemy.IncomingColdDamage = 0;

            ServerManager.BroadcastNetworkMessage(
                NetworkTags.EnemyTakeDamage,
                new EnemyTakeDamageData(enemy.EnemyGuid, enemy.Health, enemy.IsDead));
        }

    }

    private void SetEnemyTarget(EnemyState enemy, List<PlayerState> players, SceneState scene)
    {
        if (players.Count > 0)
        {
            Dictionary<int, float> distanceFromPlayers = new Dictionary<int, float>();

            foreach (PlayerState player in players)
            {
                if (player.Scene.ToLower() == scene.Name.ToLower())
                {
                    distanceFromPlayers.Add(player.ClientId, Vector2.Distance(player.Location, enemy.Location));
                }
            }

            int targetId = distanceFromPlayers.OrderByDescending(x => x.Value).Last().Key;

            if (enemy.TargetPlayerId != targetId)
            {
                enemy.TargetPlayerId = targetId;

                ServerManager.BroadcastNetworkMessage(
                    NetworkTags.EnemyNewTarget,
                    new EnemyNewTargetData(enemy.EnemyGuid, targetId, scene.Name));
            }
        }

    }
}



