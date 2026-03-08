using UnityEngine;

namespace AniDrag.AI
{
    public abstract class AICore_AttackStrategy : ScriptableObject
    {
        /// <summary> Called when the strategy is assigned to an AI (after instantiation). </summary>
        public virtual void Initialize(AICore_Controller ai) { }

        /// <summary> 
        /// Called every frame while the AI has a target and is within attack range.
        /// Use this to check cooldowns and call weapon methods (Fire, Attack, etc.).
        /// </summary>
        public abstract void UpdateAttack(AICore_Controller ai, Transform target);

        /// <summary> Optional: called when the AI enters attack range. </summary>
        public virtual void OnEnterAttackRange(AICore_Controller ai, Transform target) { }

        /// <summary> Optional: called when the AI leaves attack range. </summary>
        public virtual void OnExitAttackRange(AICore_Controller ai, Transform target) { }
    }
}