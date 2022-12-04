using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConnectedPlayers : MonoBehaviour
{

    public PlayerState PlayerState { get
        {
            try { return ClientManager.Instance.StateManager.PlayerState; } catch { return null; }
        } 
    }

    public IEnumerable<PlayerState> Players
    {
        get
        {
            try { return ClientManager.Instance.StateManager.WorldState.Players.Where(x => x.ClientId != ClientManager.Instance.Client.ID).OrderBy(x => x.ClientId); } catch { return null; }
        }
    }

    void Update()
    {
        for (var i = 1; i < transform.childCount; i++)
        {
            var networkPlayerFrame = transform.GetChild(i).gameObject;

            if (Players.Count() >= i)
            {
                networkPlayerFrame.SetActive(true);
            }
            else
            {
                networkPlayerFrame.SetActive(false);
            }

        }
    }
}
