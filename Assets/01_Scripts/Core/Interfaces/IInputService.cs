using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AniDrag.Core
{
    public interface IInputService
    {
        // Direct access to PlayerInput (optional, for legacy cases)
        PlayerInput Inputs { get; }
        //void Initialize();
        // UI toggles – pressed this frame
        bool MenuPressed { get; }
        bool InventoryPressed { get; }
        bool QuestPressed { get; }
        bool EnableMouseHeld { get; }   // true while button is held

        // Events for UI toggles
        event Action OnMenuToggle;
        event Action OnInventoryToggle;
        event Action OnQuestToggle;
        event Action<bool> OnEnableMouseChanged;   // true = pressed, false = released

        // Combat inputs
        bool AttackPressed { get; }      // melee attack (was pressed this frame)
        bool AttackHeld { get; }         // melee attack (held)
        bool AltAttackPressed { get; }   // alt melee attack
        bool AltAttackHeld { get; }
        bool BlockPressed { get; }       // block (pressed)
        bool BlockHeld { get; }          // block (held)

        // Ranged combat
        bool FirePressed { get; }
        bool FireHeld { get; }
        bool AltFirePressed { get; }
        bool AltFireHeld { get; }
        bool AimHeld { get; }
        bool ReloadPressed { get; }

        // General combat
        bool HolsterPressed { get; }

        // Movement
        Vector2 MoveInput { get; }
        bool JumpPressed { get; }
        bool JumpHeld { get; }
        bool SprintHeld { get; }
        bool CrouchHeld { get; }
        bool DashPressed { get; }
        bool InteractPressed { get; }
        bool InteractHeld { get; }

        // Camera
        Vector2 LookInput { get; }
    }
}