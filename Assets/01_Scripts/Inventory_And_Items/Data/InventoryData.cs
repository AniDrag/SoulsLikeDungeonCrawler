using System.Collections.Generic;
using AniDrag.Core;
using UnityEngine;

namespace AniDrag.Inventory
{
    public class InventoryData
    {
        [SerializeField] private List<ItemStack> _items = new List<ItemStack>();

        public List<ItemStack> Items => _items;

        public void SetItemAtIndex(int index, ItemStack stack)
        {
            if (index >= 0 && index < _items.Count)
                _items[index] = stack;
        }

        public void RemoveItemAtIndex(int index)
        {
            if (index >= 0 && index < _items.Count)
                _items.RemoveAt(index);
        }

        public int AddItem(Item item, int amount, int maxTotalSlots = int.MaxValue)
        {
            // (same as your existing implementation)
            if (item == null || amount <= 0) return 0;
            int remaining = amount;
            int currentSlotCount = _items.Count;

            // try to stack
            if (item.isStackable)
            {
                foreach (var stack in _items)
                {
                    if (stack.item == item && stack.amount < item.maxStack)
                    {
                        int space = item.maxStack - stack.amount;
                        int toAdd = Mathf.Min(space, remaining);
                        int index = _items.IndexOf(stack);
                        _items[index] = new ItemStack(item, stack.amount + toAdd);
                        remaining -= toAdd;
                        if (remaining <= 0) return amount;
                    }
                }
            }

            // check slot limit
            int newStacksNeeded = item.isStackable ? Mathf.CeilToInt((float)remaining / item.maxStack) : remaining;
            if (currentSlotCount + newStacksNeeded > maxTotalSlots)
                return 0;

            // create new stacks
            if (item.isStackable)
            {
                while (remaining > 0)
                {
                    int newStackAmount = Mathf.Min(item.maxStack, remaining);
                    _items.Add(new ItemStack(item, newStackAmount));
                    remaining -= newStackAmount;
                }
            }
            else
            {
                for (int i = 0; i < remaining; i++)
                    _items.Add(new ItemStack(item, 1));
                remaining = 0;
            }

            return amount - remaining;
        }

        public bool RemoveItem(Item item, int amount)
        {
            // (same as your existing implementation)
            if (amount <= 0) return true;
            int toRemove = amount;
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                var stack = _items[i];
                if (stack.item == item)
                {
                    if (stack.amount > toRemove)
                    {
                        _items[i] = new ItemStack(item, stack.amount - toRemove);
                        return true;
                    }
                    else
                    {
                        toRemove -= stack.amount;
                        _items.RemoveAt(i);
                        if (toRemove <= 0) return true;
                    }
                }
            }

            return false;
        }

        public bool UseItemAtIndex(int index, GameObject user)
        {
            if (index < 0 || index >= _items.Count) return false;
            var stack = _items[index];
            bool consumed = stack.item.Use(user);
            if (consumed)
            {
                if (stack.amount > 1)
                    _items[index] = new ItemStack(stack.item, stack.amount - 1);
                else
                    _items.RemoveAt(index);
            }

            return consumed;
        }

        public int GetItemCount(Item item)
        {
            int count = 0;
            foreach (var stack in _items)
                if (stack.item == item)
                    count += stack.amount;
            return count;
        }
    }
}