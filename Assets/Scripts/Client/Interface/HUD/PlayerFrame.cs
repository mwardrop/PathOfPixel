using Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerFrame: MonoBehaviour, IPointerClickHandler
{

    public IEnumerable<PlayerState> Players
    {
        get
        {
            try { return ClientManager.Instance.StateManager.WorldState.Players.Where(x => x.ClientId != ClientManager.Instance.Client.ID).OrderBy(x => x.ClientId); } catch { return null; }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameObject.name.ToLower() != "localplayer")
        {
            var playerIndex = gameObject.name.OnlyNumbers() - 1;
            var player = Players.ToList()[playerIndex];

            ClientManager.Instance.StateManager.Actions.InitiateTrade(player.ClientId);
        }

    }
}

