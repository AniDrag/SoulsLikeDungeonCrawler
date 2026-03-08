using AniDrag.Core;
using UnityEngine;

namespace AniDrag.AI
{
    public class SearchState : BaseState<AIController>
    {
        public float searchEndTime { get; private set; }

        public SearchState(AIController controller, Animator animator) : base(controller, animator) { }
        public override string StateName()
        {
            return "Searching State";
        }
        public override void OnEnter()
        {
            base.OnEnter();
            // Go to last known position of the target
            if (controller.memory.currentTarget != null)
            {
                var mem = controller.memory.GetMemory(controller.memory.currentTarget);
                if (mem != null)
                    controller.MoveTo(mem.lastKnownPosition);
            }
            else
            {
                controller.MoveTo(controller.transform.position);
            }

            searchEndTime = Time.time + 5f; 
        }

        public override void OnUpdate()
        {
            // State logic is minimal; transitions handle exiting.
        }

        public override void OnExit()
        {
            controller.StopMoving();
        }
    }
}