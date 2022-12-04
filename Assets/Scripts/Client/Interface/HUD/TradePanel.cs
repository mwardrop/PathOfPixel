using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TradePanel : BasePanel
{
    public Button AcceptButton;

    public GameObject PlayerAcceptIndicator;
    public GameObject TraderAcceptIndicator;

    public ActiveTradeState ActiveTradeState
    {
        get
        {
            try { return ClientManager.Instance.StateManager.ActiveTrade; } catch { return null; }
        }
    }

    public PlayerState PlayerState
    {
        get
        {
            try { return ClientManager.Instance.StateManager.PlayerState; } catch { return null; }
        }
    }

    protected override void Start()
    {
        CloseButton.onClick.AddListener(CloseClicked);
        AcceptButton.onClick.AddListener(TradeClicked);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        var isRequestingPlayer = ActiveTradeState.RequestingPlayerId == PlayerState.ClientId;

        if (isRequestingPlayer)
        {
            if(ActiveTradeState.RequestingAccepted) {
                PlayerAcceptIndicator.SetActive(true);
            } else {
                PlayerAcceptIndicator.SetActive(false);
            }
            if (ActiveTradeState.RecievingAccepted) {
                TraderAcceptIndicator.SetActive(true);
            } else {
                TraderAcceptIndicator.SetActive(false);
            }
        } else { 
            if (ActiveTradeState.RequestingAccepted) {
                TraderAcceptIndicator.SetActive(true);
            } else {
                TraderAcceptIndicator.SetActive(false);
            }
            if (ActiveTradeState.RecievingAccepted) {
                PlayerAcceptIndicator.SetActive(true);
            } else {
                PlayerAcceptIndicator.SetActive(false);
            }
        }
    }

    private void CloseClicked()
    {
        ClientManager.Instance.StateManager.Actions.CancelTrade();

    }

    private void TradeClicked()
    {
        AcceptButton.transform.Find("Clicked").gameObject.SetActive(true);
        Invoke("ResetTradeClick", 0.1f);
        ClientManager.Instance.StateManager.Actions.AcceptTrade();
    }

    private void ResetTradeClick()
    {
        AcceptButton.transform.Find("Clicked").gameObject.SetActive(false);
    }
}
