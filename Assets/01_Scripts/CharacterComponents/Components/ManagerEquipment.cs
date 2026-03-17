using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using AniDrag.Core;
using AniDrag.Utility;

namespace AniDrag.CharacterComponents
{
       public class ManagerEquipment : MonoBehaviour, IEquipmentUser
    {
        [Header("========================\n" +
                "   Starting Equipment    \n" +
                "========================")]
        [SerializeField] private List<Item> _startingEquipment = new List<Item>();

        private Dictionary<EquipmentType, IEquippable> _equippedItems = new Dictionary<EquipmentType, IEquippable>();
        private IInventoryHolder _inventory;
        public event System.Action<IEquipmentUser> OnEquipmentChanged;
        public UnityEvent<ManagerEquipment> OnEquipmentChangedUnity;
        public event System.Action<IEquippable> OnItemUnequipped;
        public UnityEvent<IEquippable> OnItemUnequippedUnity;

        private void Start()
        {
            _inventory = GetComponent<IInventoryHolder>();
            if (_inventory == null)
                Debug.LogWarning("No IInventoryHolder found – unequipped items will not be returned to inventory.");
            foreach (var item in _startingEquipment)
            {
                if (item is IEquippable equippable)
                    Equip(equippable);
            }
        }

        public void Equip(IEquippable item)
        {
            if (item == null) return;
            EquipmentType slot = item.EquipmentType;

            // Unequip any existing item in the same slot
            if (_equippedItems.TryGetValue(slot, out var oldItem))
            {
                _equippedItems.Remove(slot);
                OnItemUnequipped?.Invoke(oldItem);
                OnItemUnequippedUnity?.Invoke(oldItem);

                // Return old item to inventory
                if (oldItem is Item oldItemObj && _inventory != null)
                    _inventory.ReciveUnequippedItem(oldItemObj);
            }

            _equippedItems[slot] = item;
            NotifyEquipmentChanged();
        }

        public void Unequip(EquipmentType slot)
        {
            if (_equippedItems.TryGetValue(slot, out var item))
            {
                _equippedItems.Remove(slot);
                OnItemUnequipped?.Invoke(item);
                OnItemUnequippedUnity?.Invoke(item);

                // Return item to inventory
                if (item is Item itemObj && _inventory != null)
                    _inventory.ReciveUnequippedItem(itemObj);

                NotifyEquipmentChanged();
            }
        }

        public IEquippable GetEquipped(EquipmentType slot)
        {
            _equippedItems.TryGetValue(slot, out var item);
            return item;
        }

        public IReadOnlyDictionary<EquipmentType, IEquippable> GetAllEquipped() => _equippedItems;

        public Item GetEquippedItem(EquipmentType slot) => GetEquipped(slot) as Item;

        public Stats GetTotalStats()
        {
            Stats total = new Stats(0, 0, 0, 0);
            foreach (var kvp in _equippedItems)
                total.Add(kvp.Value.EquipmentStats);
            return total;
        }

        private void NotifyEquipmentChanged()
        {
            OnEquipmentChanged?.Invoke(this);
            OnEquipmentChangedUnity?.Invoke(this);
        }

        [Header("========================\n" +
                "         Debug           \n" +
                "========================")]
        [SerializeField] private Item _debugEquipItem;

        [Button("Debug Equip")]
        private void DebugEquip()
        {
            if (_debugEquipItem != null && _debugEquipItem is IEquippable equippable)
                Equip(equippable);
        }

        [Button("Debug Unequip")]
        private void DebugUnequip()
        {
            if (_debugEquipItem != null && _debugEquipItem is IEquippable equippable)
                Unequip(equippable.EquipmentType);
        }

        [Button("Debug Print Equip")]
        private void DebugPrintEquipped()
        {
            foreach (var kvp in _equippedItems)
                Debug.Log($"{kvp.Key}: {(kvp.Value as Item)?.itemName ?? "Unknown"}");
        }
    }
}