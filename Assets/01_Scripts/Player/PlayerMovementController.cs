using UnityEngine;
using UnityEngine.InputSystem;
using AniDrag.CharacterComponents;
using AniDrag.Core;
namespace AniDrag.Player
{
    /// <summary>
    /// Central controller for all player movement behavior.
    /// 
    /// Handles:
    /// - Input polling via the Unity Input System.
    /// - Movement state management (Idle, Walking, Running, etc.) through PlayerStateMachine.
    /// - Physics-based movement and environmental checks (ground, slope, water).
    /// - Smoothing acceleration/deceleration and clamping velocity.
    /// - Coyote time and input buffering for smoother controls.
    /// - Events for dash, slide, and jump that states can subscribe to.
    /// 
    /// Works with Unity's Rigidbody system. 
    /// Replace Rigidbody API with custom wrappers if your project uses modified physics.
    /// </summary>

    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PlayerInput))]
    
    public class PlayerMovementController : FSM
    {
        //All Animation names pertating to player movement state machine
        #region Animation Hashes
        [field: SerializeField] public string Run = "WalkRun";
        [field: SerializeField] public string Jump = "Jump";
        [field: SerializeField] public string Falling = "Falling";
        [field: SerializeField] public string Crouch = "Crouching";
        [field: SerializeField] public string Dash = "Dashing";
        [field: SerializeField] public string Slide = "Sliding";
        [field: SerializeField] public string Swim = "Swiming";
        #endregion
        public enum MovementProfile
        {
            None,       // movement disabled
            Grounded,   // normal walking/running
            Airborne,   // reduced control
            Swimming,   // water movement
            Sliding     // momentum-based
        }
        #region === References ===
        [Header("========================\n" +
       "     Refrences      \n" +
       "========================")]
        [Tooltip("The Rigidbody used for physics movement.")]
        [SerializeField] private Rigidbody body; public Rigidbody Body => body;
        [Tooltip("The PlayerInput component (new Input System).")]
        [SerializeField] private PlayerInput input; public PlayerInput Inputs => input;
        [Tooltip("Orientation transform used to convert local move vectors into world-space (usually player root).")]
        [SerializeField] private Transform orientation; public Transform Orientation => orientation;
        [Tooltip("Player Stats on the same object, used to determine movement speed and reaction timings.")]
        [SerializeField] private PlayerEntity playerStats;
        [Tooltip("Lowest point on player for water detection.")]
        [SerializeField] private Transform waterCheckTransform;
        [Tooltip("Waist height transform used to check ground contact.")]
        [SerializeField] private Transform groundCheckTransform;
        #endregion

        #region === Movement Settings ===
        [Header("========================\n" +
       "    Movement Settings      \n" +
       "========================")]
        [field:SerializeField] public float arealSpeed { get; private set; } = 1f;
        [Tooltip("Walking speed in meters per second.")]
        [field:SerializeField] public float walkSpeed { get; private set; } = 5f;
        [Tooltip("Running/sprinting speed in meters per second.")]
        [field: SerializeField] public float runSpeed { get; private set; } = 10f; 
        [Tooltip("Crouch movement speed in meters per second.")]
        [field:SerializeField] public float crouchSpeed { get; private set; } = 1f; 
        [Tooltip("Initial speed when sliding starts.")]
        [field:SerializeField] public float slideSpeed { get; private set; } = 13f; 
        [Tooltip("Force applied when performing a dash (impulse).")]
        [field:SerializeField] public float dashForce { get; private set; } = 20f; 
        [Tooltip("Upward impulse applied during jump.")]
        [field:SerializeField] public float jumpForce { get; private set; } = 7f;

        [Header("========================\n" +
       "    Debug Settings      \n" +
       "========================")]
        [Tooltip("Current smoothed speed used for animation blending.")]
        [field:SerializeField] public float currentSpeed { get; private set; } = 0f; 
        [Tooltip("Current vertical (Y) velocity for animation or logic checks.")]
        [field:SerializeField] public float yVelocity { get; private set; } = 0f;
        // Internal timers (negative values are fine — act as expired)
        private float _jumpBufferTimer;
        private float _coyoteTimer;
        private float _slideBufferTimer;
        private float _slideCooldownTimer;
        private float _dashBufferTimer;
        private float _dashCooldownTimer;
        
        #region === Input Cache ===
        public Vector2 moveInput { get; private set; }
        public Vector3 moveDirection { get; private set; }
        public bool toggleType { get; private set; } // if an input is toggle or hold type this helps states decide what to do when we let go og said button example running or crouching.
        #endregion

        [Header("========================\n" +
       "    Speed tinkering      \n" +
       "========================")]
        [Tooltip("Acceleration smoothing factor — higher = faster response.")]
        [SerializeField] private float _acceleration = 10f; 
        [Tooltip("Deceleration smoothing factor — higher = stops quicker.")]
        [SerializeField] private float _deceleration = 8f;
        [Tooltip("Control factor applied when in air! currentAcceleration * 0.9f - 0.2f.")]
        [SerializeField, Range(0.9f, 0.1f)] private float _inAirControlFactor = 0.7f;
        #endregion
        #region === Environment Settings ===
        [Header("=== Ground - Slope - Water Settings ===")]
        [Tooltip("Custom gravity value applied to the player.")]
        [SerializeField, Range(1f, 10f)] private float gravity = 9.8f; public float Gravity => gravity;
        [Tooltip("Drag value when grounded.")]
        [SerializeField, Range(0.01f, 1f)] private float groundDrag = 6f;
        [Tooltip("Drag value when in air.")]
        [SerializeField, Range(0.01f, 1f)] private float airDrag = 6f;
        [Tooltip("Drag value when underwater.")]
        [SerializeField, Range(0.01f, 1f)] private float waterDrag = 6f;
        [Tooltip("Drag value applied during sliding.")]
        [SerializeField, Range(0.01f, 1f)] private float slidingDrag = 6f;
        [Space]
        [Tooltip("Maximum slope angle the player can stand/walk on.")]
        [SerializeField, Range(20, 80)] private float maxSlopeAngle = 50f;
        [Tooltip("Cached slope hit data from raycast.")]
        [SerializeField] private RaycastHit _slopeHit;
        [Space]
        [Tooltip("Radius of sphere used for ground detection.")]
        [SerializeField, Range(0.01f, 1f)] private float groundCheckRadius = 0.4f;
        [Tooltip("Radius of sphere used for water detection.")]
        [SerializeField, Range(0.01f, 1f)] private float waterCheckRadius = 0.5f;
        [SerializeField] public bool onSlope { get; private set; }
        [SerializeField] public bool inAir { get; private set; }
        [SerializeField] public bool inWater { get; private set; }
        [SerializeField] public bool isGrounded { get; private set; }
        [Space]
        [Tooltip("Layer mask for ground detection.")]
        public LayerMask groundMask;
        [Tooltip("Layer mask for water detection.")]
        public LayerMask waterMask;
        #endregion
        #region === Timers and Buffers ===
        [Header("========================\n" +
       "    Grounding check Helpers      \n" +
       "========================")]
        [Tooltip("Coyote time allows jump shortly after leaving ground.")]
        [SerializeField] private float coyoteTime = 0.12f;
        [Tooltip("Jump buffering time window before landing.")]
        [SerializeField] private float jumpBufferTime = 0.12f;
        [Tooltip("Slide input buffer duration.")]
        [SerializeField] private float slideBufferTime = 0.12f;
        [Tooltip("Cooldown between slides.")]
        [SerializeField] private float slideCooldownTime = 0.5f;
        [Tooltip("Dash input buffer duration.")]
        [SerializeField] private float dashBufferTime = 0.12f;
        [Tooltip("Cooldown between dashes.")]
        [SerializeField] private float dashCooldownTime = 1.0f;


        #region === Movement Runtime State ===
        [Header("========================\n" +
       "    Debug stuff to see      \n" +
       "========================")]
        [SerializeField] private MovementProfile currentProfile = MovementProfile.Grounded;
        [SerializeField] private float targetSpeed = 10;
        [SerializeField] private bool movementEnabled = true;
        [SerializeField] private float targetLinearDamping;
        [SerializeField] private float dampingLerpSpeed = 5f;
        [SerializeField] private float _currentAcceleration;
        public float yvelocity = 0;

        #endregion
        #endregion

        #region === State Machine References ===
        // ------------------------------------------------------------------------------------------------------------------
        // States  ----------------------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------------------------------
        public PlayerIdleState idle { get; private set; }
        public PlayerWalkingState walking { get; private set; }
        public PlayerRunningState running { get; private set; }
        public PlayerJumpingState jumping { get; private set; }
        public PlayerFallingState falling { get; private set; }
        //public PlayerIdleState crouching { get; private set; }
        //public PlayerIdleState sliding { get; private set; }
        //public PlayerIdleState dashing { get; private set; }
        #endregion
        #region === Unity Events ===
        private void OnValidate()
        {
            // Auto-assign dependencies when component is first added
            body = GetComponent<Rigidbody>();
            playerStats = GetComponent<PlayerEntity>();
            input = GetComponent<PlayerInput>();
            for (int i = 0; i < transform.childCount; i++)
            {
                switch (transform.GetChild(i).name )
                {
                    case "Orientation":
                        orientation = transform.GetChild(i);
                        break;
                    case "GroundCheck":
                        groundCheckTransform = transform.GetChild(i);
                        break;
                    case "WaterCheck":
                        waterCheckTransform = transform.GetChild(i);
                        break;
                }
            }
        }

        private void Awake()
        {
            SetupFSM();

            SetMovementProfile(MovementProfile.Grounded);

            idle = new PlayerIdleState(this, animator);
            walking = new PlayerWalkingState(this, animator);
            running = new PlayerRunningState(this, animator);
            jumping = new PlayerJumpingState(this, animator);
            falling = new PlayerFallingState(this, animator);

            idle.TransitionSetup();
            walking.TransitionSetup();
            running.TransitionSetup();
            jumping.TransitionSetup();
            falling.TransitionSetup();

            SetState(idle);
        }

        protected override void Update()
        {
            Direction();
            Timers();
            base.Update();
            yvelocity = body.linearVelocity.y;
        }
        protected override void FixedUpdate()
        {
            EnvironmentCheck();
            //ApplyMovement();

            base.FixedUpdate();
        }
        #endregion

        #region === Input handling ===

        /// <summary>
        /// Polls player input and calculates the intended movement direction.
        /// Projects input to world-space based on camera/player orientation.
        /// Slope projection is applied if standing on a slope.
        /// </summary>
        private void Direction()
        {
            if (input == null || input.actions == null) return;

            moveInput = input.actions["Move"].ReadValue<Vector2>();
            moveInput = Vector2.ClampMagnitude(moveInput, 1f); // Better than Normalize()

            if (orientation == null) return;

            Vector3 rawDir = moveInput.x * orientation.right + moveInput.y * orientation.forward;
            rawDir.Normalize();
            moveDirection = onSlope ? GetSlopeMoveDirection(rawDir) : rawDir;
        }
        /// <summary>
        /// Projects a direction along the slope plane.
        /// Ensures the player moves along the slope surface rather than clipping through or flying off.
        /// </summary>
        Vector3 GetSlopeMoveDirection(Vector3 moveDir)
        {
            // Project the desired movement onto the slope's plane using the cached slope normal
            return Vector3.ProjectOnPlane(moveDir, _slopeHit.normal).normalized;
        }
        #endregion

        #region === Velocity Manipulation && Environment ===
        private void EnvironmentCheck()
        {
            // While feet tuch ground we are grounded so in shallow water we are grounded, when it reaches neare the head we are in in water and swiming.
            inWater = Physics.CheckSphere(waterCheckTransform.position, waterCheckRadius, waterMask); // shold be a raycast up from feet to head and get the chracter height and se if w are swiming or not. also if we hit the water plane we are in water.
            isGrounded = !inWater && Physics.CheckSphere(groundCheckTransform.position, groundCheckRadius, groundMask);
            onSlope = false;
            if(isGrounded)
            {
                // Buffer Timers here when on ground
                onSlope = OnSlopeCheck();
                body.useGravity = true;
                targetLinearDamping = groundDrag;
            }
            else if (inWater)
            {
                body.useGravity = false;
                targetLinearDamping = waterDrag;
            }
            else
            {
                body.useGravity = true;
                targetLinearDamping = airDrag;
            }

            LinearDampingLerping();
        }

        bool OnSlopeCheck()
        {
            if (Physics.Raycast(groundCheckTransform.position, Vector3.down, out _slopeHit, 0.1f))
            {
                float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
                return angle < maxSlopeAngle && angle != 0f;
            }
            return false;
        }

        //Lerps the linear damping to target value for smooth transitions and less jarring movement
        void LinearDampingLerping()
        {
            if (Mathf.Abs(body.linearDamping - targetLinearDamping) <= 0.01f)
            {
                if(body.linearDamping != targetLinearDamping)// Prevent unnecessary assignments
                    body.linearDamping = targetLinearDamping;

                return;
            }

            body.linearDamping = Mathf.Lerp(
                body.linearDamping,
                targetLinearDamping,
                dampingLerpSpeed * Time.fixedDeltaTime
            );
        }

        /// <summary>
        /// Ensures horizontal movement does not exceed the intended speed.
        /// Does not affect vertical velocity (jump/fall).
        /// </summary>
        public void ClampVelocity()
        {
            Vector3 flatVel = new Vector3(Body.linearVelocity.x, 0, Body.linearVelocity.z);
            if (flatVel.magnitude > targetSpeed)
            {
                Vector2 limitedVelocity = flatVel.normalized * targetSpeed;
                body.linearVelocity = new Vector3(limitedVelocity.x, body.linearVelocity.y, limitedVelocity.y);
            }
        }
        #endregion

        #region === Movement Application ===
        public void HandleAcceleration()
        {
            if (Body == null) return;
            //else if(targetSpeed == 0)
            //{ 
            //    ClampVelocity(); 
            //    return; 
            //}

            Vector3 targetVelocity = new Vector3(moveDirection.x * targetSpeed, Body.linearVelocity.y, moveDirection.z * targetSpeed);
            Body.linearVelocity = Vector3.Lerp(
                Body.linearVelocity,
                targetVelocity,
                _currentAcceleration * Time.fixedDeltaTime
            );
        }
        /// <summary>
        /// Applies Rigidbody velocity according to the current move direction and speed.
        /// Handles:
        /// - Slope alignment
        /// - Smooth acceleration and deceleration
        /// - Reduced mid-air control
        /// - Horizontal velocity clamping
        /// </summary>
        public void ApplyMovement()
        {
            GetAcceleration();
            HandleAcceleration();
            // 5. Clamp horizontal speed
            //ClampVelocity();
        }
        public void GetAcceleration()
        {
            
            _currentAcceleration = (targetSpeed > GetCurrentSpeed())
                          ? _acceleration
                          : _deceleration;
            if (inAir)
                _currentAcceleration *= _inAirControlFactor;
            else if (targetSpeed == 0 && !isGrounded)
            {
                _currentAcceleration = 20;
                return;
            }
        }

        float GetCurrentSpeed()
        {
            Vector3 flatVel = new Vector3(Body.linearVelocity.x, 0, Body.linearVelocity.z);
            return flatVel.magnitude;
        }

        #endregion

        #region === Buffer Timers ===
        /// <summary>
        /// Updates all action timers (jump, dash, slide).
        /// </summary>
        void Timers()
        {
            JumpBufferTimers();
            DashBufferTimers();
            SliderBufferTimers();
        }

        // Jump timing logic
        public void RequestJump() => _jumpBufferTimer = jumpBufferTime;
        public bool CanJump() => (_jumpBufferTimer > 0f) && (isGrounded || _coyoteTimer > 0f) && !inWater;
        public void JumpBufferTimers()
        {
            _jumpBufferTimer -= Time.deltaTime;
            _coyoteTimer -= Time.deltaTime;
        }

        // Dash timing logic
        public void RequestDash() => _dashBufferTimer = dashBufferTime;
        public bool CanDash() => !inWater && (_dashBufferTimer > 0f) && (_dashCooldownTimer <= 0f);
        public void DashBufferTimers()
        {
            _dashBufferTimer -= Time.deltaTime;
            _dashCooldownTimer -= Time.deltaTime;
        }

        // Slide timing logic
        public void RequestSlide() => _slideBufferTimer = slideBufferTime;
        public bool CanSlide()
        {
            if (!isGrounded || inWater || _slideCooldownTimer > 0 || NoInputDetected()) return false;
            return _slideBufferTimer > 0f;
        }
        public void SliderBufferTimers()
        {
            _slideBufferTimer -= Time.deltaTime;
            _slideCooldownTimer -= Time.deltaTime;
        }

        // Input state helper
        public bool NoInputDetected() => moveInput.magnitude < 0.1f;
        #endregion

        #region === Misc Helpers ===
        /// <summary>
        /// Handles transitions into crouch from any state when input is pressed.
        /// </summary>
        public void AnyState()
        {
            //if (Inputs.actions["Crouch"].IsPressed() && stateMC.currentState != crouching)
            //    stateMC.ChangeState(crouching);
            //else if (Inputs.actions["Dash"].IsPressed())
            //{
            //    RequestDash();
            //    if (CanDash())
            //    {
            //        stateMC.ChangeState(dashing);
            //    }
            //}
        }
        #endregion

        #region === Movement API (Called by States) ===

        /// <summary>
        /// Enables or disables all movement completely.
        /// Used for inventory, cutscenes, dialogue, stun, etc.
        /// </summary>
        public void SetMovementEnabled(bool enabled)
        {
            movementEnabled = enabled;

            if (!enabled)
            {
                // Immediately kill horizontal velocity
                Body.linearVelocity = new Vector3(0, Body.linearVelocity.y, 0);
            }
        }

        /// <summary>
        /// Sets how movement behaves (grounded, air, swimming, sliding).
        /// </summary>
        public void SetMovementProfile(MovementProfile profile)
        {
            currentProfile = profile;
        }

        /// <summary>
        /// Sets the desired movement speed (walk, run, crouch, swim).
        /// </summary>
        public void SetTargetSpeed(float speed)
        {
            targetSpeed = speed;
        }

        #endregion
        #region === Gizmos ===
        private void OnDrawGizmosSelected()
        {
            if (groundCheckTransform != null)
            {
                Gizmos.color = isGrounded ? Color.green : Color.red;
                Gizmos.DrawWireSphere(groundCheckTransform.position, groundCheckRadius);
            }

            if (waterCheckTransform != null)
            {
                Gizmos.color = inWater ? Color.blue : Color.cyan;
                Gizmos.DrawWireSphere(waterCheckTransform.position, waterCheckRadius);
            }

            if (Application.isPlaying)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, transform.position + moveDirection * 2f);
            }
        }
        #endregion
    }
}