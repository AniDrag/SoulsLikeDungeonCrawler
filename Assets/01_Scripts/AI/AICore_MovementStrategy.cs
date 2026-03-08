using UnityEngine;

namespace AniDrag.AI
{
    public abstract class AICore_MovementStrategy : ScriptableObject
    {
        /// <summary> Called when the strategy is assigned to an AI (after instantiation). </summary>
        public virtual void Initialize(AICore_Controller ai) { }

        /// <summary> 
        /// Called every frame while the AI has a target. 
        /// Should control the NavMeshAgent (set destination, adjust speed, etc.).
        /// </summary>
        /// <param name="ai">The AI controller.</param>
        /// <param name="target">The current target transform.</param>
        public abstract void UpdateMovement(AICore_Controller ai, Transform target);
    }
}