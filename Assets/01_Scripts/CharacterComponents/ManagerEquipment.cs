using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using AniDrag.Core;
using AniDrag.Utility; // for [Button] attribute (if used)

namespace AniDrag.CharacterComponents
{
    public class ManagerEquipment : MonoBehaviour, IEquipmentUser
    {
        [Header("=================================\n" +
                "    Equipment Manager Details    \n" +
                "=================================")]
        [SerializeField] private Dictionary<EquipmentType, Item> equipmentSlots = new Dictionary<EquipmentType, Item>();

        [Header("=====================================\n" +
                "    Equipment Starter equipment      \n" +
                "=====================================")]
        public List<Item> startingEquipment = new List<Item>();

        [Header("========================\n" +
                "    Event Connectors    \n" +
                "========================")]
        public UnityEvent<ManagerEquipment> OnEquipmentChanged;
        public UnityEvent<Item> ReturnEquipmentToInventory;
        public UnityEvent<GameObject> OnWeaponSwapped; // changed to UnityEvent for Inspector wiring

        // ------------------------------------------------------------------------------------------------------------------
        // IEquipmentUser Implementation
        // ------------------------------------------------------------------------------------------------------------------

        public void Equip(IEquippable item)
        {
            // Since our items are all derived from Item, we cast and call the main Equip method.
            if (item is Item concreteItem)
                Equip(concreteItem);
            else
                Debug.LogWarning($"Cannot equip {item} – not an Item type.");
        }

        public void Unequip(EquipmentType slotType)
        {
            if (equipmentSlots.ContainsKey(slotType))
            {
                equipmentSlots.Remove(slotType);
                // RecalculateTotalStats(); // if needed
                OnEquipmentChanged?.Invoke(this);
            }
        }

        public IEquippable GetEquipped(EquipmentType slot)
        {
            equipmentSlots.TryGetValue(slot, out var item);
            return item;
        }

        // ------------------------------------------------------------------------------------------------------------------
        // Original Equip(Item) method – kept for backward compatibility
        // ------------------------------------------------------------------------------------------------------------------

        public void Equip(Item pItem)
        {
            if (pItem == null) return;

            EquipmentType slot = pItem.equipmentType;

            if (equipmentSlots.TryGetValue(slot, out var oldItem))
            {
                if (oldItem != null)
                    ReturnEquipmentToInventory?.Invoke(oldItem);
            }

            equipmentSlots[slot] = pItem;

            if (slot == EquipmentType.MainWeapon)
                OnWeaponSwapped?.Invoke(pItem.worldPrefab);

            OnEquipmentChanged?.Invoke(this);
            Debug.Log($"Equipped {pItem.itemName} in slot {slot}");
        }

        // ------------------------------------------------------------------------------------------------------------------
        // Initialization and Helpers
        // ------------------------------------------------------------------------------------------------------------------

        public void Initialized()
        {
            if (startingEquipment.Count > 0)
            {
                foreach (var equipment in startingEquipment)
                {
                    Equip(equipment);
                }
            }
        }

        public Item GetEquippedItem(EquipmentType type)
        {
            equipmentSlots.TryGetValue(type, out var item);
            return item;
        }

        // Optional stats recalculation – uncomment if needed
        // private void RecalculateTotalStats() { ... }

#if UNITY_EDITOR
        [Header("Debug")]
        public Item Debug_Equipment;

        [Button]
        public void Debug_EquipEquipment()
        {
            Equip(Debug_Equipment);
        }

        [Button]
        public void Debug_UnequipEquipment()
        {
            if (Debug_Equipment != null)
                Unequip(Debug_Equipment.equipmentType);
        }

        [Button]
        public void Debug_PrintEquipment()
        {
            Debug.Log("=== Equipment Debug ===");
            foreach (var kvp in equipmentSlots)
            {
                Debug.Log($"{kvp.Key} -> {kvp.Value?.itemName ?? "Empty"}");
            }
            Debug.Log("=======================");
        }
#endif
    }
}