using Data.Attacks;
using Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class TradeItem : MonoBehaviour
{
    private Icon _icon;
    public Icon icon
    {
        get
        {
            if (!_icon) { _icon = gameObject.GetComponent<Icon>(); }
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

    ActiveTradeState ActiveTradeState
    {
        get
        {
            return ClientManager.Instance.StateManager.ActiveTrade;
        }
    }

    private static Dictionary<String, Sprite> sprites = new Dictionary<String, Sprite>();

    public Sprite background;

    void Start()
    {
        
    }

    void Update()
    {
        var isRequestingPlayer = ActiveTradeState.RequestingPlayerId == PlayerState.ClientId;
        var equipmentSlot = (InventorySlots)Enum.Parse(typeof(InventorySlots), $"{gameObject.name}");

        IEnumerable<InventoryItemState> tradeItems;
        if(isRequestingPlayer) {
            tradeItems = ActiveTradeState.RecievingOffer.Where(x => x.Slot == equipmentSlot);
        } else {
            tradeItems = ActiveTradeState.RequestingOffer.Where(x => x.Slot == equipmentSlot);
        }

        if (tradeItems.Any())
        {
            var tradeItem = tradeItems.First();

            var TypeKey = tradeItem.Item.ItemType.ToString();

            if (tradeItem.Item.ItemSubType != ItemStateSubType.None)
            {
                TypeKey = tradeItem.Item.ItemSubType.ToString();
            }
            var baseSpriteKey = $"{TypeKey}Sprite{tradeItem.Item.ItemImageId + 1}";

            var raritySpriteKey = tradeItem.Item.ItemRarity + baseSpriteKey;

            if (!sprites.ContainsKey(raritySpriteKey))
            {

                var baseSprite = (Sprite)icon.GetType().GetField(baseSpriteKey).GetValue(icon);
                var replacementColor = (Color)typeof(GameConstants).GetField(tradeItem.Item.ItemRarity + "Color").GetValue(null);
                var newTexture = TextureColorSwapper.SwapColors(baseSprite.texture, GameConstants.SwapColor, replacementColor);
                var coloredSprite = Sprite.Create(newTexture, baseSprite.rect, new Vector2(0, 1));

                sprites.Add(raritySpriteKey, coloredSprite);
            }
            icon.CurrentIcon = sprites.GetValueOrDefault(raritySpriteKey); ;
            icon.IsDraggable = false;
            icon.Type = IconType.Gear;
            icon.ReferenceKey = (int)tradeItem.Slot;
            icon.DropHandled = true;
            icon.DragScale = 0.6f;

        }
        else
        {
            icon.CurrentIcon = background;
            icon.IsDraggable = false;
            icon.Type = IconType.None;
            icon.ReferenceKey = -1;
            icon.DropHandled = true;
        }
    }


}
