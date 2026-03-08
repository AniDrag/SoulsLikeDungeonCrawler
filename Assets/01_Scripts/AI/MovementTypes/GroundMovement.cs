using UnityEngine;
using UnityEngine.AI;
using AniDrag.CharacterComponents;

namespace AniDrag.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class GroundMovement : AIMovement
    {
        private NavMeshAgent agent;

        public override void Initialize(AIController ai)
        {
            base.Initialize(ai);
            agent = GetComponent<NavMeshAgent>();
            if (agent == null)
                Debug.LogError($"GroundMovement on {gameObject.name} requires a NavMeshAgent component!", this);
        }

        public override void MoveTo(Vector3 position)
        {
            if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
                agent.SetDestination(position);
        }

        public override void Stop()
        {
            if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
                agent.ResetPath();
        }

        public override void FaceTarget(Entity target)
        {
            if (target == null) return;
            Vector3 direction = (target.transform.position - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(direction);
        }

        public override bool HasReachedDestination()
        {
            if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh)
                return false;
            return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
        }

        public override Vector3[] GetPatrolPoints()
        {
            // Optional: implement later (e.g., get from spawner)
            return null;
        }
    }
}