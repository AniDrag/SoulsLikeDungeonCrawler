using UnityEngine;
using AniDrag.Core;
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
            controller.SetTargetSpeed(controller.walkSpeed);
            controller.GetAcceleration();
        }
        public override void OnUpdate() { }
        public override void OnFixedUpdate() { controller.ApplyMovement(); }
        public override void OnExit() { }
        public override void TransitionSetup()
        {
            controller.AddTransition(this, controller.idle, new FuncPredicate(Idle));
            controller.AddTransition(this, controller.walking, new FuncPredicate(Walking));
            controller.AddTransition(this, controller.running, new FuncPredicate(Running));
        }
        bool Idle()
        {
            return controller.isGrounded && controller.moveInput.sqrMagnitude <= 0.1f;
        }
        bool Walking()
        {
            return controller.isGrounded && controller.moveInput.sqrMagnitude > 0.1f &&
                   !controller.Inputs.actions["Sprint"].IsPressed();
        }
        bool Running()
        {
            return controller.isGrounded && controller.moveInput.sqrMagnitude > 0.1f &&
                   controller.Inputs.actions["Sprint"].IsPressed();
        }
    }
}