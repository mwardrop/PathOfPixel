using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

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

    private PlayerState PlayerState { get { try { return ClientManager.Instance.StateManager.PlayerState; } catch { return null; } } }

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

        ClientManager.Instance.StateManager.Actions.SetPlayerHotbarItem(dropIcon.TypeKey, dropIcon.Type, HotbarIndex);
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
                break;
            case IconType.Skill:
                ClientManager.Instance.StateManager.Actions.ActivatePlayerSkill(IconTypeKey);
                break;

        }
    }

    public void Update()
    {
        if (PlayerState != null)
        {
            var state = GetHotbarStateItem(HotbarIndex);

            if (state != null)
            {
                if (IconTypeKey != state.Key)
                {
                    IconTypeKey = state.Key;
                    IconType = (IconType)state.Value;

                    switch (IconType)
                    {
                        case IconType.Attack:
                            var attack = CreateInstance.Attack(IconTypeKey, 0);
                            HotbarIcon.CurrentIcon = (Sprite)HotbarIcon.GetType().GetField($"AttackIcon{attack.IconId}").GetValue(HotbarIcon);
                            break;
                        case IconType.Skill:
                            var skill = CreateInstance.Skill(IconTypeKey, 0);
                            HotbarIcon.CurrentIcon = (Sprite)HotbarIcon.GetType().GetField($"SkillIcon{skill.IconId}").GetValue(HotbarIcon);
                            break;
                    }

                    HotbarIcon.Type = IconType;
                    HotbarIcon.TypeKey = IconTypeKey;
                    HotbarIcon.gameObject.SetActive(true);
                }
            }

            if (IconType == IconType.Skill)
            {
                if (PlayerState.ActiveSkills.Count(x => x.Key == IconTypeKey && x.Value == PlayerState.ClientId) == 1)
                {
                    image.sprite = HotbarSkillActive;
                }
                else
                {
                    image.sprite = HotbarSkillNormal;
                }
            }

            if (IconType == IconType.Attack)
            {
                if (IconTypeKey == PlayerState.ActiveAttack)
                {
                    for (var i = 0; i < 6; i++)
                    {
                        var skill = gameObject.transform.parent.Find($"Skill{i + 1}").gameObject.GetComponent<HotbarSkill>();
                        if (skill.image.sprite == HotbarSkillAttack && skill.IconTypeKey != PlayerState.ActiveAttack)
                        {
                            skill.image.sprite = HotbarSkillNormal;
                        }
                    }
                    image.sprite = HotbarSkillAttack;
                }
            }
        }
    }

    private KeyValueState GetHotbarStateItem(int index)
    {
        try {
            return PlayerState.HotbarItems.First(x => x.Index == index);
        } catch {
            return null;
        }
    }

}
