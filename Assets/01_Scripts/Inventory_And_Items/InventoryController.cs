using AniDrag.Core;
using AniDrag.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace AniDrag.InventoryAndItems
{
    public class InventoryController : MonoBehaviour
    {
        [Header("========================\n" +
                "     Inventory Data     \n" +
                "========================")]
        [SerializeField] private List<ItemStack> startingItems = new List<ItemStack>();
        private InventoryData inventory = new InventoryData();
        [SerializeField] private PlayerInput inputs;

        [Header("========================\n" +
                "         Events         \n" +
                "========================")]
        public UnityEvent OnInventoryChanged; // UI listens to this
        public Action EnableDisableInventory;

        [Header("========================\n" +
                "          Debug         \n" +
                "========================")]
        [SerializeField] List<ItemStack> Debug_Item;

        // The owner (usually this game object) that receives item effects.
        // Could be set to a different object if needed (e.g., player uses item on target).
        private GameObject effectReceiver;

        private void Awake()
        {
            effectReceiver = gameObject;
            foreach (var ite in startingItems)
                AddItem(ite.item, ite.amount);
        }
        void Update()
        {
        
            if (inputs.actions["Inventory"].triggered)
            {
                EnableDisableInventory?.Invoke();
            }
        }

        /// <summary>
        /// Set a different receiver for item effects (e.g., if inventory belongs to a container).
        /// </summary>
        public void SetEffectReceiver(GameObject receiver) => effectReceiver = receiver;

        /// <summary>
        /// Add an item to the inventory.
        /// </summary>
        /// <returns>True if at least one item was added.</returns>
        public bool AddItem(Item item, int amount)
        {
            int added = inventory.AddItem(item, amount);
            if (added > 0)
            {
                OnInventoryChanged?.Invoke();
                return true;
            }
            return false;
        }
        /// <summary>
        /// get the unequipped item back in inventory. Connect to EquipmentManager.Return Equipment item event.
        /// </summary>
        /// <param name="item"></param>
        public void ReceiveUnequippedItem(Item item)
        {
            AddItem(item, 1);
        }

        /// <summary>
        /// Remove an item from the inventory.
        /// </summary>
        /// <returns>True if the full amount was removed.</returns>
        public bool RemoveItem(Item item, int amount)
        {
            bool success = inventory.RemoveItem(item, amount);
            if (success) OnInventoryChanged?.Invoke();
            return success;
        }

        /// <summary>
        /// Use an item at the given index. Called by UI.
        /// </summary>
        public bool UseItemAtIndex(int index)
        {
            bool used = inventory.UseItemAtIndex(index, effectReceiver);
            if (used) OnInventoryChanged?.Invoke();
            return used;
        }

        /// <summary>
        /// Get read?only list of current item stacks.
        /// </summary>
        public IReadOnlyList<ItemStack> GetItems() => inventory.Items;

        [Button]
        void AddItemToInventory()
        {
            foreach (var ite in Debug_Item)
                AddItem(ite.item, ite.amount);
        }

       
    }
}