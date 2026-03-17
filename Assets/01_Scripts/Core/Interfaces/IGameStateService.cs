using System;

namespace AniDrag.Core
{
    public interface IGameStateService
    {
        bool IsGameplayAllowed { get; }
        bool IsCursorLocked { get; }
        bool IsTimeFrozen { get; }
        bool IsMovementAllowed { get; }
        bool IsCameraControlAllowed { get; }

        void LockCursor();
        void UnlockCursor();
        void FreezeTime();
        void UnfreezeTime();
        void LockMovement();
        void UnlockMovement();
        void LockCamera();
        void UnlockCamera();

        event Action<bool> OnCursorLockChanged;          // true = locked
        event Action<bool> OnTimeFrozenChanged;          // true = frozen
        event Action<bool> OnMovementAllowedChanged;      // true = allowed
        event Action<bool> OnCameraControlAllowedChanged; // true = allowed
    }
}