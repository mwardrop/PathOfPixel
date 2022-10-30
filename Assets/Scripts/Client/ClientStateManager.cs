using DarkRift;
using DarkRift.Client;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientStateManager 
{

    public WorldState WorldState = new WorldState();
    public PlayerState PlayerState
    {
        get
        {
            return WorldState.Players
                .First(x => x.ClientId == ClientId);
        }
    }
    public int ClientId
    {
        get
        {
            return ClientManager.Instance.Client.ID;
        }
    }

    public ClientStateManager(WorldState worldState)
    {
        ClientManager.Instance.Client.MessageReceived += OnNetworkMessage;

        WorldState = worldState;

        LoadScene(PlayerState.Scene);
    }

    public void OnNetworkMessage(object sender, MessageReceivedEventArgs e)
    {
        using (Message message = e.GetMessage())
        {
            switch ((NetworkTags)message.Tag)
            {
                case NetworkTags.SpawnPlayer:
                    SpawnPlayer(message.Deserialize<PlayerStateData>());
                    break;
                case NetworkTags.MovePlayer:
                    MovePlayer(message.Deserialize<MovePlayerData>());
                    break;
                case NetworkTags.PlayerDisconnect:
                    PlayerDisconnect(message.Deserialize<IntegerData>());
                    break;
                case NetworkTags.SpawnEnemy:
                    SpawnEnemy(message.Deserialize<EnemyStateData>());
                    break;
            }
        }
    }

    public void RequestSpawn()
    {
        ClientManager.SendNetworkMessage(NetworkTags.SpawnRequest);
    }

    public void RequestMove(Vector2 target)
    {
        ClientManager.SendNetworkMessage(NetworkTags.MoveRequest, new TargetData(target));
    }

    private void SpawnPlayer(PlayerStateData playerStateData)
    {

        PlayerState playerState = playerStateData.PlayerState;

        if (WorldState.Players.Where(x=> x.ClientId == playerState.ClientId).Count() == 0)
        {
            WorldState.Players.Add(playerState);
        }

        GameObject newPlayer = UnityEngine.Object.Instantiate(
            ClientManager.ClientPrefabs.WarriorSprite,
            playerState.Location,
            Quaternion.identity);

        if(playerState.ClientId == ClientId) {
            newPlayer.tag = "LocalPlayer";
        } else {
            newPlayer.tag = "NetworkPlayer";
        }

        newPlayer.GetComponent<PlayerSprite>().NetworkClientId = playerState.ClientId;

    }

    private void SpawnEnemy(EnemyStateData enemyStateData)
    {
        EnemyState enemyState = enemyStateData.EnemyState;

        GameObject prefab = ClientManager.ClientPrefabs.PossessedSprite;

        switch (enemyState.Type)
        {
            case EnemyType.Possessed:
                prefab = ClientManager.ClientPrefabs.PossessedSprite;
                break;
        }
        
        GameObject newEnemy = UnityEngine.Object.Instantiate(
            prefab,
            enemyState.Location,
            Quaternion.identity);

        newEnemy.GetComponent<EnemySprite>().StateGuid = enemyState.EnemyGuid;

    }

    private void MovePlayer(MovePlayerData movePlayerData)
    {
        WorldState.Players.First(x => x.ClientId == movePlayerData.ClientId).TargetLocation = movePlayerData.Target;
    }

    private void PlayerDisconnect(IntegerData clientId)
    {
        WorldState.Players.RemoveAll(x => x.ClientId == clientId.Integer);
    }

    private void LoadScene(string scene, LoadSceneMode mode = LoadSceneMode.Single)
    {
        SceneManager.sceneLoaded += LoadSceneCallback;
        SceneManager.LoadScene(scene, mode);

        void LoadSceneCallback(Scene scene, LoadSceneMode mode)
        {
            foreach (PlayerState player in WorldState.Players)
            {
                if (player.Scene == scene.name && player.ClientId != PlayerState.ClientId)
                {
                    SpawnPlayer(new PlayerStateData(player));
                }
            }

            foreach (EnemyState enemy in WorldState.Scenes.First(x => x.Name.ToLower() == scene.name.ToLower()).Enemies)
            {
                SpawnEnemy(new EnemyStateData(enemy));
            }

            RequestSpawn();
        }
    }
}
