using Data.Attacks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InventoryPanel : BasePanel
{

    PlayerState PlayerState { 
        get {
            return ClientManager.Instance.StateManager.PlayerState;
        } 
    }

    GameObject InventoryItems;

    protected override void Start()
    {
        base.Start();

        InventoryItems = gameObject.transform
                .Find("Inventory")
                .Find("Viewport")
                .Find("Content")
                .Find("InventoryItems").gameObject;
    }

    protected override void Update()
    {
        base.Update();

        for(var i = 0; i < GameConstants.InventorySize; i++ )
        {
            var icon = InventoryItems.transform
            .Find($"Item{i + 1}")
            .Find("Icon")
            .GetComponent<Icon>();

            var existingState = PlayerState.Inventory.Items.Where(x => (int)x.Slot == i);

            if(existingState.Any())
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

                var baseSprite = (Sprite)icon.GetType().GetField($"{icon.TypeKey}Sprite{state.Item.ItemImageId + 1}").GetValue(icon);
                var replacementColor = (Color)typeof(GameConstants).GetField(state.Item.ItemRarity + "Color").GetValue(null);
                var newTexture = TextureColorSwapper.SwapColors(baseSprite.texture, GameConstants.SwapColor, replacementColor);
                var coloredSprite = Sprite.Create(newTexture, baseSprite.rect, new Vector2(0, 1));

                icon.CurrentIcon = coloredSprite;
                icon.IsDraggable = true;
                icon.Type = IconType.Gear;
                icon.ReferenceKey = (int)state.Slot;
            } else
            {
                icon.CurrentIcon = (Sprite)icon.GetType().GetField($"TransparentIcon").GetValue(icon);
                icon.IsDraggable = false;
                icon.Type = IconType.None;
                icon.ReferenceKey = i;
            }
        }

        //foreach(InventoryItemState state in PlayerState.Inventory.Items)
        //{
        //    var icon = InventoryItems.transform
        //        .Find($"Item{(int)state.Slot + 1}")
        //        .Find("Icon")
        //        .GetComponent<Icon>();

        //    if (state.Item.ItemSubType == ItemStateSubType.None) {
        //        icon.TypeKey = state.Item.ItemType.ToString(); 
        //    } else {
        //        icon.TypeKey = state.Item.ItemSubType.ToString();
        //    }

        //    var baseSprite = (Sprite)icon.GetType().GetField($"{icon.TypeKey}Sprite{state.Item.ItemImageId + 1}").GetValue(icon);
        //    var replacementColor = (Color)typeof(GameConstants).GetField(state.Item.ItemRarity + "Color").GetValue(null);
        //    var newTexture = TextureColorSwapper.SwapColors(baseSprite.texture, GameConstants.SwapColor, replacementColor);
        //    var coloredSprite = Sprite.Create(newTexture, baseSprite.rect, new Vector2(0, 1));

        //    icon.CurrentIcon = coloredSprite;
        //    icon.IsDraggable = true;
        //    icon.Type = IconType.Gear;
        //    icon.ReferenceKey = (int)state.Slot;
        //}
    }
}
