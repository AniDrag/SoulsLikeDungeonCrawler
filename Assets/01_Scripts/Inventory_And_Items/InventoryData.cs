using System.Collections.Generic;
using UnityEngine;
using AniDrag.Core;
namespace AniDrag.InventoryAndItems
{
    [System.Serializable]
    public class InventoryData
    {
        [SerializeField] private List<ItemStack> items = new List<ItemStack>();
        public IReadOnlyList<ItemStack> Items => items;

        /// <summary>
        /// Adds an item to the inventory, handling stacking.
        /// </summary>
        /// <returns>The amount actually added (may be less if inventory full).</returns>
        public int AddItem(Item item, int amount)
        {
            if (item == null || amount <= 0) return 0;

            int remaining = amount;

            if (item.isStackable)
            {
                // Add to existing stacks first
                foreach (var stack in items)
                {
                    if (stack.item == item && stack.amount < item.maxStack)
                    {
                        int space = item.maxStack - stack.amount;
                        int toAdd = Mathf.Min(space, remaining);
                        stack.amount += toAdd;
                        remaining -= toAdd;
                        if (remaining <= 0) return amount; // all added
                    }
                }

                // Create new stacks as needed
                while (remaining > 0)
                {
                    int newStackAmount = Mathf.Min(item.maxStack, remaining);
                    items.Add(new ItemStack(item, newStackAmount));
                    remaining -= newStackAmount;
                }
            }
            else
            {
                // Non?stackable: add each item individually
                for (int i = 0; i < remaining; i++)
                {
                    items.Add(new ItemStack(item, 1));
                }
                remaining = 0;
            }

            return amount - remaining;
        }

        /// <summary>
        /// Removes a specific amount of an item from the inventory.
        /// </summary>
        /// <returns>True if the full amount was removed.</returns>
        public bool RemoveItem(Item item, int amount)
        {
            if (amount <= 0) return true;

            int toRemove = amount;
            for (int i = items.Count - 1; i >= 0; i--)
            {
                var stack = items[i];
                if (stack.item == item)
                {
                    if (stack.amount > toRemove)
                    {
                        stack.amount -= toRemove;
                        return true;
                    }
                    else
                    {
                        toRemove -= stack.amount;
                        items.RemoveAt(i);
                        if (toRemove <= 0) return true;
                    }
                }
            }
            return false; // not enough items
        }

        /// <summary>
        /// Uses an item at the given index. The owner is the GameObject that will receive effects.
        /// </summary>
        /// <returns>True if the item was used and should be consumed.</returns>
        public bool UseItemAtIndex(int index, GameObject owner)
        {
            if (index < 0 || index >= items.Count) return false;

            var stack = items[index];
            if (stack.item.Use(owner))
            {
                stack.amount--;
                if (stack.amount <= 0)
                    items.RemoveAt(index);
                return true;
            }
            return false;
        }
        public int IndexOfStack(ItemStack stack) => items.IndexOf(stack);

    }

}