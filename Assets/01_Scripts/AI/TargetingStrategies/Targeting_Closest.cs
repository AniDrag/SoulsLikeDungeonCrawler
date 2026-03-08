using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Targeting/Closest")]
public class Targeting_Closest : AICore_TargetingStrategy
{
    public override Transform UpdateTarget(AICore_Controller ai)
    {
        if (ai.sense.detectedTargets.Count == 0)
            return null;

        // Find the closest entity by distance
        var closest = ai.sense.detectedTargets
            .OrderBy(e => Vector3.Distance(ai.transform.position, e.transform.position))
            .FirstOrDefault();

        Transform target = closest != null ? closest.transform : null;
#if UNITY_EDITOR
        Debug.Log($"[Targeting_Closest] Selected target: {target?.name ?? "None"}");
#endif
        return target;
    }
}