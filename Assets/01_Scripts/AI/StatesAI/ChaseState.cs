using AniDrag.Core;
using UnityEngine;

namespace AniDrag.AI
{
    public class ChaseState : BaseState<AIController>
    {
        public ChaseState(AIController controller, Animator animator) : base(controller, animator) { }
        public override string StateName()
        {
            return "Chase State";
        }
        public override void OnEnter()
        {
            base.OnEnter();
            // Optionally play chase animation
        }

        public override void OnUpdate()
        {
            var target = controller.memory.currentTarget;
            if (target != null)
            {
                controller.MoveTo(target.transform.position);
            }
        }

        public override void OnExit()
        {
            controller.StopMoving();
        }
    }
}