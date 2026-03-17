using AniDrag.Core;
using UnityEngine;

namespace AniDrag.Player
{
    public class PlayerFallingState : BaseState<PlayerMovementController>
    {
        public PlayerFallingState(PlayerMovementController controller, Animator animator)
            : base(controller, animator) { }

        public override string StateName() => "Falling";

        public override void OnEnter()
        {
            controller.SetMovementProfile(PlayerMovementController.MovementProfile.Airborne);
            controller.SetTargetSpeed(controller.WalkSpeed);
        }

        public override void OnUpdate() { }

        public override void OnFixedUpdate()
        {
            controller.ApplyMovement();
        }

        public override void OnExit() { }

        public override void TransitionSetup()
        {
            controller.AddTransition(this, controller.Idle, new FuncPredicate(Idle));
            controller.AddTransition(this, controller.Walking, new FuncPredicate(Walking));
            controller.AddTransition(this, controller.Running, new FuncPredicate(Running));
        }

        private bool Idle() => controller.IsGrounded && controller.MoveInput.sqrMagnitude <= 0.1f;
        private bool Walking() => controller.IsGrounded && controller.MoveInput.sqrMagnitude > 0.1f && !controller.SprintHeld;
        private bool Running() => controller.IsGrounded && controller.SprintHeld;
    }
}