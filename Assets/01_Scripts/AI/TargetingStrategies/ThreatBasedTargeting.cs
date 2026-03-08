using AniDrag.CharacterComponents;
using UnityEngine;

namespace AniDrag.AI
{
    [CreateAssetMenu(menuName = "AniDrag/AI/Targeting/Threat Based")]
    public class ThreatBasedTargeting : AITargetingStrategy
    {
        public override void UpdateTarget(AIController ai, AIMemory memory)
        {
            Entity highestThreat = null;
            float maxThreat = float.MinValue;
            foreach (var mem in memory.memories)
            {
                if (mem.threatLevel > maxThreat)
                {
                    maxThreat = mem.threatLevel;
                    highestThreat = mem.entity;
                }
            }
            memory.SetTarget(highestThreat);
        }
    }
}