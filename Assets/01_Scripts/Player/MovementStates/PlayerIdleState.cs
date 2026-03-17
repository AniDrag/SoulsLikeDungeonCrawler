using AniDrag.Core;
using UnityEngine;

namespace AniDrag.Player
{
    public class PlayerIdleState : BaseState<PlayerMovementController>
    {
        public PlayerIdleState(PlayerMovementController controller, Animator animator)
            : base(controller, animator) { }

        public override string StateName() => "Idle";

        public override void OnEnter()
        {
            controller.SetMovementProfile(PlayerMovementController.MovementProfile.Grounded);
            controller.SetTargetSpeed(0);
        }

        public override void OnUpdate() { }

        public override void OnFixedUpdate()
        {
            controller.ApplyMovement();
        }

        public override void OnExit() { }

        public override void TransitionSetup()
        {
            controller.AddTransition(this, controller.Falling, new FuncPredicate(Falling));
            controller.AddTransition(this, controller.Walking, new FuncPredicate(Walking));
            controller.AddTransition(this, controller.Running, new FuncPredicate(Running));
            controller.AddAnyTransition(controller.Jumping, new FuncPredicate(Jump));
        }

        private bool Falling() => controller.CurrentYSpeed < -0.03f;
        private bool Walking() => controller.MoveInput.sqrMagnitude > 0.1f;
        private bool Running() => controller.SprintHeld && Walking();
        private bool Jump()
        {
            controller.RequestJump();
            return controller.JumpPressed && controller.CanJump();
        }
    }
}