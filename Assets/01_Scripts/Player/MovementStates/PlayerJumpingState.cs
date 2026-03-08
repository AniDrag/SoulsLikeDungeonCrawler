using AniDrag.Core;
using UnityEngine;

namespace AniDrag.Player
{
    public class PlayerJumpingState : BaseState<PlayerMovementController>
    {
        public PlayerJumpingState(PlayerMovementController controller, Animator animator)
        : base(controller, animator) { }

        private float minFallVelocity = -0.1f; // Slightly larger threshold
        private bool hasStartedFalling = false;


        public override string StateName() => "Jumping";
        public override void OnEnter() {
            controller.SetMovementProfile(PlayerMovementController.MovementProfile.Airborne);
            hasStartedFalling = false;
            //Debug.Log($"[Jump] Entered Jumping state");

            // --- CALCULATE CONSISTENT JUMP ---
            float jumpVelocity = Mathf.Sqrt(2f * controller.Gravity * controller.jumpForce); // height in meters
            Vector3 resetVel = new Vector3(controller.Body.linearVelocity.x, 0, controller.Body.linearVelocity.z); // reset for consistent jump height

            // --- FORWARD MOMENTUM BOOST BASED ON PREVIOUS STATE ---
            // float forwardBoost = 0f;
            // if (stateMachine.oldState == player.walking) forwardBoost = .07f;
            // else if (stateMachine.oldState == player.running) forwardBoost = .1f;
            // else if (stateMachine.oldState == player.sliding) forwardBoost = .15f;


            // Apply final velocity to Rigidbody
            controller.Body.linearVelocity = resetVel;//reset Y velocity to 0.
            resetVel.y = jumpVelocity; // set jump velocity to resetVel
            controller.Body.linearVelocity = resetVel;
            // player.Body.AddForce(player.moveDirection * forwardBoost, ForceMode.Impulse);
            controller.SetTargetSpeed(controller.walkSpeed);
            controller.GetAcceleration();

            //Debug.Log($"[Jump] Jump initiated | vY={jumpVelocity:F2}");
        }
        public override void OnUpdate() {
            if (controller.Body.linearVelocity.y < minFallVelocity)
            {
                hasStartedFalling = true;
            }
        }
        public override void OnFixedUpdate() { controller.ApplyMovement(); }
        public override void OnExit() { }
        public override void TransitionSetup()
        {
            controller.AddTransition(this, controller.falling, new FuncPredicate(Falling));
            controller.AddTransition(this, controller.idle, new FuncPredicate(Idle));
            controller.AddTransition(this, controller.walking, new FuncPredicate(Walking));
            controller.AddTransition(this, controller.running, new FuncPredicate(Running));
        }
        bool Falling()
        {
            return hasStartedFalling && !controller.isGrounded;// .03f to avoid micro transitions and is a good buffer value 
        }
        bool Idle()
        {
            return controller.isGrounded && !Walking();
        }
        bool Walking()
        {
            return controller.moveInput.sqrMagnitude > 0.1f;
        }
        bool Running()
        {
            return controller.Inputs.actions["Sprint"].IsPressed() && Walking();
        }
    }
}