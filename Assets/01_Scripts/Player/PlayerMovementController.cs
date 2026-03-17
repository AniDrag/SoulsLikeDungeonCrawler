using UnityEngine;
using AniDrag.Core;

namespace AniDrag.Player
{
    public class PlayerMovementController : FSM
    {
        // Animation parameter names
        [field: SerializeField] public string Run { get; private set; } = "WalkRun";
        [field: SerializeField] public string Jump { get; private set; } = "Jump";
        [field: SerializeField] public string FallingString { get; private set; } = "Falling";
        [field: SerializeField] public string Crouch { get; private set; } = "Crouching";
        [field: SerializeField] public string Dash { get; private set; } = "Dashing";
        [field: SerializeField] public string Slide { get; private set; } = "Sliding";
        [field: SerializeField] public string Swim { get; private set; } = "Swiming";

        public enum MovementProfile { None, Grounded, Airborne, Swimming, Sliding }

        #region References
        [Header("References")]
        [SerializeField] private Rigidbody _body;
        [SerializeField] private Transform _orientation;
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private Transform _waterCheck;
        [SerializeField] private CameraSettings _cameraSettings; // optional
        #endregion

        #region Movement Settings
        [Header("Movement Settings")]
        [SerializeField] private float _walkSpeed = 5f;
        [SerializeField] private float _runSpeed = 10f;
        [SerializeField] private float _crouchSpeed = 2f;
        [SerializeField] private float _airControl = 0.7f;
        [SerializeField] private float _acceleration = 10f;
        [SerializeField] private float _deceleration = 8f;
        [SerializeField] private float _jumpForce = 7f;
        [SerializeField] private float _gravity = 9.8f;

        public float WalkSpeed => _walkSpeed;
        public float RunSpeed => _runSpeed;
        public float JumpForce => _jumpForce;
        public float Gravity => _gravity;
        #endregion

        #region Audio
        [Header("Audio")]
        [SerializeField] private AudioSource _movementAudioSource;
        [SerializeField] private AudioClip[] _footstepClips;
        [SerializeField] private AudioClip _jumpClip;
        [SerializeField] private AudioClip _landClip;
        [SerializeField] private float _footstepInterval = 0.5f;

        private float _footstepTimer;
        private bool _wasGroundedLastFrame;
        #endregion

        #region Environment
        [Header("Environment")]
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private LayerMask _waterMask;
        [SerializeField] private float _groundCheckRadius = 0.4f;
        [SerializeField] private float _waterCheckRadius = 0.5f;
        [SerializeField] private float _maxSlopeAngle = 50f;

        [Header("Debug")]
        [SerializeField] private bool _isGrounded;
        [SerializeField] private bool _inWater;
        [SerializeField] private bool _onSlope;
        [SerializeField] private float _currentSpeed;
        [SerializeField] private float _targetSpeed;
        #endregion

        #region Timers
        [Header("Timers")]
        [SerializeField] private float _jumpBufferTime = 0.12f;
        [SerializeField] private float _dashBufferTime = 0.12f;
        [SerializeField] private float _slideBufferTime = 0.12f;

        private float _jumpBufferTimer;
        private float _dashBufferTimer;
        private float _slideBufferTimer;
        private float _slideCooldownTimer;
        private float _dashCooldownTimer;
        #endregion

        // Public properties for states
        public Rigidbody Body => _body;
        public Transform Orientation => _orientation;
        public float CurrentYSpeed => _body.linearVelocity.y;
        public bool IsGrounded => _isGrounded;
        public bool InWater => _inWater;
        public bool OnSlope => _onSlope;
        public Vector2 MoveInput { get; private set; }
        public Vector3 MoveDirection { get; private set; }
        public bool SprintHeld => Services.Input.SprintHeld;
        public bool JumpPressed => Services.Input.JumpPressed;
        public bool CrouchHeld => Services.Input.CrouchHeld;
        public bool DashPressed => Services.Input.DashPressed;
        public bool NoInput => MoveInput.sqrMagnitude < 0.1f;

