using AniDrag.CharacterComponents;
using System.Collections.Generic;
using UnityEngine;
namespace AniDrag.AI
{
    public class AICore_Sense : MonoBehaviour
    {
        [Header("Vision")]
        public float viewDistance = 10f;
        [Range(0, 360)] public float viewAngle = 90f;
        public LayerMask targetLayers;
        public LayerMask obstacleLayers;

        [Header("Output")]
        public List<Entity> detectedTargets = new List<Entity>();

        private Collider[] hitColliders = new Collider[10];
        /// <summary>
        /// Refresh the list of detected targets based on vision.
        /// Called periodically by AIController.
        /// </summary>
        public void Core_Update()
        {
            detectedTargets.Clear();
            int numFound = Physics.OverlapSphereNonAlloc(transform.position, viewDistance, hitColliders, targetLayers);
            for (int i = 0; i < numFound; i++)
            {
                Entity target = hitColliders[i].GetComponent<Entity>();
                if (target == null) continue;

                Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
                float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

                float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
                bool inViewCone = angleToTarget < viewAngle / 2;

                if (inViewCone)
                {
                    Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;

                    if (!Physics.Raycast(rayOrigin, directionToTarget, out RaycastHit hit, distanceToTarget, obstacleLayers))
                    {
                        detectedTargets.Add(target);
#if UNITY_EDITOR
                        Debug.DrawLine(rayOrigin, target.transform.position, Color.green, 0.1f);
#endif
                    }
                    else
                    {
#if UNITY_EDITOR
                        Debug.DrawLine(rayOrigin, target.transform.position, Color.red, 0.1f);
                        Debug.Log($"Raycast hit: {hit.collider.gameObject.name} on layer {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
#endif
                    }
                }
            }
        }
        

        // Utility for drawing the vision cone in the editor
        private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
                angleInDegrees += transform.eulerAngles.y;
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            // Draw the vision cone boundaries
            Gizmos.color = Color.yellow;
            Vector3 leftBoundary = DirFromAngle(-viewAngle / 2, false);
            Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewDistance);
            Vector3 rightBoundary = DirFromAngle(viewAngle / 2, false);
            Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewDistance);

            // Optionally draw the full sphere radius (light gray)
            Gizmos.color = new Color(1, 1, 0, 0.2f);
            Gizmos.DrawWireSphere(transform.position, viewDistance);
        }
#endif
    }
}