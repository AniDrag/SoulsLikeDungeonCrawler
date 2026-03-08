using UnityEngine;
using System.Linq;
namespace AniDrag.AI
{
    [CreateAssetMenu(menuName = "AI/Targeting/HighestThreat")]
    public class Targeting_HighestThreat : AICore_TargetingStrategy
    {
        public override Transform UpdateTarget(AICore_Controller ai)
        {
            if (ai.sense.detectedTargets.Count == 0)
                return null;

            // Example: pick the entity with lowest health (or define a custom threat property)
            var highestThreat = ai.sense.detectedTargets
                .OrderBy(e => e.entityHealth)   // assumes Entity has a public float Health
                .FirstOrDefault();

            Transform target = highestThreat != null ? highestThreat.transform : null;
#if UNITY_EDITOR
            Debug.Log($"[Targeting_HighestThreat] Selected target: {target?.name ?? "None"}");
#endif
            return target;
        }
    }
}