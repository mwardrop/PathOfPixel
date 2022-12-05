using Data.Attacks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InventoryPanel : BasePanel
{

    public TMP_InputField StandardCurrencyField, PremiumCurrencyField;
    protected override void Start()
    {
        base.Start();

    }

    protected override void Update()
    {
        base.Update();

        StandardCurrencyField.text =
            ClientManager.Instance.StateManager.PlayerState.Inventory.StandardCurrency.ToString();

        PremiumCurrencyField.text =
            ClientManager.Instance.StateManager.PlayerState.Inventory.PremiumCurrency.ToString();

    }
}
