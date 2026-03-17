using AniDrag.Core;
using System;
using System.Collections.Generic;
using AniDrag.Utility;
using UnityEngine;

namespace AniDrag.Inventory
{
    public class InventoryHolder : MonoBehaviour, IInventoryHolder
    {
        [Header("========================\n" +
                "     Inventory Data     \n" +
                "========================")]
        [SerializeField] private InventoryData _inventoryData = new InventoryData();
        [SerializeField] private int _maxSlots = 20;
        [SerializeField] private GameObject _user; // entity that actually uses items

        public IReadOnlyList<ItemStack> Items => _inventoryData.Items;
        public int MaxSlots => _maxSlots;

        public event Action OnInventoryChanged;
        public event Action<Item, int> OnItemAdded;
        public event Action<Item, int> OnItemRemoved;
        public event Action<Item> OnItemUsed;

        private void Awake()
        {
            if (_user == null)
                _user = gameObject;
        }

        public bool AddItem(Item item, int amount)
        {
            if (item == null || amount <= 0) return false;

            int added = _inventoryData.AddItem(item, amount, _maxSlots);
            if (added > 0)
            {
                Debug.Log($"Added {added} of {item.name}");
                Services.EventBus?.Publish(new ItemPickedEvent
                {
                    Source = gameObject,
                    Item = item,
                    Quantity = added
                });

                OnInventoryChanged?.Invoke();
                OnItemAdded?.Invoke(item, added);
                return true;
            }
            return false;
        }

        public bool RemoveItem(Item item, int amount)
        {
            bool success = _inventoryData.RemoveItem(item, amount);
            if (success)
            {
                OnInventoryChanged?.Invoke();
                OnItemRemoved?.Invoke(item, amount);
            }
            return success;
        }

        public bool UseItemAtIndex(int index)
        {
            if (_user == null)
            {
                Debug.LogError("[InventoryHolder] _user is null!");
                return false;
            }

            bool consumed = _inventoryData.UseItemAtIndex(index, _user);
            if (consumed)
            {
                OnInventoryChanged?.Invoke();
                var items = _inventoryData.Items;
                if (index < items.Count)
                    OnItemUsed?.Invoke(items[index].item);
            }
            return consumed;
        }

        public int GetItemCount(Item item) => _inventoryData.GetItemCount(item);

        // Added to satisfy interface (typo preserved)
        public void ReciveUnequippedItem(Item item) => AddItem(item, 1);

        // Optional: remove by index (used by UI if you want a drop button)
        public bool RemoveItemAtIndex(int index, int amount = 1)
        {
            var items = _inventoryData.Items;
            if (index < 0 || index >= items.Count) return false;
            var stack = items[index];
            return RemoveItem(stack.item, amount);
        }

        [Header("========================\n" +
                "         Debug          \n" +
                "========================")]
        [SerializeField] private List<ItemStack> _debugAddItem;

        [Button("Debug Add Item")]
        private void DebugAddItem()
        {
            if (_debugAddItem.Count == 0) return;
            foreach (var data in _debugAddItem)
                AddItem(data.item, data.amount);
        }

        [Button("Debug Print Items")]
        private void DebugPrintItems()
        {
            foreach (var stack in Items)
                Debug.Log($"{stack.item.name} x{stack.amount}");
        }
    }
}