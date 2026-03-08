using System.Collections.Generic;
using UnityEngine;
using AniDrag.CharacterComponents;

namespace AniDrag.AI
{
    /// <summary>
    /// Handles sensory detection (vision and hearing) for the AI.
    /// </summary>
    public class AIPerception : MonoBehaviour
    {
        [Header("Vision")]
        public float viewRadius = 10f;
        [Range(0, 360)] public float viewAngle = 90f;
        public LayerMask targetLayers;
        public LayerMask obstacleLayers;

        [Header("Hearing")]
        public float hearingRadius = 15f;

        [Header("Output")]
        public List<Entity> detectedTargets = new List<Entity>();

        /// <summary>
        /// Refresh the list of detected targets based on current senses.
        /// Called periodically by AIController.
        /// </summary>
        public void Core_Update()
        {
            detectedTargets.Clear();

            // --- Vision ---
            Collider[] targetsInRange = Physics.OverlapSphere(transform.position, viewRadius, targetLayers);
            foreach (var col in targetsInRange)
            {
                Entity target = col.GetComponent<Entity>();
                if (target == null) continue;

                Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                {
                    float dist = Vector3.Distance(transform.position, target.transform.position);
                    if (!Physics.Raycast(transform.position, dirToTarget, dist, obstacleLayers))
                    {
                        detectedTargets.Add(target);
                    }
                }
            }

            // --- Hearing (simplified) ---
            // For now, just add any target within hearing radius (no obstacles check).
            // You can expand this with a sound emission system later.
            Collider[] hearable = Physics.OverlapSphere(transform.position, hearingRadius, targetLayers);
            foreach (var col in hearable)
            {
                Entity target = col.GetComponent<Entity>();
                if (target == null || detectedTargets.Contains(target)) continue;
                // Optionally check line-of-sight or sound occlusion
                detectedTargets.Add(target);
            }
        }
        private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
                angleInDegrees += transform.eulerAngles.y;
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
        // Gizmos for debugging
        private void OnDrawGizmosSelected()
        {
            // Vision cone
            Gizmos.color = Color.yellow;
            Vector3 forward = DirFromAngle(-viewAngle / 2, false);
            Gizmos.DrawLine(transform.position, transform.position + forward * viewRadius);
            forward = DirFromAngle(viewAngle / 2, false);
            Gizmos.DrawLine(transform.position, transform.position + forward * viewRadius);

            // Hearing radius
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, hearingRadius);
        }       
    }
}