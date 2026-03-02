using System;
using AniDrag.Core;
namespace AniDrag.InventoryAndItems
{
    public class EventBus_Inventory<T> where T : InventoryEvent
    {
        public event Action<T> OnEvent;

        public void Publish(T evt)
        {
            OnEvent?.Invoke(evt);
        }
    }
    public abstract class InventoryEvent { }

    /// <summary>
    /// Is triggered when picking up an item. and send to the Inventory where we handle the rest.
    /// </summary>
    public class OnItemPickUp : InventoryEvent
    {
        public Item item { get; private set; }
        public int amount { get; private set; }
        public OnItemPickUp(Item pItem, int pAmount)
        {
            item = pItem;

            amount = pAmount;
        }
    }

    public class ItemUseEvent : InventoryEvent
    {
        public Item item { get; private set; }

        public ItemUseEvent(Item pItem) => item = pItem;
    }
}
    