using Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour,IDropHandler
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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    { 
        var existingState = PlayerState.Inventory.Items.Where(x => x.Slot == (InventorySlots)Enum.Parse(typeof(InventorySlots), $"{gameObject.name}"));

        if (existingState.Any())
        {
            var state = existingState.First();

            if (state.Item.ItemSubType == ItemStateSubType.None)
            {
                icon.TypeKey = state.Item.ItemType.ToString();
            }
            else
            {
                icon.TypeKey = state.Item.ItemSubType.ToString();
            }

            var baseSpriteKey = $"{icon.TypeKey}Sprite{state.Item.ItemImageId + 1}";

            var raritySpriteKey = state.Item.ItemRarity + baseSpriteKey;

            if (!sprites.ContainsKey(raritySpriteKey))
            {
                var baseSprite = (Sprite)icon.GetType().GetField(baseSpriteKey).GetValue(icon);
                var replacementColor = (Color)typeof(GameConstants).GetField(state.Item.ItemRarity + "Color").GetValue(null);
                var newTexture = TextureColorSwapper.SwapColors(baseSprite.texture, GameConstants.SwapColor, replacementColor);
                var coloredSprite = Sprite.Create(newTexture, baseSprite.rect, new Vector2(0, 1));

                sprites.Add(raritySpriteKey, coloredSprite);
            }

            icon.CurrentIcon = sprites.GetValueOrDefault(raritySpriteKey);
            icon.IsDraggable = true;
            icon.Type = IconType.Gear;
            icon.ReferenceKey = (int)state.Slot;
            icon.DropHandled = false;
        }
        else
        {
            icon.CurrentIcon = (Sprite)icon.GetType().GetField($"TransparentIcon").GetValue(icon);
            icon.IsDraggable = false;
            icon.Type = IconType.None;
            icon.ReferenceKey = gameObject.name.OnlyNumbers();
            icon.DropHandled = false;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        var dropIcon = eventData.pointerDrag.GetComponent<Icon>();

        dropIcon.DropHandled = true;

        if (dropIcon.Type == IconType.Gear)
        {
            var sourceSlot = (InventorySlots)eventData.pointerDrag.GetComponent<Icon>().ReferenceKey;

            var destinationSlot = (InventorySlots)Enum.Parse(typeof(InventorySlots), $"{gameObject.name}");

            ClientManager.Instance.StateManager.Actions.InventoryUpdate(sourceSlot, destinationSlot);
        }
    }
}
