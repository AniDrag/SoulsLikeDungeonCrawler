using AniDrag.Core;
using UnityEngine;

namespace AniDrag.AI
{
    public class PatrolState : BaseState<AIController>
    {
        private Vector3[] patrolPoints;
        private int currentPointIndex;

        public PatrolState(AIController controller, Animator animator) : base(controller, animator) { }
        public override string StateName()
        {
            return "Patrol State";
        }
        public override void OnEnter()
        {
            base.OnEnter();
            // Get patrol points from movement strategy or spawner
            patrolPoints = controller.movementStrategy?.GetPatrolPoints(controller) ?? controller.movement.GetPatrolPoints();
            if (patrolPoints == null || patrolPoints.Length == 0)
            {
                // Fallback: stay in place? Or generate a random point within a radius.
                patrolPoints = new Vector3[] { controller.transform.position };
            }
            currentPointIndex = 0;
            GoToNextPoint();
        }

        private void GoToNextPoint()
        {
            if (patrolPoints.Length == 0) return;
            controller.MoveTo(patrolPoints[currentPointIndex]);
        }

        public override void OnUpdate()
        {
            if (controller.movement.HasReachedDestination())
            {
                currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
                GoToNextPoint();
            }
        }

        public override void OnExit()
        {
            controller.StopMoving();
        }
    }
}