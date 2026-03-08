using UnityEngine;
using AniDrag.CharacterComponents;

namespace AniDrag.AI
{
    public class AerialMovement : AIMovement
    {
        [Header("Movement Settings")]
        public float speed = 5f;
        public float turnSpeed = 120f;
        public float stoppingDistance = 0.5f;

        private Vector3 targetPosition;
        private bool isMoving = false;

        public override void MoveTo(Vector3 position)
        {
            targetPosition = position;
            isMoving = true;
        }

        public override void Stop()
        {
            isMoving = false;
        }

        public override void FaceTarget(Entity target)
        {
            if (target == null) return;
            Vector3 direction = (target.transform.position - transform.position).normalized;
            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot, turnSpeed * Time.deltaTime);
        }

        public override bool HasReachedDestination()
        {
            return Vector3.Distance(transform.position, targetPosition) <= stoppingDistance;
        }

        private void Update()
        {
            if (isMoving)
            {
                Vector3 direction = (targetPosition - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;
                // Optionally face movement direction
                if (direction != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }
}