using UnityEngine;
using AniDrag.Core;
namespace AniDrag.Player
{
    public class PlayerRunningState : BaseState<PlayerMovementController>
    {
        private bool togler = false;
        public PlayerRunningState(PlayerMovementController controller, Animator animator)
        : base(controller, animator) { }
        public override string StateName() => "Running";
        public override void OnEnter() {
            controller.SetMovementProfile(PlayerMovementController.MovementProfile.Grounded);
            if(controller.toggleType)
            {
                togler = true;
            }
            controller.SetTargetSpeed(controller.runSpeed);
            controller.GetAcceleration();
        }
        public override void OnUpdate() {  }
        public override void OnFixedUpdate() { controller.ApplyMovement(); }
        public override void OnExit() { }
        public override void TransitionSetup()
        {
            controller.AddTransition(this, controller.falling, new FuncPredicate(Falling));
            controller.AddTransition(this, controller.idle, new FuncPredicate(Idle));
            controller.AddTransition(this, controller.walking, new FuncPredicate(Walking));
            controller.AddTransition(this, controller.jumping, new FuncPredicate(Jump));
        }
        bool Falling()
        {
            return controller.yVelocity < -.03f;// .03f to avoid micro transitions and is a good buffer value 
        }
        bool Idle()
        {
            return controller.moveInput.sqrMagnitude <= 0.1f && !!controller.Inputs.actions["Sprint"].IsPressed(); 
        }
        bool Walking()
        {
            return togler? 
                controller.Inputs.actions["Sprint"].WasPressedThisFrame() : 
                !controller.Inputs.actions["Sprint"].IsPressed();
        }
        bool Jump()
        {
            controller.RequestJump();
            return controller.Inputs.actions["Jump"].IsPressed() && controller.CanJump();
        }
    }
}