using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDropSprite : BaseSprite
{
    public Guid itemGuid;
    public ItemState ItemDrop;

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

    private static Dictionary<String, Sprite> sprites = new Dictionary<String, Sprite>();

    public static readonly Color SwapColor = new Color(255f / 255f, 0f / 255f, 123f / 255f, 1);
    public static readonly Color CommonColor = new Color(183f / 255f, 183f / 255f, 183f / 255f);
    public static readonly Color MagicColor = new Color(76f / 255f, 110f / 255f, 173f / 255f);
    public static readonly Color RareColor = new Color(209f / 255f, 202f / 255f, 128f / 255f);
    public static readonly Color LegendaryColor = new Color(201f / 255f, 109f / 255f, 69f / 255f);
    public static readonly Color MythicColor = new Color(205f / 255f, 36f / 255f, 36f / 255f);
    public static readonly Color SetColor = new Color(143f / 255f, 208f / 255f, 50f / 255f);

    public void Start()
    {
        if(ItemDrop != null)
        {

            String baseSpriteKey = "";

            switch(ItemDrop.ItemType)
            {
                case ItemStateType.Chest:
                case ItemStateType.Feet:
                case ItemStateType.Head:
                case ItemStateType.Legs:
                    baseSpriteKey = ItemDrop.ItemType.ToString();
                    break;
                case ItemStateType.Weapon:
                case ItemStateType.Other:
                    baseSpriteKey = ItemDrop.ItemSubType.ToString();
                    break;
            }

            if (!String.IsNullOrEmpty(baseSpriteKey))
            {
                baseSpriteKey += "Sprite" + (ItemDrop.ItemImageId + 1).ToString();

                String raritySpriteKey = ItemDrop.ItemRarity + baseSpriteKey;

                if (!sprites.ContainsKey(raritySpriteKey))
                {
                    Sprite baseSprite = (Sprite)this.GetType().GetField(baseSpriteKey).GetValue(this);
                    Color replacementColor = (Color)this.GetType().GetField(ItemDrop.ItemRarity.ToString() + "Color").GetValue(this);
                    Texture2D newTexture = TextureColorSwapper.SwapColors(baseSprite.texture, SwapColor, replacementColor);
                    Sprite coloredSprite = Sprite.Create(newTexture, baseSprite.rect, new Vector2(0, 1));
                    coloredSprite.name = raritySpriteKey;

                    sprites.Add(raritySpriteKey, coloredSprite);
                }

                spriteRenderer.sprite = sprites.GetValueOrDefault(raritySpriteKey);
                spriteRenderer.material.mainTexture = spriteRenderer.sprite.texture;
                var collider = this.GetComponent<BoxCollider2D>();

                collider.size = spriteRenderer.sprite.rect.size / 100;
                collider.offset = new Vector2(
                    (spriteRenderer.sprite.rect.width / 2) / 100, 
                    -(spriteRenderer.sprite.rect.height / 2) / 100);

                transform.Rotate(0,0, UnityEngine.Random.Range(0, 360));

            }
        }

    }

    protected override void Update()
    {
        if(Input.GetMouseButtonDown(0) && this.GetComponent<BoxCollider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        {
            var playerObject = GameObject.FindWithTag("LocalPlayer");
            var playerSprite = playerObject.GetComponent<PlayerSprite>();

            if (Vector2.Distance(GameObject.FindWithTag("LocalPlayer").transform.position, transform.position) <= playerSprite.PickupRadius)
            {
                ClientManager.Instance.StateManager.Actions.ItemPickedUp(ItemDrop, playerSprite.PlayerState.Scene);
            }
        }
    }

}
