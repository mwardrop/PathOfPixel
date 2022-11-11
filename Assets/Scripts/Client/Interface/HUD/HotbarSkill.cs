using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Windows;

public class HotbarSkill : MonoBehaviour, IDropHandler, IPointerClickHandler
{

    public Sprite HotbarSkillNormal;
    public Sprite HotbarSkillActive;
    public Sprite HotbarSkillAttack;

    private IconType IconType;
    private string IconTypeKey;
    private int HotbarIndex;
    private GameObject HotbarIconObject;
    private Icon HotbarIcon;

    private UnityEngine.UI.Image _image;
    public UnityEngine.UI.Image image
    {
        get
        {
            if (!_image) { _image = GetComponent<UnityEngine.UI.Image>(); }
            return _image;
        }
        set { _image = value; }
    }

    public void Awake()
    {
        HotbarIndex = Int32.Parse(string.Concat(name.Where(char.IsNumber)));
        HotbarIconObject = gameObject.transform.parent.Find($"IconSkill{HotbarIndex}").gameObject;
        HotbarIcon = HotbarIconObject.GetComponent<Icon>();
    }

    public void OnDrop(PointerEventData eventData)
    {
                
        Icon dropIcon = eventData.pointerDrag.GetComponent<Icon>();
        IconType = dropIcon.Type;
        IconTypeKey = dropIcon.TypeKey;

        HotbarIcon.CurrentIcon = dropIcon.CurrentIcon;
        HotbarIconObject.SetActive(true);

        ClientManager.Instance.StateManager.Actions.SetPlayerHotbarItem(IconTypeKey, IconType, HotbarIndex);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Activate();
    }

    public void Activate()
    {
        if (IconTypeKey == null) { return; }
        switch (IconType)
        {
            case IconType.Attack:
                ClientManager.Instance.StateManager.Actions.SetPlayerActiveAttack(IconTypeKey);
                for (var i = 0; i < 6; i++)
                {
                    var skill = gameObject.transform.parent.Find($"Skill{i + 1}").gameObject.GetComponent<HotbarSkill>();
                    if (skill.image.sprite == HotbarSkillAttack) { 
                        skill.image.sprite = HotbarSkillNormal; 
                    }
                }
                image.sprite = HotbarSkillAttack;
                break;
            case IconType.Skill:
                ClientManager.Instance.StateManager.Actions.ActivatePlayerSkill(IconTypeKey);
                break;

        }
    }

    public void Update()
    {
        if(IconType == IconType.Skill)
        {
            var playerState = ClientManager.Instance.StateManager.PlayerState;
            if (playerState.ActiveSkills.Count(x => x.Key == IconTypeKey && x.Value == playerState.ClientId) == 1)
            {
                image.sprite = HotbarSkillActive;
            }
            else
            {
                image.sprite = HotbarSkillNormal;
            }
        }
    }

    void Start()
    {
        // TODO: On initial state load, should mimic OnDrop
    }

}
