using AniDrag.Core;
using UnityEngine;

namespace AniDrag.AI
{
    public class IdleState : BaseState<AIController>
    {
        public float waitEndTime { get; private set; }

        public IdleState(AIController controller, Animator animator) : base(controller, animator) { }
        public override string StateName()
        {
            return "Idle State";
        }
        public override void OnEnter()
        {
            base.OnEnter();
            waitEndTime = Time.time + Random.Range(2f, 5f);
            controller.StopMoving();
            // Optionally trigger idle animation
        }

        public override void OnUpdate()
        {
            // No active logic; transitions are handled by predicates.
        }
    }
}