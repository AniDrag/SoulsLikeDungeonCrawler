using UnityEngine;
using AniDrag.Core;
using AniDrag.Inventory;

namespace AniDrag.InventoryAndItems
{
    public class InventoryEntry
    {
        public Item item {  get; private set; }
        public UI_InventorySlot UI { get; private set; }
        public int stack;

        public InventoryEntry(Item pItem, UI_InventorySlot pUI, int pStack = 1)
        {
            item = pItem;
            UI = pUI;
            stack = pStack;
        }
    }
}
