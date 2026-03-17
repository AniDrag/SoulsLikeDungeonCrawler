using UnityEngine;
using AniDrag.Core;

namespace AniDrag.Inventory
{
    [CreateAssetMenu(fileName = "New Equipment", menuName = "AniDrag/Items/Equipment")]
    public class EquipmentItem : Item, IEquippable
    {
        [Header("Equipment")]
        public EquipmentType equipmentType;
        public Stats equipmentStats = new Stats(); // bonuses when equipped
        public int baseDamage = 0; // for weapons

        // IEquippable implementation
        public EquipmentType EquipmentType => equipmentType;
        public GameObject WorldPrefab => worldPrefab; // could also use base.worldPrefab
        public Stats EquipmentStats => equipmentStats; 
        public override bool Use(GameObject owner)
        {
            Debug.Log($"[EquipmentItem] {itemName} used by {owner.name}");
            var user = owner.GetComponent<IEquipmentUser>();
            if (user != null)
            {
                user.Equip(this);
                Debug.Log($"[EquipmentItem] Equipped {itemName} on {owner.name}");
                return true; // Consumed
            }
            else
            {
                Debug.LogError($"[EquipmentItem] {owner.name} does not implement IEquipmentUser!");
                return false;
            }
        }
        public override string GetTooltipText()
        {
            string text = base.GetTooltipText();
            text += $"\n\n<b>Equipment Stats</b>\n";
            text += $"Name: {itemName}\n";
            text += $"Type: {equipmentType}\n";
            text += $"Rarity: {rarity.ToString()}\n";
            text += $"<b>Stats</b> \n ";
            if (equipmentStats.VIT != 0) text += $"VIT: +{equipmentStats.VIT}\n";
            if (equipmentStats.STR != 0) text += $"STR: +{equipmentStats.STR}\n";
            if (equipmentStats.DEX != 0) text += $"DEX: +{equipmentStats.DEX}\n";
            if (equipmentStats.INT != 0) text += $"INT: +{equipmentStats.INT}\n";
            if (baseDamage > 0) text += $"Base Damage: {baseDamage}";
            text += $"Description:\n {description}\n";
            text += $"Value: {baseValue} \n ";
            text += $"Weight: {weight} \n ";
            return text;
        }
    }
}