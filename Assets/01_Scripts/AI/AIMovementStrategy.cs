using UnityEngine;

namespace AniDrag.AI
{
    /// <summary>
    /// Optional strategy for movement patterns (e.g., patrol points, wandering).
    /// </summary>
    public abstract class AIMovementStrategy : ScriptableObject
    {
        public virtual void Initialize(AIController ai) { }
        public abstract Vector3[] GetPatrolPoints(AIController ai);
    }
}