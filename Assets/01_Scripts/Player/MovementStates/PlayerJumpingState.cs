using AniDrag.Core;
using UnityEngine;

namespace AniDrag.Player
{
    public class PlayerJumpingState : BaseState<PlayerMovementController>
    {
        private float _minFallVelocity = -0.1f;
        private bool _hasStartedFalling;

        public PlayerJumpingState(PlayerMovementController controller, Animator animator)
            : base(controller, animator) { }

        public override string StateName() => "Jumping";

        public override void OnEnter()
        {
            controller.SetMovementProfile(PlayerMovementController.MovementProfile.Airborne);
            _hasStartedFalling = false;

            // Calculate jump velocity
            float jumpVelocity = Mathf.Sqrt(2f * controller.Gravity * controller.JumpForce);
            Vector3 currentVel = controller.Body.linearVelocity;
            currentVel.y = 0; // reset Y
            controller.Body.linearVelocity = currentVel;
            currentVel.y = jumpVelocity;
            controller.Body.linearVelocity = currentVel;

            controller.SetTargetSpeed(controller.WalkSpeed);
        }

        public override void OnUpdate()
        {
            if (controller.Body.linearVelocity.y < _minFallVelocity)
                _hasStartedFalling = true;
        }

        public override void OnFixedUpdate()
        {
            controller.ApplyMovement();
        }

        public override void OnExit() { }

        public override void TransitionSetup()
        {
            controller.AddTransition(this, controller.Falling, new FuncPredicate(Falling));
            controller.AddTransition(this, controller.Idle, new FuncPredicate(Idle));
            controller.AddTransition(this, controller.Walking, new FuncPredicate(Walking));
            controller.AddTransition(this, controller.Running, new FuncPredicate(Running));
        }

        private bool Falling() => _hasStartedFalling && !controller.IsGrounded;
        private bool Idle() => controller.IsGrounded && controller.MoveInput.sqrMagnitude <= 0.1f;
        private bool Walking() => controller.IsGrounded && controller.MoveInput.sqrMagnitude > 0.1f && !controller.SprintHeld;
        private bool Running() => controller.IsGrounded && controller.SprintHeld;
    }
}