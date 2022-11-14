using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public enum IconType
{
    Attack,
    Skill,
    Passive,
    Item,
    Gear
}

public class Icon : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{

    public Sprite AttackIcon1;
    public Sprite AttackIcon2;
    public Sprite AttackIcon3;
    public Sprite AttackIcon4;
    public Sprite AttackIcon5;
    public Sprite AttackIcon6;
    public Sprite AttackIcon7;
    public Sprite AttackIcon8;
    public Sprite AttackIcon9;
    public Sprite AttackIcon10;
    public Sprite AttackIcon11;
    public Sprite AttackIcon12;
    public Sprite AttackIcon13;
    public Sprite AttackIcon14;
    public Sprite AttackIcon15;
    public Sprite AttackIcon16;
    public Sprite AttackIcon17;
    public Sprite AttackIcon18;
    public Sprite AttackIcon19;
    public Sprite AttackIcon20;
    public Sprite AttackIcon21;
    public Sprite AttackIcon22;
    public Sprite AttackIcon23;
    public Sprite AttackIcon24;
    public Sprite AttackIcon25;
    public Sprite AttackIcon26;
    public Sprite AttackIcon27;
    public Sprite AttackIcon28;
    public Sprite AttackIcon29;
    public Sprite AttackIcon30;
    public Sprite AttackIcon31;
    public Sprite AttackIcon32;
    public Sprite AttackIcon33;
    public Sprite AttackIcon34;
    public Sprite AttackIcon35;
    public Sprite AttackIcon36;

    public Sprite SkillIcon1;
    public Sprite SkillIcon2;
    public Sprite SkillIcon3;
    public Sprite SkillIcon4;
    public Sprite SkillIcon5;
    public Sprite SkillIcon6;
    public Sprite SkillIcon7;
    public Sprite SkillIcon8;
    public Sprite SkillIcon9;
    public Sprite SkillIcon10;
    public Sprite SkillIcon11;
    public Sprite SkillIcon12;
    public Sprite SkillIcon13;
    public Sprite SkillIcon14;
    public Sprite SkillIcon15;
    public Sprite SkillIcon16;
    public Sprite SkillIcon17;
    public Sprite SkillIcon18;
    public Sprite SkillIcon19;
    public Sprite SkillIcon20;
    public Sprite SkillIcon21;
    public Sprite SkillIcon22;
    public Sprite SkillIcon23;
    public Sprite SkillIcon24;
    public Sprite SkillIcon25;
    public Sprite SkillIcon26;
    public Sprite SkillIcon27;
    public Sprite SkillIcon28;
    public Sprite SkillIcon29;
    public Sprite SkillIcon30;
    public Sprite SkillIcon31;
    public Sprite SkillIcon32;
    public Sprite SkillIcon33;
    public Sprite SkillIcon34;
    public Sprite SkillIcon35;
    public Sprite SkillIcon36;
    public Sprite SkillIcon37;

    public Sprite PassiveIcon1;
    public Sprite PassiveIcon2;
    public Sprite PassiveIcon3;
    public Sprite PassiveIcon4;
    public Sprite PassiveIcon5;
    public Sprite PassiveIcon6;
    public Sprite PassiveIcon7;
    public Sprite PassiveIcon8;
    public Sprite PassiveIcon9;
    public Sprite PassiveIcon10;
    public Sprite PassiveIcon11;
    public Sprite PassiveIcon12;
    public Sprite PassiveIcon13;
    public Sprite PassiveIcon14;
    public Sprite PassiveIcon15;
    public Sprite PassiveIcon16;
    public Sprite PassiveIcon17;
    public Sprite PassiveIcon18;
    public Sprite PassiveIcon19;
    public Sprite PassiveIcon20;
    public Sprite PassiveIcon21;
    public Sprite PassiveIcon22;
    public Sprite PassiveIcon23;
    public Sprite PassiveIcon24;
    public Sprite PassiveIcon25;
    public Sprite PassiveIcon26;
    public Sprite PassiveIcon27;
    public Sprite PassiveIcon28;
    public Sprite PassiveIcon29;
    public Sprite PassiveIcon30;
    public Sprite PassiveIcon31;
    public Sprite PassiveIcon32;
    public Sprite PassiveIcon33;
    public Sprite PassiveIcon34;
    public Sprite PassiveIcon35;
    public Sprite PassiveIcon36;
    public Sprite PassiveIcon37;
    public Sprite PassiveIcon38;

