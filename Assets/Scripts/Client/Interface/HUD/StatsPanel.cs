using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsPanel : BasePanel
{
    private PlayerState PlayerState { get { return ClientManager.Instance.StateManager.PlayerState; } }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (PlayerState == null) { return; }

        gameObject.transform.Find("TextValues").GetComponent<TextMeshProUGUI>().text = 
        $@"{PlayerState.Level}
           {PlayerState.Experience}         
           {PlayerState.MaxHealth}
           {PlayerState.HealthRegen}s
           {PlayerState.MaxMana}
           {PlayerState.ManaRegen}s
           {PlayerState.PhysicalDamage}
           {PlayerState.FireDamage}
           {PlayerState.ColdDamage}
           {PlayerState.BleedChance}%
           {PlayerState.BurnChance}%
           {PlayerState.FreezeChance}%
           {PlayerState.Armor}
           {PlayerState.FireResistance}%
           {PlayerState.ColdResistance}%
           {PlayerState.Dodge}%
           {PlayerState.Accuracy}%
           {PlayerState.CritChance}%
           {PlayerState.StunChance}%
           {PlayerState.MoveSpeed}%";
    }


}
