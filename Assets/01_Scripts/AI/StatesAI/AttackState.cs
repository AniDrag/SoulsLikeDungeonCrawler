using AniDrag.Core;
using UnityEngine;

namespace AniDrag.AI
{
    public class AttackState : BaseState<AIController>
    {
        public AttackState(AIController controller, Animator animator) : base(controller, animator) { }
        public override string StateName()
        {
            return "Attack State";
        }
        public override void OnEnter()
        {
            base.OnEnter();
            controller.StopMoving();
        }

        public override void OnUpdate()
        {
            var target = controller.memory.currentTarget;
            if (target == null) return; // transition will handle

            controller.FaceTarget(target);

            float cooldown = controller.combatStrategy.GetCooldown(controller);
            if (controller.combat.CanAttack(cooldown))
            {
                controller.Attack(target);
            }
        }

        public override void OnExit()
        {
            // Optionally reset attack animation trigger
        }
    }
}