    public Sprite ChestSprite1;
    public Sprite ChestSprite2;
    public Sprite ChestSprite3;
    public Sprite ChestSprite4;
    public Sprite ChestSprite5;
    public Sprite ChestSprite6;
    public Sprite ChestSprite7;
    public Sprite ChestSprite8;

    public Sprite FeetSprite1;
    public Sprite FeetSprite2;
    public Sprite FeetSprite3;
    public Sprite FeetSprite4;

    public Sprite HeadSprite1;
    public Sprite HeadSprite2;
    public Sprite HeadSprite3;
    public Sprite HeadSprite4;
    public Sprite HeadSprite5;

    public Sprite LegsSprite1;
    public Sprite LegsSprite2;
    public Sprite LegsSprite3;
    public Sprite LegsSprite4;
    public Sprite LegsSprite5;

    public Sprite OtherAmuletSprite1;
    public Sprite OtherAmuletSprite2;
    public Sprite OtherAmuletSprite3;

    public Sprite OtherRingSprite1;
    public Sprite OtherRingSprite2;
    public Sprite OtherRingSprite3;

    public Sprite OtherGlovesSprite1;

    public Sprite WeaponSwordSprite1;
    public Sprite WeaponSwordSprite2;
    public Sprite WeaponSwordSprite3;
    public Sprite WeaponSwordSprite4;
    public Sprite WeaponSwordSprite5;
    public Sprite WeaponSwordSprite6;

    public Sprite WeaponAxeSprite1;
    public Sprite WeaponAxeSprite2;
    public Sprite WeaponAxeSprite3;

    public Sprite WeaponStaffSprite1;
    public Sprite WeaponStaffSprite2;
    public Sprite WeaponStaffSprite3;

    public Sprite WeaponHammerSprite1;

    public Sprite WeaponFishingRodSprite1;

    public Sprite InventorySlotWeapon;
    public Sprite InventorySlotChest;
    public Sprite InventorySlotFeet;
    public Sprite InventorySlotHead;
    public Sprite InventorySlotLegs;
    public Sprite InventorySlotOther;

    public Sprite CurrentIcon;
    public Sprite TransparentIcon;

    private GameObject DragIconObject;
    private bool IsDragging = false;
    public bool IsDraggable = false;
    public IconType Type;
    public string TypeKey;

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

    public void Update()
    {
        image.sprite = CurrentIcon;

        if(IsDraggable && IsDragging && Input.GetMouseButtonUp(0))
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    private CanvasGroup canvasGroup;

    public void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IsDraggable)
        {
            if (DragIconObject == null)
            {             
                DragIconObject = CreateInstance.Prefab(this.gameObject, transform.position);
                DragIconObject.transform.SetParent(GameObject.FindWithTag("HUD").transform);
                DragIconObject.GetComponent<CanvasGroup>().alpha = 0.6f;
                DragIconObject.transform.localScale = Vector3.one * 0.2f;

                Icon dragIcon = DragIconObject.GetComponent<Icon>();
                dragIcon.CurrentIcon = CurrentIcon;
                dragIcon.IsDragging = true;
                dragIcon.Type = Type;
                dragIcon.TypeKey = TypeKey;
            }
            
            DragIconObject.transform.position = eventData.position;           
        }

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsDraggable)
        {
            canvasGroup.blocksRaycasts = false;
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsDraggable)
        {
            canvasGroup.blocksRaycasts = true;
        }
    }
}

