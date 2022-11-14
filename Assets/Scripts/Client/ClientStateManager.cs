using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ClientStateManager 
{
    public StateCalculator StateCalculator;
    public WorldState WorldState; // = new WorldState();
    public PlayerState PlayerState
    {
        get
        {
            try
            {
                return WorldState.Players
                    .First(x => x.ClientId == ClientId);
            } catch
            {
                return null;
            }
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

        LoadScene("OverworldScene", LoadSceneMode.Single);
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

    public GameObject GetItemDropObject(System.Guid itemGuid)
    {
        try
        {
            return GameObject.FindGameObjectsWithTag("ItemDrop").ToList().
                First(x => x.GetComponent<ItemDropSprite>().itemGuid == itemGuid);
        }
        catch { return null; }
    }



    public void LoadScene(string scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded += LoadSceneCallback;
        SceneManager.LoadScene(scene, mode);

        void LoadSceneCallback(Scene scene, LoadSceneMode mode)
        {
            if (scene.name.ToLower() != "loginscene")
            {
                foreach (PlayerState player in ClientManager.Instance.StateManager.WorldState.Players)
                {
                    if (player.Scene.ToLower() == scene.name.ToLower())
                    {
                        Handlers.SpawnPlayer(new PlayerStateData(player));
                    }
                }

                foreach (EnemyState enemy in ClientManager.Instance.StateManager.WorldState.Scenes.First(x => x.Name.ToLower() == scene.name.ToLower()).Enemies)
                {
                    Handlers.SpawnEnemy(new EnemyStateData(enemy, scene.name));
                }

                ClientManager.Instance.StateManager.Actions.Spawn();
            }
        }
    }
}
