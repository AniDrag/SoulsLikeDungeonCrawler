using UnityEngine;
using System;

namespace AniDrag.Core
{
    /// <summary>
    /// Base class for all items. Derive to create specific item types (equipment, consumable, etc.).
    /// </summary>
    public abstract class Item : ScriptableObject
    {
        [Header("Basic Info")]
        public string itemName = "New Item";
        public ItemType itemType = ItemType.Generic;
        public ItemRarity rarity = ItemRarity.Common;
        [TextArea] public string description;
        public Sprite icon;
        public GameObject worldPrefab;

        [Header("Stacking")]
        public bool isStackable = false;
        public int maxStack = 1;
        public bool isUnique = false;

        [Header("Value")]
        public int baseValue = 0;
        public float weight = 0f;

        /// <summary>
        /// Called when the item is used by an owner.
        /// </summary>
        /// <param name="owner">The GameObject using the item.</param>
        /// <returns>True if the item should be consumed (removed from inventory).</returns>
        public abstract bool Use(GameObject owner);
        
        public virtual string GetTooltipText()
        {
            string text = $"<b>{itemName}</b>\n";
            text += $"{description}\n\n";
            text += $"Rarity: {rarity}\n";
            text += $"Value: {baseValue}\n";
            text += $"Weight: {weight:F1}";
            return text;
        }
    }
}