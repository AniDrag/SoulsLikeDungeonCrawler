using AniDrag.CharacterComponents;
using System.Linq;
using UnityEngine;

namespace AniDrag.AI
{
    [CreateAssetMenu(menuName = "AniDrag/AI/Targeting/Nearest")]
    public class NearestTargeting : AITargetingStrategy
    {
        public override void UpdateTarget(AIController ai, AIMemory memory)
        {
            Entity nearest = null;
            float minDist = float.MaxValue;
            foreach (var mem in memory.memories)
            {
                float dist = Vector3.Distance(ai.transform.position, mem.lastKnownPosition);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = mem.entity;
                }
            }
            memory.SetTarget(nearest);
        }
    }
}