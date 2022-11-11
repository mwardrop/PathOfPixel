

using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class StateUpdater
{
    public StateUpdater()
    {
        //ServerManager.Instance.InvokeRepeating("StateManager.StateUpdater.OneSecondTick", 0, 1);
        //ServerManager.Instance.InvokeRepeating("StateManager.StateUpdater.ThirtySecondTick", 0, 30);
    }

    //public void OneSecondTick()
    //{

    //}

    //public void ThirtySecondTick()
    //{

    //}

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
        ApplyIncomingDamageToPlayer(player);

        ApplySkillsToPlayersWithinRadius(player);
    }

    private void UpdateEnemy(EnemyState enemy, SceneState scene, WorldState world)
    {
        ApplyIncomingDamageToEnemy(enemy);

        SetEnemyTarget(enemy, world.Players, scene);

    }

    private void ApplySkillsToPlayersWithinRadius(PlayerState player)
    {
        var activeSkills = player.ActiveSkills.Where(x => x.Value == player.ClientId).Select(x => x.Key);

        foreach(string skillName in activeSkills)
        {
            var skill = CreateInstance.Skill(skillName, player.Skills.First(x => x.Key == skillName).Value);

            foreach(PlayerState otherPlayer in ServerManager.Instance.StateManager.WorldState.Players)
            {
            
                if(otherPlayer.ClientId == player.ClientId) { continue; }

                if(Vector2.Distance(player.Location, otherPlayer.Location) < skill.Radius)
                {
                    if (otherPlayer.ActiveSkills.Count(x => x.Key == skillName && x.Value == player.ClientId) == 0)
                    {
                        otherPlayer.ActiveSkills.Add(new KeyValueState(
                            skillName,
                            player.ClientId));

                        ServerManager.Instance.StateManager.ActivatedPlayerSkills
                            .First(x => x.Skill.GetName() == skillName && x.PlayerState.ClientId == player.ClientId)
                            .PlayersInRadius.Add(otherPlayer.ClientId);

                        ServerManager.SendNetworkMessage(
                            otherPlayer.ClientId,
                            NetworkTags.ActivatePlayerSkill,
                            new StringIntegerData(skillName, player.ClientId));
                    }
                } else
                {
                    if (otherPlayer.ActiveSkills.Count(x => x.Key == skillName && x.Value == player.ClientId) > 0)
                    {
                        otherPlayer.ActiveSkills.RemoveAll(x => x.Key == skillName && x.Value == player.ClientId);

                        ServerManager.Instance.StateManager.ActivatedPlayerSkills
                            .First(x => x.Skill.GetName() == skillName && x.PlayerState.ClientId == player.ClientId)
                            .PlayersInRadius.Remove(otherPlayer.ClientId);

                        ServerManager.SendNetworkMessage(
                            otherPlayer.ClientId,
                            NetworkTags.DeactivatePlayerSkill,
                            new StringIntegerData(skillName, player.ClientId));
                    }
                }
            }
        }
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

    private void ApplyIncomingDamageToPlayer(PlayerState player)
    {
        if (player.IncomingPhysicalDamage > 0 || player.IncomingFireDamage > 0 || player.IncomingColdDamage > 0)
        {

            player.Health -= player.IncomingPhysicalDamage;
            player.Health -= player.IncomingFireDamage;
            player.Health -= player.IncomingColdDamage;

            player.IsDead = player.Health <= 0 ? true : false;

            player.IncomingPhysicalDamage = 0;
            player.IncomingFireDamage = 0;
            player.IncomingColdDamage = 0;

            ServerManager.BroadcastNetworkMessage(
                NetworkTags.PlayerTakeDamage,
                new PlayerTakeDamageData(player.ClientId, player.Health, player.IsDead));
        }

    }

    private void SetEnemyTarget(EnemyState enemy, List<PlayerState> players, SceneState scene)
    {
        if (players.Count > 0)
        {
            Dictionary<int, float> distanceFromPlayers = new Dictionary<int, float>();

            foreach (PlayerState player in players)
            {
                if (player.Scene.ToLower() == scene.Name.ToLower() && player.isTargetable)
                {
                    distanceFromPlayers.Add(player.ClientId, Vector2.Distance(player.Location, enemy.Location));
                }
            }

            if (distanceFromPlayers.Count > 0) { 

                int targetId = distanceFromPlayers.OrderByDescending(x => x.Value).Last().Key;

                if (enemy.TargetPlayerId != targetId)
                {
                    enemy.TargetPlayerId = targetId;

                    ServerManager.BroadcastNetworkMessage(
                        NetworkTags.EnemyNewTarget,
                        new EnemyPlayerPairData(enemy.EnemyGuid, targetId, scene.Name));
                }
            }
        }

    }
}



