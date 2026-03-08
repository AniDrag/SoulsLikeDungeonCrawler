using System.Collections.Generic;
using UnityEngine;
using AniDrag.CharacterComponents;

namespace AniDrag.AI
{
    

    public class AIMemory : MonoBehaviour
    {
        [Header("Settings")]
        public float forgetTime = 10f; // Time after which a memory is forgotten if not seen.

        [Header("Current State")]
        public List<EntityMemory> memories = new List<EntityMemory>();
        public Entity currentTarget; // Public setter/getter

        /// <summary>
        /// Update memories with newly detected entities.
        /// </summary>
        public void MemoryUpdate(List<Entity> newlyDetected)
        {
            // Update existing memories and add new ones
            foreach (var pEntity in newlyDetected)
            {
                var mem = GetMemory(pEntity);
                if (mem == null)
                {
                    mem = new EntityMemory { entity = pEntity };
                    memories.Add(mem);
                }
                mem.lastKnownPosition = pEntity.transform.position;
                mem.lastSeenTime = Time.time;
                // Optionally update threat (could be based on distance, damage, etc.)
            }

            // Remove old memories
            memories.RemoveAll(m => Time.time - m.lastSeenTime > forgetTime);
        }

        /// <summary>
        /// Retrieve memory for a specific entity.
        /// </summary>
        public EntityMemory GetMemory(Entity entity)
        {
            return memories.Find(m => m.entity == entity);
        }

        /// <summary>
        /// Set the current target (called by targeting strategy).
        /// </summary>
        public void SetTarget(Entity target)
        {
            currentTarget = target;
        }
    }
}