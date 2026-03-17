using System;
using UnityEngine;
using UnityEngine.InputSystem;
using AniDrag.Core.Input; 

namespace AniDrag.Core
{


    public class UnityInputService : MonoBehaviour, IInputService,IInitializableService
    {
        [SerializeField] private PlayerInput _playerInput;
        public PlayerInput Inputs => _playerInput;

        private InputSystem_Actions _inputActions;

        // --- IInputService properties ---

        // UI toggles
        public bool MenuPressed => _inputActions.UI.Menu.WasPressedThisFrame();
        public bool InventoryPressed => _inputActions.UI.Inventory.WasPressedThisFrame();
        public bool QuestPressed => _inputActions.UI.ToggleQuestPanel.WasPressedThisFrame();
        public bool EnableMouseHeld => _inputActions.UI.EnableMouse.IsPressed();

        // Combat
        public bool AttackPressed => _inputActions.MeleWeapons.Attack.WasPressedThisFrame();
        public bool AttackHeld => _inputActions.MeleWeapons.Attack.IsPressed();
        public bool AltAttackPressed => _inputActions.MeleWeapons.AltAttack.WasPressedThisFrame();
        public bool AltAttackHeld => _inputActions.MeleWeapons.AltAttack.IsPressed();
        public bool BlockPressed => _inputActions.MeleWeapons.Block.WasPressedThisFrame();
        public bool BlockHeld => _inputActions.MeleWeapons.Block.IsPressed();

        // Ranged
        public bool FirePressed => _inputActions.RangedWeapon.Fire.WasPressedThisFrame();
        public bool FireHeld => _inputActions.RangedWeapon.Fire.IsPressed();
        public bool AltFirePressed => _inputActions.RangedWeapon.AltFire.WasPressedThisFrame();
        public bool AltFireHeld => _inputActions.RangedWeapon.AltFire.IsPressed();
        public bool AimHeld => _inputActions.RangedWeapon.Aim.IsPressed();
        public bool ReloadPressed => _inputActions.RangedWeapon.Reload.WasPressedThisFrame();

        // General combat
        public bool HolsterPressed => _inputActions.GeneralCombatActions.Holster.WasPressedThisFrame();

        // Movement
        public Vector2 MoveInput
        {
            get
            {
                if (!Services.GameState.IsMovementAllowed) return Vector2.zero;
                return _inputActions.Player.Move.ReadValue<Vector2>();
            }
        }

        public bool JumpPressed => _inputActions.Player.Jump.WasPressedThisFrame();
        public bool JumpHeld => _inputActions.Player.Jump.IsPressed();
        public bool SprintHeld => _inputActions.Player.Sprint.IsPressed();
        public bool CrouchHeld => _inputActions.Player.Crouch.IsPressed();
        public bool DashPressed => _inputActions.Player.Dash.WasPressedThisFrame();
        public bool InteractPressed => _inputActions.Player.Interact.WasPressedThisFrame();
        public bool InteractHeld => _inputActions.Player.Interact.IsPressed();

        // Camera
        public Vector2 LookInput
        {
            get
            {
                if (!Services.GameState.IsCameraControlAllowed) return Vector2.zero;
                return _inputActions.Player.Look.ReadValue<Vector2>();
            }
        }

        // --- Events ---
        public event Action OnMenuToggle;
        public event Action OnInventoryToggle;
        public event Action OnQuestToggle;
        public event Action<bool> OnEnableMouseChanged;
        // (One‑shot action events are optional; you can also just query properties)

        private void Awake()
        {
            _inputActions = new InputSystem_Actions();

            // Subscribe to UI events – these are independent
            _inputActions.UI.Menu.performed += _ => OnMenuToggle?.Invoke();
            _inputActions.UI.Inventory.performed += _ => OnInventoryToggle?.Invoke();
            _inputActions.UI.ToggleQuestPanel.performed += _ => OnQuestToggle?.Invoke();
            _inputActions.UI.EnableMouse.started += _ => OnEnableMouseChanged?.Invoke(true);
            _inputActions.UI.EnableMouse.canceled += _ => OnEnableMouseChanged?.Invoke(false);

        }
        public void Initialize()
        {
            // Now Services.GameState is guaranteed to be registered.
            Services.GameState.OnMovementAllowedChanged += OnGameStateChanged;
            Services.GameState.OnCameraControlAllowedChanged += OnGameStateChanged;
            Services.GameState.OnTimeFrozenChanged += OnGameStateChanged;
            Services.GameState.OnCursorLockChanged += OnGameStateChanged;

            UpdateActionMapStates();
        }
        private void OnDestroy()
        {
            if (Services.GameState != null)
            {
                Services.GameState.OnMovementAllowedChanged -= OnGameStateChanged;
                Services.GameState.OnCameraControlAllowedChanged -= OnGameStateChanged;
                Services.GameState.OnTimeFrozenChanged -= OnGameStateChanged;
                Services.GameState.OnCursorLockChanged -= OnGameStateChanged;
            }

            _inputActions?.Disable();
            _inputActions?.Dispose();
        }

        private void OnGameStateChanged(bool _) => UpdateActionMapStates();

        private void UpdateActionMapStates()
        {
            var gs = Services.GameState;
            if (gs == null) return;

            bool gameplay = gs.IsCursorLocked && !gs.IsTimeFrozen;
            Debug.Log($"Gameplay = {gameplay} (CursorLocked={gs.IsCursorLocked}, TimeFrozen={gs.IsTimeFrozen})");
            SetMapEnabled(_inputActions.Player, gameplay);
            SetMapEnabled(_inputActions.MeleWeapons, gameplay);
            SetMapEnabled(_inputActions.RangedWeapon, gameplay);
            SetMapEnabled(_inputActions.GeneralCombatActions, gameplay);
            _inputActions.UI.Enable();
        }

        private void SetMapEnabled(InputActionMap map, bool enabled)
        {
            if (enabled)
                map.Enable();
            else
                map.Disable();
        }
    }
}