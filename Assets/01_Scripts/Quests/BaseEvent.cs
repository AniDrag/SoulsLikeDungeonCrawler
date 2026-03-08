using AniDrag.CharacterComponents;
using AniDrag.Core;
using UnityEngine;

namespace AniDrag.Quest
{
    public class BaseEvent
    {
        // Identity references
        public Entity SourceIdentity;
        public Entity TargetIdentity;

        // Control and metadata
        public bool Consumed = false; // set true to stop propagation
        public int Priority = 0;
        public float TimeCreated = 0f;

        public BaseEvent()
        {
            TimeCreated = Time.time;
        }

        public BaseEvent(Entity source, Entity target)
        {
            SourceIdentity = source;
            TargetIdentity = target;
            TimeCreated = Time.time;
        }
    }


    /// <summary>
    /// Source is who died, target is who killed
    /// </summary>
    public class DeathEvent : BaseEvent
    {
        public int xp;
        public DeathEvent(Entity whoDied, Entity whoKilled = null, int xp = 0)
            : base(whoDied, whoKilled)
        {
            this.xp = xp;
        }
    }
    // Fired when an item is picked up into an inventory.
    // SourceIdentity = picker (the Identity who picked it up)
    // TargetIdentity = optional item-world Identity (null if no Identity)
    public class ItemPickedEvent : BaseEvent
    {
        public Item ItemDef;
        public int Quantity; // how many were added to inventory

        public ItemPickedEvent( Item def, int quantity)
        {
            ItemDef = def;
            Quantity = quantity;
        }
    }
}