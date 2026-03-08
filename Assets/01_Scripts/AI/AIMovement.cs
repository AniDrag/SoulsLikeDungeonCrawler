using UnityEngine;
using AniDrag.CharacterComponents;

namespace AniDrag.AI
{
    /// <summary>
    /// Abstract base for all movement types (ground, aerial).
    /// </summary>
    public abstract class AIMovement : MonoBehaviour
    {
        protected AIController ai;

        /// <summary>
        /// Initialize with the owning AIController.
        /// </summary>
        public virtual void Initialize(AIController ai)
        {
            this.ai = ai;
        }

        public abstract void MoveTo(Vector3 position);

        public abstract void Stop();

        public abstract void FaceTarget(Entity target);

        public abstract bool HasReachedDestination();

        public virtual Vector3[] GetPatrolPoints()
        {
            // Default: return null; can be overridden by a movement strategy.
            return null;
        }
    }
}