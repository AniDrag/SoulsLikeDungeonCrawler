using AniDrag.Core;
using UnityEngine;

namespace AniDrag.Player
{
    public class PlayerWalkingState : BaseState<PlayerMovementController>
    {
        public PlayerWalkingState(PlayerMovementController controller, Animator animator)
            : base(controller, animator) { }

        public override string StateName() => "Walking";

        public override void OnEnter()
        {
            controller.SetMovementProfile(PlayerMovementController.MovementProfile.Grounded);
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
            controller.AddTransition(this, controller.Falling, new FuncPredicate(Falling));
            controller.AddTransition(this, controller.Idle, new FuncPredicate(Idle));
            controller.AddTransition(this, controller.Running, new FuncPredicate(Running));
            controller.AddTransition(this, controller.Jumping, new FuncPredicate(Jump));
        }

        private bool Falling() => controller.CurrentYSpeed < -0.03f;
        private bool Idle() => controller.MoveInput.sqrMagnitude <= 0.1f;
        private bool Running() => controller.SprintHeld;
        private bool Jump()
        {
            controller.RequestJump();
            return controller.JumpPressed && controller.CanJump();
        }
    }
}