using AniDrag.CharacterComponents;
using UnityEngine;
namespace AniDrag.AI
{
    /// <summary>
    /// Stores information about each known entity: last known position, threat level, etc.
    /// </summary>
    [System.Serializable]
    public class EntityMemory
    {
        public Entity entity;
        public Vector3 lastKnownPosition;
        public float lastSeenTime;
        public float threatLevel; // can be updated based on damage dealt, etc.
    }
}