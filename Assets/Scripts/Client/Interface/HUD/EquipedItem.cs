using Data.Attacks;
using Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipedItem : MonoBehaviour, IDropHandler
{
    private Icon _icon;
    public Icon icon
    {
        get
        {
            if (!_icon) { _icon = gameObject.transform.Find("Icon").GetComponent<Icon>(); }
            return _icon;
        }
        set { _icon = value; }
    }

    PlayerState PlayerState
    {
        get
        {
            return ClientManager.Instance.StateManager.PlayerState;
        }
    }

    private static Dictionary<String, Sprite> sprites = new Dictionary<String, Sprite>();

    public Sprite background;

    void Start()
    {
        
    }

    void Update()
    {

        var equipmentSlot = (InventorySlots)Enum.Parse(typeof(InventorySlots), $"{gameObject.name}");

        var equipedItems = PlayerState.Inventory.Equiped.Where(x => x.Slot == equipmentSlot);

        if (equipedItems.Any())
        {
            var equipedItem = equipedItems.First();

            var TypeKey = equipedItem.Item.ItemType.ToString();

            if (equipedItem.Item.ItemSubType != ItemStateSubType.None)
            {
                TypeKey = equipedItem.Item.ItemSubType.ToString();
            }
            var baseSpriteKey = $"{TypeKey}Sprite{equipedItem.Item.ItemImageId + 1}";

            var raritySpriteKey = equipedItem.Item.ItemRarity + baseSpriteKey;

            if (!sprites.ContainsKey(raritySpriteKey))
            {

                var baseSprite = (Sprite)icon.GetType().GetField(baseSpriteKey).GetValue(icon);
                var replacementColor = (Color)typeof(GameConstants).GetField(equipedItem.Item.ItemRarity + "Color").GetValue(null);
                var newTexture = TextureColorSwapper.SwapColors(baseSprite.texture, GameConstants.SwapColor, replacementColor);
                var coloredSprite = Sprite.Create(newTexture, baseSprite.rect, new Vector2(0, 1));

                sprites.Add(raritySpriteKey, coloredSprite);
            }
            icon.CurrentIcon = sprites.GetValueOrDefault(raritySpriteKey); ;
            icon.IsDraggable = true;
            icon.Type = IconType.Gear;
            icon.ReferenceKey = (int)equipedItem.Slot;
            icon.DropHandled = true;
            icon.DragScale = 0.6f;

        } else
        {
            icon.CurrentIcon = background;
            icon.IsDraggable = false;
            icon.Type = IconType.None;
            icon.ReferenceKey = -1;
            icon.DropHandled = true;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        var dropIcon = eventData.pointerDrag.GetComponent<Icon>();

        dropIcon.DropHandled = true;

        if (dropIcon.Type == IconType.Gear)
        {
            var inventorySlot = (InventorySlots)eventData.pointerDrag.GetComponent<Icon>().ReferenceKey;

            var equipmentSlot = (InventorySlots)Enum.Parse(typeof(InventorySlots), $"{gameObject.name}");

            ClientManager.Instance.StateManager.Actions.InventoryUpdate(inventorySlot, equipmentSlot);
        }
    }

}
