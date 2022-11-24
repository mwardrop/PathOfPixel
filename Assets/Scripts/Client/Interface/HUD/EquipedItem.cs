using Data.Attacks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipedItem : MonoBehaviour, IDropHandler
{

    PlayerState PlayerState
    {
        get
        {
            return ClientManager.Instance.StateManager.PlayerState;
        }
    }


    void Start()
    {
        
    }

    void Update()
    {
        var icon = gameObject.transform
            .Find($"Icon")
            .GetComponent<Icon>();

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

            var baseSprite = (Sprite)icon.GetType().GetField($"{TypeKey}Sprite{equipedItem.Item.ItemImageId + 1}").GetValue(icon);
            var replacementColor = (Color)typeof(GameConstants).GetField(equipedItem.Item.ItemRarity + "Color").GetValue(null);
            var newTexture = TextureColorSwapper.SwapColors(baseSprite.texture, GameConstants.SwapColor, replacementColor);
            var coloredSprite = Sprite.Create(newTexture, baseSprite.rect, new Vector2(0, 1));

            icon.CurrentIcon = coloredSprite;
            icon.IsDraggable = true;
            icon.Type = IconType.Gear;
            icon.ReferenceKey = (int)equipedItem.Slot;

        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        var dropIcon = eventData.pointerDrag.GetComponent<Icon>();

        if (dropIcon.Type == IconType.Gear)
        {
            var inventorySlot = (InventorySlots)eventData.pointerDrag.GetComponent<Icon>().ReferenceKey;

            var equipmentSlot = (InventorySlots)Enum.Parse(typeof(InventorySlots), $"{gameObject.name}");

            ClientManager.Instance.StateManager.Actions.InventoryUpdate(inventorySlot, equipmentSlot);
        }
    }

}