        // States
        public PlayerIdleState Idle { get; private set; }
        public PlayerWalkingState Walking { get; private set; }
        public PlayerRunningState Running { get; private set; }
        public PlayerJumpingState Jumping { get; private set; }
        public PlayerFallingState Falling { get; private set; }

        private RaycastHit _slopeHit;
        private MovementProfile _currentProfile = MovementProfile.Grounded;
        private float _currentAcceleration;
        private bool _movementEnabled = true;

        private void OnValidate()
        {
            if (_body == null) _body = GetComponent<Rigidbody>();
            if (_orientation == null) _orientation = transform.Find("Orientation");
            if (_groundCheck == null) _groundCheck = transform.Find("GroundCheck");
            if (_waterCheck == null) _waterCheck = transform.Find("WaterCheck");
        }

        private void Awake()
        {
            SetupFSM();
            Idle = new PlayerIdleState(this, animator);
            Walking = new PlayerWalkingState(this, animator);
            Running = new PlayerRunningState(this, animator);
            Jumping = new PlayerJumpingState(this, animator);
            Falling = new PlayerFallingState(this, animator);

            Idle.TransitionSetup();
            Walking.TransitionSetup();
            Running.TransitionSetup();
            Jumping.TransitionSetup();
            Falling.TransitionSetup();

            SetState(Idle);
            _wasGroundedLastFrame = _isGrounded;
        }

       private void OnEnable()
       {
           Services.GameState.OnMovementAllowedChanged += SetMovementEnabled;
       }
       
       private void OnDisable()
       {
           Services.GameState.OnMovementAllowedChanged -= SetMovementEnabled;
       }


        protected override void Update()
        {
            if (!_movementEnabled) return;
            UpdateMoveInput();
            UpdateTimers();
            HandleFootsteps();
            base.Update();
        }

        protected override void FixedUpdate()
        {
            if (!_movementEnabled) return;
            CheckEnvironment();
            ApplyMovement();
            DetectLanding();
            base.FixedUpdate();
        }

        private void UpdateMoveInput()
        {
            MoveInput = Services.Input.MoveInput;
            MoveInput = Vector2.ClampMagnitude(MoveInput, 1f);

            if (_orientation != null)
            {
                Vector3 raw = MoveInput.x * _orientation.right + MoveInput.y * _orientation.forward;
                raw.Normalize();
                MoveDirection = _onSlope ? GetSlopeMoveDirection(raw) : raw;
            }
        }

        private Vector3 GetSlopeMoveDirection(Vector3 moveDir)
        {
            return Vector3.ProjectOnPlane(moveDir, _slopeHit.normal).normalized;
        }

        private void CheckEnvironment()
        {
            _inWater = Physics.CheckSphere(_waterCheck.position, _waterCheckRadius, _waterMask);
            _isGrounded = !_inWater && Physics.CheckSphere(_groundCheck.position, _groundCheckRadius, _groundMask);
            _onSlope = false;

            if (_isGrounded)
            {
                _onSlope = OnSlopeCheck();
                _body.linearDamping = 6f; // ground drag
            }
            else if (_inWater)
            {
                _body.useGravity = false;
                _body.linearDamping = 4f; // water drag
            }
            else
            {
                _body.useGravity = true;
                _body.linearDamping = 2f; // air drag
            }
        }

        private bool OnSlopeCheck()
        {
            if (Physics.Raycast(_groundCheck.position, Vector3.down, out _slopeHit, 0.1f))
            {
                float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
                return angle < _maxSlopeAngle && angle != 0f;
            }
            return false;
        }

        public void ApplyMovement()
        {
            UpdateTargetSpeed();
            UpdateAcceleration();
            Vector3 targetVelocity = new Vector3(MoveDirection.x * _targetSpeed, _body.linearVelocity.y, MoveDirection.z * _targetSpeed);
            _body.linearVelocity = Vector3.Lerp(_body.linearVelocity, targetVelocity, _currentAcceleration * Time.fixedDeltaTime);
        }

