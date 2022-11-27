using DarkRift;
using System.IO;
using System.Net;
using UnityEditor;
using UnityEngine;

public class OnSaveHook : AssetModificationProcessor
{
    public static string[] OnWillSaveAssets(string[] paths)
    {
        // Get the name of the scene to save.
        string scenePath = string.Empty;
        string sceneName = string.Empty;

        foreach (string path in paths)
        {
            if (path.Contains(".unity"))
            {
                scenePath = Path.GetDirectoryName(path);
                sceneName = Path.GetFileNameWithoutExtension(path);
            }
        }

        if (sceneName.Length == 0)
        {
            return paths;
        }

        // do stuff

        var sceneState = new SceneState();
        sceneState.Name = sceneName;

        var playerSpawns = GameObject.FindGameObjectsWithTag("PlayerSpawn");
        foreach (var playerSpawn in playerSpawns)
        {

            var sprite = playerSpawn.GetComponent<SpriteRenderer>();
            var script = playerSpawn.GetComponent<PlayerSpawn>();

            sceneState.PlayerSpawns.Add(new SpawnState(script.Name, sprite.bounds.min, sprite.bounds.max));
        }

        var enemySpawns = GameObject.FindGameObjectsWithTag("EnemySpawn");
        foreach (var enemySpawn in enemySpawns)
        {
            var sprite = enemySpawn.GetComponent<SpriteRenderer>();
            var script = enemySpawn.GetComponent<EnemySpawn>();

            sceneState.EnemySpawns.Add(new EnemySpawnState(
                script.ZoneName, 
                sprite.bounds.min, 
                sprite.bounds.max, 
                script.Character, 
                script.Minimum, 
                script.Maximum,
                script.CharacterRarity,
                string.IsNullOrEmpty(script.CharacterName) ? script.Character.ToString() : script.CharacterName,
                script.MinLevel,
                script.MaxLevel,
                script.ActivateSkills,
                script.ChanceOfChest));
        }

        var server = new DarkRift.Server.DarkRiftServer(new DarkRift.Server.ServerSpawnData(IPAddress.Parse("127.0.0.1"), 9999, IPVersion.IPv4));
        server.StartServer();

        var writer = DarkRiftWriter.Create();
        writer.Write(sceneState);
        var byteArray = writer.ToArray();

        Directory.CreateDirectory($"{Application.persistentDataPath}/Scenes/");

        File.WriteAllBytes($"{Application.persistentDataPath}/Scenes/{sceneName}.dat", byteArray);

        server.Dispose();

        return paths;
    }

}