using System.Linq;
using Unity.VisualScripting;
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

    public ClientActions Actions;
    public ClientHandlers Handlers;

    public ClientStateManager(WorldState worldState)
    {       
        WorldState = worldState;

        Actions = new ClientActions();
        Handlers = new ClientHandlers(WorldState, PlayerState);
        ClientManager.Instance.Client.MessageReceived += Handlers.OnNetworkMessage;

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
                    Handlers.SpawnPlayer(new PlayerStateData(player));
                }
            }

            foreach (EnemyState enemy in ClientManager.Instance.StateManager.WorldState.Scenes.First(x => x.Name.ToLower() == scene.name.ToLower()).Enemies)
            {
                Handlers.SpawnEnemy(new EnemyStateData(enemy));
            }

            ClientManager.Instance.StateManager.Actions.RequestSpawn();
        }
    }


}
