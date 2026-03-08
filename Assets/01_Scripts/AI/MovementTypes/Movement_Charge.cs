using UnityEngine;
using UnityEngine.AI;
namespace AniDrag.AI
{
    [CreateAssetMenu(menuName = "AI/Movement/Charge")]
    public class Movement_Charge : AICore_MovementStrategy
    {
        public float chargeSpeed = 5f;
        public float stopDistance = 1.5f;           // How close to get before stopping

        public override void Initialize(AICore_Controller ai)
        {
            if (ai.agent != null)
                ai.agent.speed = chargeSpeed;
        }

        public override void UpdateMovement(AICore_Controller ai, Transform target)
        {
            NavMeshAgent agent = ai.agent;
            if (agent == null || target == null) return;

            float distance = Vector3.Distance(ai.transform.position, target.position);
            if (distance <= stopDistance)
            {
                agent.isStopped = true;
                // Optionally face the target
                Vector3 direction = (target.position - ai.transform.position).normalized;
                direction.y = 0;
                if (direction != Vector3.zero)
                    ai.transform.rotation = Quaternion.LookRotation(direction);
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(target.position);
            }
        }
    }
}