

using Data.Characters;
using System.Collections;
using System.Linq;
using UnityEngine;

public class StateUpdater
{
    private ServerStateManager StateManager;

    public StateUpdater(ServerStateManager stateManager)
    {
        StateManager = stateManager;
        ServerManager.Instance.StartCoroutine(OneTickUpdate(StateManager.WorldState));
        ServerManager.Instance.StartCoroutine(OneSecondUpdate(StateManager.WorldState));
        ServerManager.Instance.StartCoroutine(FiveMinuteUpdate(StateManager.WorldState));
    }

    IEnumerator FiveMinuteUpdate(WorldState worldstate)
    {
        while (true)
        {
            foreach (SceneState scene in worldstate.Scenes)
            {
                foreach(EnemySpawnState spawn in scene.EnemySpawns)
                {
                    var spawnedCount = scene.Enemies.Count(x => x.Spawn == spawn.Name);
                    if (spawnedCount < spawn.Minimum) {

                        var numberToSpawn = Random.Range(spawn.Minimum - spawnedCount, spawn.Maximum - spawnedCount);
                        for(var i = 0; i < numberToSpawn; i++)
                        {

                            var x = Random.Range(spawn.MinBounds.x, spawn.MaxBounds.x);
                            var y = Random.Range(spawn.MinBounds.y, spawn.MaxBounds.y);
                            var level = Random.Range(spawn.MinLevel, spawn.MaxLevel);
                            var enemy = CreateInstance.Character(spawn.Character.ToString(), level, spawn.CharacterRarity);

                            StateManager.SpawnEnemy(scene, spawn, $"{spawn.CharacterRarity} {spawn.CharacterName}", new Vector2(x, y), enemy, spawn.ActivateSkills);
                        }

                    }
                }
            }
            yield return new WaitForSeconds(300f);
        }

    }

    IEnumerator OneSecondUpdate(WorldState worldstate)
    {
        while (true)
        {
            foreach (PlayerState player in worldstate.Players)
            {
                if (!player.IsDead)
                {
                    StateManager.ApplyRegenToCharacter(player);

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
                foreach (EnemyState enemy in scene.Enemies)
                {
                    if (!enemy.IsDead)
                    {
                        StateManager.ApplyRegenToCharacter(enemy);

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
            yield return new WaitForSeconds(1.0f);
        }
        
    }

    IEnumerator OneTickUpdate(WorldState worldstate)
    {
        while (true)
        {
            foreach (ActivatedPlayerSkill skill in StateManager.ActivatedPlayerSkills.ToList())
            {
                skill.Update();
            }

            foreach (ActivatedEnemySkill skill in StateManager.ActivatedEnemySkills.ToList())
            {
                skill.Update();
            }

            foreach (PlayerState player in worldstate.Players)
            {
                if (!player.IsDead)
                {
                    StateManager.ApplyIncomingDamageToPlayer(player);
                }
            }
            foreach (SceneState scene in worldstate.Scenes)
            {
                foreach (EnemyState enemy in scene.Enemies)
                {
                    if (!enemy.IsDead)
                    {
                        StateManager.ApplyIncomingDamageToEnemy(enemy, scene);

                        StateManager.SetEnemyTarget(enemy, worldstate.Players, scene);
                    }
                }
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

}



