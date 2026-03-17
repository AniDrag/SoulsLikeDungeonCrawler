using UnityEngine;

namespace AniDrag.Core
{
    public class CursorUnlockHandler : MonoBehaviour
    {
        private void OnEnable()
        {
            Services.Input.OnEnableMouseChanged += HandleEnableMouse;
        }

        private void OnDisable()
        {
            Services.Input.OnEnableMouseChanged -= HandleEnableMouse;
        }

        private void HandleEnableMouse(bool isPressed)
        {
            if (isPressed)
            {
                Services.GameState.UnlockCursor();
                // Optionally disable camera look
            }
            else
            {
                Services.GameState.LockCursor();
                // Re-enable camera look
            }
        }
    }
}