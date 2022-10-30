using DarkRift;
using DarkRift.Client;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientStateManager 
{
    public ClientActions Actions = new ClientActions();

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

        LoadScene(PlayerState.Scene, LoadSceneMode.Single);
    }

    public void Update()
    {

    }

    private void LoadScene(string scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded += LoadSceneCallback;
        SceneManager.LoadScene(scene, mode);

        void LoadSceneCallback(Scene scene, LoadSceneMode mode)
        {
            foreach (PlayerState player in ClientManager.Instance.StateManager.WorldState.Players)
            {
                if (player.Scene == scene.name && player.ClientId != ClientManager.Instance.StateManager.PlayerState.ClientId)
                {
                    SpawnPlayer(new PlayerStateData(player));
                }
            }

            foreach (EnemyState enemy in ClientManager.Instance.StateManager.WorldState.Scenes.First(x => x.Name.ToLower() == scene.name.ToLower()).Enemies)
            {
                SpawnEnemy(new EnemyStateData(enemy));
            }

            ClientManager.Instance.StateManager.Actions.RequestSpawn();
        }
    }

    private void OnNetworkMessage(object sender, MessageReceivedEventArgs e)
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
                case NetworkTags.EnemyTakeDamage:
                    EnemyTakeDamage(message.Deserialize<EnemyTakeDamageData>());
                    break;
                case NetworkTags.PlayerTakeDamage:
                    PlayerTakeDamage(message.Deserialize<PlayerTakeDamageData>());
                    break;
                case NetworkTags.PlayerAttack:
                    PlayerAttack(message.Deserialize<IntegerData>());
                    break;
            }
        }
    }

    private void SpawnPlayer(PlayerStateData playerStateData)
    {

        PlayerState playerState = playerStateData.PlayerState;

        if (WorldState.Players.Where(x=> x.ClientId == playerState.ClientId).Count() == 0)
        {
            WorldState.Players.Add(playerState);
        }

        GameObject newPlayer = UnityEngine.Object.Instantiate(
            ClientManager.Prefabs.WarriorSprite,
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

        GameObject prefab = ClientManager.Prefabs.PossessedSprite;

        switch (enemyState.Type)
        {
            case EnemyType.Possessed:
                prefab = ClientManager.Prefabs.PossessedSprite;
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

    private void EnemyTakeDamage(EnemyTakeDamageData enemyTakeDamageData)
    {
        WorldState
            .Scenes.First(x => x.Name == PlayerState.Scene)
            .Enemies.First(x => x.EnemyGuid == enemyTakeDamageData.EnemyGuid)
            .IncomingDamage += enemyTakeDamageData.Damage;
    }

    private void PlayerTakeDamage(PlayerTakeDamageData playerTakeDamageData)
    {
        WorldState
            .Players.First(x => x.ClientId == playerTakeDamageData.ClientId)
            .IncomingDamage += playerTakeDamageData.Damage;
    }

    private void PlayerAttack(IntegerData integerData)
    {
        int clientId = integerData.Integer;

        if (clientId != ClientId) {
            List<GameObject> networkPlayers = GameObject.FindGameObjectsWithTag("NetworkPlayer").ToList();
            GameObject attackingPlayer = networkPlayers.First(x => x.GetComponent<PlayerSprite>().NetworkClientId == integerData.Integer);
            attackingPlayer.GetComponent<CharacterSprite>().SetState(SpriteState.Attack1);
        }
    }


}
