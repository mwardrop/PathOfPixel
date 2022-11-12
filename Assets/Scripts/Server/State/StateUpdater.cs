

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.TextCore.Text;
using static UnityEngine.EventSystems.EventTrigger;

public class StateUpdater
{

    public void OneSecondUpdate(WorldState worldstate)
    {
        foreach(PlayerState player in worldstate.Players)
        {
            if (!player.IsDead)
            {
                ApplyRegenToCharacter(player);

                ServerManager.BroadcastNetworkMessage(
                    NetworkTags.UpdatePlayerRegen,
                    new PlayerRegenData(
                        player.ClientId,
                        player.Health,
                        player.Mana
                    )
                );
            }
        }

        foreach (SceneState scene in worldstate.Scenes)
        {
            foreach(EnemyState enemy in scene.Enemies)
            {
                if (!enemy.IsDead)
                {
                    ApplyRegenToCharacter(enemy);

                    ServerManager.BroadcastNetworkMessage(
                        NetworkTags.UpdateEnemyRegen,
                        new EnemyRegenData(
                            enemy.EnemyGuid,
                            enemy.Health,
                            enemy.Mana,
                            scene.Name
                        )
                    );
                }
            }
        }
        
    }

    public void ApplyRegenToCharacter(ICharacterState character)
    {
        if (character.Health != character.MaxHealth)
        {
            character.Health = Math.Min(character.Health + character.HealthRegen, character.MaxHealth);
        }
        if (character.Mana != character.MaxMana)
        {
            character.Mana = Math.Min(character.Mana + character.ManaRegen, character.MaxMana);
        }
    }
    public void Update(WorldState worldstate)
    {

        foreach(ActivatedPlayerSkill skill in ServerManager.Instance.StateManager.ActivatedPlayerSkills.ToList())
        {
            skill.Update();
        }

        foreach (ActivatedEnemySkill skill in ServerManager.Instance.StateManager.ActivatedEnemySkills.ToList())
        {
            skill.Update();
        }

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
        if (!player.IsDead)
        {
            ApplyIncomingDamageToPlayer(player);
        }
    }

    private void UpdateEnemy(EnemyState enemy, SceneState scene, WorldState world)
    {
        if (!enemy.IsDead)
        {
            ApplyIncomingDamageToEnemy(enemy, scene);

            SetEnemyTarget(enemy, world.Players, scene);
        }
    }

    private void ApplyIncomingDamageToEnemy(EnemyState enemy, SceneState scene)
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
                new EnemyTakeDamageData(enemy.EnemyGuid, enemy.Health, scene.Name, enemy.IsDead));

            if (enemy.IsDead)
            {
                var totalDamage = enemy.DamageTracker.Sum(x => x.Value);
                foreach(KeyValueState playerDamage in enemy.DamageTracker)
                {
                    var player = ServerManager.Instance.StateManager.WorldState.GetPlayerState(Int32.Parse(playerDamage.Key));
                    player.Experience += enemy.Experience * ((playerDamage.Value * 100) / totalDamage) / 100;

                    ServerManager.BroadcastNetworkMessage(
                        NetworkTags.UpdatePlayerExperience,
                        new IntegerPairData(player.ClientId, player.Experience));
                }

                ServerManager.Instance.StartCoroutine(DestroyCoroutine());
                IEnumerator DestroyCoroutine()
                {
                    yield return new WaitForSeconds(60);
                    ServerManager.Instance.StateManager.WorldState.Scenes
                        .First(x => x.Name.ToLower() == scene.Name.ToLower()).Enemies
                        .Remove(enemy);

                }
            }
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



