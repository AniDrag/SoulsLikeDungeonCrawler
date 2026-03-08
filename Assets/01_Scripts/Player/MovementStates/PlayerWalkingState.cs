using UnityEngine;
using AniDrag.Core;
namespace AniDrag.Player
{
    public class PlayerWalkingState : BaseState<PlayerMovementController>
    {
        public PlayerWalkingState(PlayerMovementController controller, Animator animator)
        : base(controller, animator) { }
        public override string StateName() => "Walking";
        public override void OnEnter() {
            controller.SetMovementProfile(PlayerMovementController.MovementProfile.Grounded);
            controller.SetTargetSpeed(controller.walkSpeed);
            controller.GetAcceleration();
        }
        public override void OnUpdate() { }
        public override void OnFixedUpdate() { controller.ApplyMovement(); }
        public override void OnExit() { }
        public override void TransitionSetup()
        {
            controller.AddTransition(this, controller.falling, new FuncPredicate(Falling));
            controller.AddTransition(this, controller.idle, new FuncPredicate(Idle));
            controller.AddTransition(this, controller.running, new FuncPredicate(Running));
            controller.AddTransition(this, controller.jumping, new FuncPredicate(Jump));
        }
        bool Falling()
        {
            return controller.yVelocity < -.03f;// .03f to avoid micro transitions and is a good buffer value 
        }
        bool Idle()
        {
            return controller.moveInput.sqrMagnitude <= 0.1f;
        }
        bool Running()
        {
            return controller.Inputs.actions["Sprint"].IsPressed();
        }
        bool Jump()
        {
            controller.RequestJump();
            return controller.Inputs.actions["Jump"].IsPressed() && controller.CanJump();
        }
    }
}