using UnityEngine;
namespace AniDrag.AI
{
    [CreateAssetMenu(menuName = "AI/Targeting/Random")]
    public class Targeting_Random : AICore_TargetingStrategy
    {
        public override Transform UpdateTarget(AICore_Controller ai)
        {
            var detected = ai.sense.detectedTargets;
            if (detected.Count == 0)
                return null;


            int randomIndex = Random.Range(0, detected.Count);
            Transform target = detected[randomIndex].transform;
#if UNITY_EDITOR
            Debug.Log($"[Targeting_Random] Selected target: {target?.name ?? "None"}");
#endif
            return target;
        }
    }
}