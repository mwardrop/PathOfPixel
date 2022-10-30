

public static class StateUpdater
{
    public static void Update(WorldState worldstate)
    {
        foreach (PlayerState player in worldstate.Players)
        {
            UpdatePlayer(player);
        }
        foreach (SceneState scene in worldstate.Scenes)
        {
            UpdateScene(scene);
        }
    }

    private static void UpdateScene(SceneState scene)
    {
        foreach (EnemyState enemy in scene.Enemies)
        {
            UpdateEnemy(enemy);
        }
    }

    private static void UpdatePlayer(PlayerState player)
    {
        player.Health -= player.IncomingDamage;
        player.IncomingDamage = 0;
    }

    private static void UpdateEnemy(EnemyState enemy)
    {
        enemy.Health -= enemy.IncomingDamage;
        enemy.IncomingDamage = 0;
    }
}



