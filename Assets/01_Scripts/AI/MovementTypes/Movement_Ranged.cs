using UnityEngine;
using UnityEngine.AI;
namespace AniDrag.AI
{
    [CreateAssetMenu(menuName = "AI/Movement/Ranged")]
    public class Movement_Ranged : AICore_MovementStrategy
    {
        public float preferredDistance = 8f;
        public float strafeRadius = 2f;
        public float strafeInterval = 2f;
        public float movementSpeed = 3.5f;

        private float lastStrafeTime;
        private Vector3 strafeDirection;

        public override void Initialize(AICore_Controller ai)
        {
            // Set agent speed
            if (ai.agent != null)
                ai.agent.speed = movementSpeed;
            lastStrafeTime = -strafeInterval;
        }

        public override void UpdateMovement(AICore_Controller ai, Transform target)
        {
            NavMeshAgent agent = ai.agent;
            if (agent == null || target == null) return;

            Vector3 toTarget = target.position - ai.transform.position;
            float distance = toTarget.magnitude;


            if (Mathf.Abs(distance - preferredDistance) > 1f)
            {

                Vector3 desiredPosition = target.position - toTarget.normalized * preferredDistance;
                agent.SetDestination(desiredPosition);
            }
            else
            {

                if (Time.time >= lastStrafeTime + strafeInterval)
                {

                    Vector3 right = Vector3.Cross(toTarget.normalized, Vector3.up).normalized;
                    strafeDirection = (Random.value > 0.5f ? right : -right) * strafeRadius;
                    lastStrafeTime = Time.time;
                }

                if (strafeDirection != Vector3.zero)
                {
                    Vector3 strafeTarget = ai.transform.position + strafeDirection;
                    agent.SetDestination(strafeTarget);
                }
            }
        }
    }
}