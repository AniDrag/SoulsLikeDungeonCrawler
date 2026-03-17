using System;
using UnityEngine;

namespace AniDrag.Core
{
    public class GameStateService : MonoBehaviour, IGameStateService
    {
        // Request counters
        private int _cursorUnlockRequests = 0;   // number of systems wanting cursor FREE
        private int _timeFreezeRequests = 0;     // number of systems wanting time FROZEN
        private int _movementLocks = 0;           // number of systems blocking movement
        private int _cameraLocks = 0;             // number of systems blocking camera control

        // Last applied states (to avoid redundant updates)
        private bool _cursorLockedApplied = true;   // start locked
        private bool _timeFrozenApplied = false;    // start not frozen
        private bool _movementAllowedApplied = true; // start allowed
        private bool _cameraAllowedApplied = true;   // start allowed

        // Public properties – derived directly from request counters
        public bool IsCursorLocked => _cursorUnlockRequests == 0;
        public bool IsTimeFrozen => _timeFreezeRequests > 0;
        public bool IsMovementAllowed => _movementLocks == 0;
        public bool IsCameraControlAllowed => _cameraLocks == 0;
        public bool IsGameplayAllowed => IsMovementAllowed && IsCameraControlAllowed && !IsTimeFrozen;

        // Events
        public event Action<bool> OnCursorLockChanged;        // true = locked
        public event Action<bool> OnTimeFrozenChanged;        // true = frozen
        public event Action<bool> OnMovementAllowedChanged;   // true = allowed
        public event Action<bool> OnCameraControlAllowedChanged; // true = allowed

        private void Start()
        {
            // Sync applied states with the actual current state (in case they were set before Start)
            _cursorLockedApplied = Cursor.lockState == CursorLockMode.Locked;
            _timeFrozenApplied = Time.timeScale == 0f;
        }

        #region Cursor
        public void LockCursor()
        {
            _cursorUnlockRequests = Math.Max(0, _cursorUnlockRequests - 1);
            Debug.Log($"LockCursor: requests now {_cursorUnlockRequests}");
            UpdateCursor();
        }

        public void UnlockCursor()
        {
            _cursorUnlockRequests++;
            Debug.Log($"UnlockCursor: requests now {_cursorUnlockRequests}");
            UpdateCursor();
        }

        private void UpdateCursor()
        {
            bool newLocked = _cursorUnlockRequests == 0;
            if (newLocked == _cursorLockedApplied) return; // no change

            _cursorLockedApplied = newLocked;
            if (newLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Debug.Log("[Cursor] LOCKED");
            }
            else
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                Debug.Log("[Cursor] UNLOCKED");
            }

            OnCursorLockChanged?.Invoke(newLocked);
        }
        #endregion

        #region Time
        public void FreezeTime()
        {
            _timeFreezeRequests++;
            Debug.Log($"FreezeTime: requests now {_timeFreezeRequests}");
            UpdateTime();
        }

        public void UnfreezeTime()
        {
            _timeFreezeRequests = Math.Max(0, _timeFreezeRequests - 1);
            Debug.Log($"UnfreezeTime: requests now {_timeFreezeRequests}");
            UpdateTime();
        }

        private void UpdateTime()
        {
            bool newFrozen = _timeFreezeRequests > 0;
            if (newFrozen == _timeFrozenApplied) return;

            _timeFrozenApplied = newFrozen;
            Time.timeScale = newFrozen ? 0f : 1f;
            Debug.Log($"[Time] Frozen = {newFrozen}, timeScale = {Time.timeScale}");
            OnTimeFrozenChanged?.Invoke(newFrozen);
        }
        #endregion

        #region Movement
        public void LockMovement()
        {
            _movementLocks++;
            UpdateMovement();
        }

        public void UnlockMovement()
        {
            _movementLocks = Math.Max(0, _movementLocks - 1);
            UpdateMovement();
        }

        private void UpdateMovement()
        {
            bool allowed = _movementLocks == 0;
            if (allowed == _movementAllowedApplied) return;

            _movementAllowedApplied = allowed;
            OnMovementAllowedChanged?.Invoke(allowed);
        }
        #endregion

        #region Camera
        public void LockCamera()
        {
            _cameraLocks++;
            UpdateCamera();
        }

        public void UnlockCamera()
        {
            _cameraLocks = Math.Max(0, _cameraLocks - 1);
            UpdateCamera();
        }

        private void UpdateCamera()
        {
            bool allowed = _cameraLocks == 0;
            if (allowed == _cameraAllowedApplied) return;

            _cameraAllowedApplied = allowed;
            OnCameraControlAllowedChanged?.Invoke(allowed);
        }
        #endregion
    }
}