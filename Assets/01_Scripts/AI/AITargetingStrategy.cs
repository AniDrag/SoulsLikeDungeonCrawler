using UnityEngine;

namespace AniDrag.AI
{
    /// <summary>
    /// Base class for targeting strategies. Determines which enemy to focus.
    /// </summary>
    public abstract class AITargetingStrategy : ScriptableObject
    {
        protected Transform selectedTarget;

        public Transform GetTarget() => selectedTarget;
        /// <summary> Optional initialization when attached to an AI. </summary>
        public virtual void Initialize(AIController ai) { }

        /// <summary> Update the current target in AIMemory. </summary>
        public abstract void UpdateTarget(AIController ai, AIMemory memory);
    }
}