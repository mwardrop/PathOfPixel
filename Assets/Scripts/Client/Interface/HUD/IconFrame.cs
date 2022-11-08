using Data.Attacks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class IconFrame : MonoBehaviour, IPointerClickHandler
{

    private Icon _icon;
    public Icon icon
    {
        get
        {
            if (!_icon) { _icon = gameObject.transform.Find("Icon").gameObject.GetComponent<Icon>(); }
            return _icon;
        }
        set { _icon = value; }
    }

    private TextMeshProUGUI _level;
    public TextMeshProUGUI level
    {
        get
        {
            if (!_level) { _level = gameObject.transform.Find("Level").gameObject.GetComponent<TextMeshProUGUI>(); }
            return _level;
        }
        set { _level = value; }
    }

    public void Update()
    {

        switch (icon.Type)
        {
            case IconType.Attack:
                level.text = ClientManager.Instance.StateManager.PlayerState.Attacks.First(x => x.Key == icon.TypeKey).Value.ToString();
                break;
            case IconType.Skill:
                level.text = ClientManager.Instance.StateManager.PlayerState.Skills.First(x => x.Key == icon.TypeKey).Value.ToString();
                break;
            case IconType.Passive:
                level.text = ClientManager.Instance.StateManager.PlayerState.Passives.First(x => x.Key == icon.TypeKey).Value.ToString();
                break;
        }

    }

        public void OnPointerClick(PointerEventData eventData) // 3
    {
        gameObject.transform.Find("Clicked").gameObject.SetActive(true);
        Invoke("ResetButtons", 0.1f);

        switch(icon.Type)
        {
            case IconType.Attack:
                
                if (ClientManager.Instance.StateManager.PlayerState.AttackPoints > 0)
                {
                    ClientManager.Instance.StateManager.PlayerState.AttackPoints -= 1;
                    ClientManager.Instance.StateManager.PlayerState.Attacks
                        .First(x => x.Key == icon.TypeKey).Value += 1;

                    ClientManager.Instance.StateManager.Actions.SpendAttackPoint(icon.TypeKey);
                }
                break;
            case IconType.Skill:
                if (ClientManager.Instance.StateManager.PlayerState.SkillPoints > 0)
                {
                    ClientManager.Instance.StateManager.PlayerState.SkillPoints -= 1;
                    ClientManager.Instance.StateManager.PlayerState.Skills
                        .First(x => x.Key == icon.TypeKey).Value += 1;

                    ClientManager.Instance.StateManager.Actions.SpendSkillPoint(icon.TypeKey);
                }
                break;
            case IconType.Passive:
                if (ClientManager.Instance.StateManager.PlayerState.PassivePoints > 0)
                {
                    ClientManager.Instance.StateManager.PlayerState.PassivePoints -= 1;
                    ClientManager.Instance.StateManager.PlayerState.Passives
                        .First(x => x.Key == icon.TypeKey).Value += 1;

                    ClientManager.Instance.StateManager.Actions.SpendPassivePoint(icon.TypeKey);
                }
                break;
        }
        
    }

    private void ResetButtons()
    {
        gameObject.transform.Find("Clicked").gameObject.SetActive(false);
    }
}
