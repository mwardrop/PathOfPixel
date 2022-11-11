using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientStateManager 
{
    public StateCalculator StateCalculator;
    public WorldState WorldState; // = new WorldState();
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

    public ClientActions Actions;
    public ClientHandlers Handlers;

    public ClientStateManager(WorldState worldState)
    {       
        WorldState = worldState;
        StateCalculator = new StateCalculator();
        Actions = new ClientActions(this);
        Handlers = new ClientHandlers(this);

        ClientManager.Instance.Client.MessageReceived += Handlers.OnNetworkMessage;

        LoadScene(PlayerState.Scene, LoadSceneMode.Single);
    }

    public void Update()
    {

    }

    public GameObject GetPlayerGameObject(int clientId)
    {
        try
        {
            if (ClientManager.Instance.Client.ID == clientId)
            {
                return GameObject.FindWithTag("LocalPlayer");
            }
            else
            {
                return GameObject.FindGameObjectsWithTag("NetworkPlayer").ToList()
                    .First(x => x.GetComponent<PlayerSprite>().NetworkClientId == clientId);
            }
        }
        catch { return null; }
    }

    public GameObject GetEnemyGameObject(System.Guid enemyGuid)
    {
        try
        {
            return GameObject.FindGameObjectsWithTag("Enemy").ToList().
                First(x => x.GetComponent<EnemySprite>().StateGuid == enemyGuid);
        }
        catch { return null; }
    }



    private void LoadScene(string scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded += LoadSceneCallback;
        SceneManager.LoadScene(scene, mode);

        void LoadSceneCallback(Scene scene, LoadSceneMode mode)
        {
            foreach (PlayerState player in ClientManager.Instance.StateManager.WorldState.Players)
            {
                if (player.Scene.ToLower() == scene.name.ToLower() && player.ClientId != ClientManager.Instance.StateManager.PlayerState.ClientId)
                {
                    Handlers.SpawnPlayer(new PlayerStateData(player));
                }
            }

            foreach (EnemyState enemy in ClientManager.Instance.StateManager.WorldState.Scenes.First(x => x.Name.ToLower() == scene.name.ToLower()).Enemies)
            {
                Handlers.SpawnEnemy(new EnemyStateData(enemy));
            }

            ClientManager.Instance.StateManager.Actions.Spawn();
        }
    }


}
