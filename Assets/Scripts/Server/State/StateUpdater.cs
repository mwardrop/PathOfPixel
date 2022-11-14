

using System.Linq;

public class StateUpdater
{
    private ServerStateManager StateManager;

    public StateUpdater(ServerStateManager stateManager)
    {
        StateManager = stateManager;
    }

    public void OneSecondUpdate(WorldState worldstate)
    {
        foreach(PlayerState player in worldstate.Players)
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
            foreach(EnemyState enemy in scene.Enemies)
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
    }

}



