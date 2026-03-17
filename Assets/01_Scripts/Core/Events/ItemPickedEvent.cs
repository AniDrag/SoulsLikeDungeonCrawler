namespace AniDrag.Core
{
    public class ItemPickedEvent : BaseEvent
    {
        public Item Item { get; set; }
        public int Quantity { get; set; }
    }
}