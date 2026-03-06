using UnityEngine;
using AniDrag.Core;
namespace AniDrag.InventoryAndItems
{
    #region Classes
    public class InventoryEntry
    {
        public Item item {  get; private set; }
        public ItemUI UI { get; private set; }
        public int stack;

        public InventoryEntry(Item pItem, ItemUI pUI, int pStack = 1)
        {
            item = pItem;
            UI = pUI;
            stack = pStack;
        }
    }

    [System.Serializable]
    public class ItemStack
    {
        public Item item;
        public int amount;

        public ItemStack(Item item, int amount)
        {
            this.item = item;
            this.amount = amount;
        }
    }

    [System.Serializable]
    public class DroppableItem
    {
        public Item item { get; private set; }
        public int min { get; private set; }
        public int max { get; private set; }

        public DroppableItem(Item pItem, int pMin, int pMax)
        {
            item = pItem;
            min = pMin;
            max = pMax;
        }
    }


    #endregion
}
