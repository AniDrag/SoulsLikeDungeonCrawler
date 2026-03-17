using UnityEngine;

namespace AniDrag.Core
{
    public abstract class BaseEvent
    {
        public GameObject Source { get; set; }
        public GameObject Target { get; set; }
        public bool Consumed { get; set; }
        public float TimeCreated { get; } = Time.time;
    }
}