        private void UpdateTargetSpeed()
        {
            switch (_currentProfile)
            {
                case MovementProfile.Grounded:
                    _targetSpeed = SprintHeld ? _runSpeed : _walkSpeed;
                    break;
                case MovementProfile.Airborne:
                    _targetSpeed = _walkSpeed; // reduced control handled by acceleration factor
                    break;
                case MovementProfile.Swimming:
                    _targetSpeed = _walkSpeed * 0.5f;
                    break;
                case MovementProfile.Sliding:
                    // speed decays over time
                    break;
                default:
                    _targetSpeed = 0;
                    break;
            }
        }

        private void UpdateAcceleration()
        {
            float flatSpeed = new Vector3(_body.linearVelocity.x, 0, _body.linearVelocity.z).magnitude;
            _currentAcceleration = (_targetSpeed > flatSpeed) ? _acceleration : _deceleration;
            if (!_isGrounded)
                _currentAcceleration *= _airControl;
        }

        #region Public API for States
        public void SetMovementProfile(MovementProfile profile) => _currentProfile = profile;
        public void SetTargetSpeed(float speed) => _targetSpeed = speed;
        public void SetMovementEnabled(bool enabled)
        {
            Debug.Log($"Movement allowed changed to {enabled}");
            _movementEnabled = enabled;
            if (!enabled)
                _body.linearVelocity = new Vector3(0, _body.linearVelocity.y, 0);
        }
        public void RequestJump() => _jumpBufferTimer = _jumpBufferTime;
        public bool CanJump() => _jumpBufferTimer > 0f && _isGrounded && !_inWater;
        public void RequestDash() => _dashBufferTimer = _dashBufferTime;
        public bool CanDash() => _dashBufferTimer > 0f && _dashCooldownTimer <= 0f;
        public void RequestSlide() => _slideBufferTimer = _slideBufferTime;
        public bool CanSlide() => _isGrounded && !_inWater && _slideCooldownTimer <= 0f && !NoInput && _slideBufferTimer > 0f;

        // Called by JumpingState when jump is performed
        public void PlayJumpSound()
        {
            if (_movementAudioSource != null && _jumpClip != null)
                _movementAudioSource.PlayOneShot(_jumpClip);
        }
        #endregion

        private void UpdateTimers()
        {
            _jumpBufferTimer -= Time.deltaTime;
            _dashBufferTimer -= Time.deltaTime;
            _slideBufferTimer -= Time.deltaTime;
            _dashCooldownTimer -= Time.deltaTime;
            _slideCooldownTimer -= Time.deltaTime;
        }

        #region Audio
        private void HandleFootsteps()
        {
            if (!_isGrounded || _inWater || NoInput)
            {
                _footstepTimer = 0f;
                return;
            }

            float currentSpeed = new Vector3(_body.linearVelocity.x, 0, _body.linearVelocity.z).magnitude;
            if (currentSpeed < 0.1f) return;

            _footstepTimer -= Time.deltaTime;
            if (_footstepTimer <= 0f)
            {
                PlayFootstepSound();
                // Adjust interval based on speed
                float speedFactor = Mathf.Clamp01(currentSpeed / _runSpeed);
                _footstepTimer = _footstepInterval / Mathf.Max(0.5f, speedFactor);
            }
        }

        private void PlayFootstepSound()
        {
            if (_movementAudioSource == null || _footstepClips.Length == 0) return;
            AudioClip clip = _footstepClips[Random.Range(0, _footstepClips.Length)];
            _movementAudioSource.PlayOneShot(clip);
        }

        private void DetectLanding()
        {
            if (!_wasGroundedLastFrame && _isGrounded)
            {
                // Landed
                if (_movementAudioSource != null && _landClip != null)
                    _movementAudioSource.PlayOneShot(_landClip);
            }
            _wasGroundedLastFrame = _isGrounded;
        }
        #endregion

        private void OnDrawGizmosSelected()
        {
            if (_groundCheck)
            {
                Gizmos.color = _isGrounded ? Color.green : Color.red;
                Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRadius);
            }
            if (_waterCheck)
            {
                Gizmos.color = _inWater ? Color.blue : Color.cyan;
                Gizmos.DrawWireSphere(_waterCheck.position, _waterCheckRadius);
            }
        }
    }
}