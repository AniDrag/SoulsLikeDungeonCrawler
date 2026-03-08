using AniDrag.CharacterComponents;
using UnityEngine;

namespace AniDrag.AI
{
    [CreateAssetMenu(menuName = "AniDrag/AI/Combat/Melee")]
    public class MeleeCombatStrategy : AICombatStrategy
    {
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float attackCooldown = 1.5f;

        public override float GetAttackRange(AIController ai) => attackRange;
        public override float GetCooldown(AIController ai) => attackCooldown;

        public override void Attack(AIController ai, Entity target)
        {
            // Delegate to AICombat component
            ai.combat.PerformAttack();
        }
    }
}