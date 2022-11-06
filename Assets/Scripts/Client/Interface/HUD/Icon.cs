﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Icon : MonoBehaviour, IDragHandler
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

    public Sprite CurrentIcon;

    private GameObject DragIcon;
    private bool IsDragging = false;

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
        if(Input.GetMouseButtonUp(0) && IsDragging)
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        print("Icon being dragged!");
        if(DragIcon == null)
        {
            DragIcon = CreateInstance.Prefab(this.gameObject, transform.position);
            DragIcon.GetComponent<Icon>().CurrentIcon = CurrentIcon;
            DragIcon.GetComponent<Icon>().IsDragging = true;
            DragIcon.transform.parent = GameObject.FindWithTag("HUD").transform;
        }
        
        DragIcon.transform.position = eventData.position;
        DragIcon.transform.localScale = Vector3.one * 0.2f;

    }

}
