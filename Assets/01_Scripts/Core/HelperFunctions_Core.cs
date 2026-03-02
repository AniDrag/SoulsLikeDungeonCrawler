using UnityEngine;
namespace AniDrag.Core
{
    [System.Serializable]
    public enum ConsumableEffectType { None, Health, Strenght, Dexterity, Inteligence, Agility, Stamina }
    [System.Serializable]
    public enum EquipmentType { None, Head, Chest, Arms, Legs, Boots, MainWeapon }
    [System.Serializable]
    public enum ItemType { Generic, Equipment, Consumable, Quest, Crafting }
    [System.Serializable]
    public enum ItemRarity { Common, Uncommon, Rare, Epic, Mythic, Legendary, World }
}