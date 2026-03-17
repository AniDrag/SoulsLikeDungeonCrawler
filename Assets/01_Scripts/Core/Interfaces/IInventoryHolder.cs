using System;
using System.Collections.Generic;
namespace AniDrag.Core
{
    public interface IInventoryHolder
    {
        IReadOnlyList<ItemStack> Items { get; }
        int MaxSlots { get; }

        bool AddItem(Item item, int amount);
        bool RemoveItem(Item item, int amount);
        bool UseItemAtIndex(int index);
        int GetItemCount(Item item);
        
        void ReciveUnequippedItem(Item item); // typo kept for compatibility
        
        event Action OnInventoryChanged;
        event Action<Item, int> OnItemAdded;   // item, amount added
        event Action<Item, int> OnItemRemoved; // item, amount removed
        event Action<Item> OnItemUsed;
    }
}