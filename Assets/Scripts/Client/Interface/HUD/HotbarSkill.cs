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
    private string HotbarIndex;
    private GameObject HotbarIconObject;
    private Icon HotbarIcon;

    public void Awake()
    {
        HotbarIndex = string.Concat(name.Where(char.IsNumber));
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
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    void Start()
    {
        // TODO: On initial state load, should mimic OnDrop
    }

}
