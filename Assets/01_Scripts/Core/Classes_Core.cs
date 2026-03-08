using System;
using UnityEngine;

namespace AniDrag.Core
{
    [CreateAssetMenu(menuName = "AniDrag/Item/SimpleItem", fileName = "Item")]
    [System.Serializable]
    public class Item : ScriptableObject, IEquippable
    {
        [Header("Basic Info")]
        public string IDitem = string.Empty;
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

        [Header("Consumable")]
        public bool isNegative = false;
        public ConsumableEffectType effect = ConsumableEffectType.None;
        public int effectValue = 0;
        public float effectTime = 0f;

        [Header("Equipment")]
        public int baseDamage = 0;
        public Stats stats = new Stats();
        public EquipmentType equipmentType = EquipmentType.None;

        // IEquippable implementation
        public EquipmentType EquipmentType => equipmentType;
        public GameObject WorldPrefab => worldPrefab;

        public virtual bool Use(GameObject owner)
        {
            Debug.Log($"Item: {itemName} used by {owner.name}");
            return true;
        }
    }

    [System.Serializable]
    public class Stats
    {
        public int VIT;
        public int STR;
        public int DEX;
        public int INT;

        public Stats(int vit = 1, int str = 1, int dex = 1, int intel = 1)
        {
            VIT = vit; STR = str; DEX = dex; INT = intel;
        }

        public Stats(Stats other)
        {
            VIT = other.VIT; STR = other.STR; DEX = other.DEX; INT = other.INT;
        }

        public void Add(Stats other)
        {
            VIT += other.VIT; STR += other.STR; DEX += other.DEX; INT += other.INT;
        }

        public void Reset() => VIT = STR = DEX = INT = 0;
    }

    /// <summary>
    ///  Call .Evaluate to see if condition is true.
    /// </summary>
    public class FuncPredicate : IPredicate
    {
        readonly Func<bool> func;

        public FuncPredicate(Func<bool> pFunc)
        {
            func = pFunc;
        }

        public bool Evaluate() => func.Invoke();
    }
}