using System;
using UnityEngine;
using AniDrag.Utility;

namespace AniDrag.CharacterComponents
{
    /// <summary>
    /// Mainly used by the PlayerEntity class to manage stamina for actions like sprinting or attacking.
    /// Can be attached to any GameObject that requires stamina management.
    /// Will be used by boss enemies in the future for special attacks or phases, and give attack of opportunity when stamina is low.
    /// </summary>

    public class StaminaComponent : MonoBehaviour
    {
        [Header("========================\n" +
     "    Stamina Details      \n" +
     "========================")]
        [field: SerializeField] public int maxStamina { get; private set; } = 100;
        public int currentStamina { get; private set; }
        [SerializeField] private int staminaRegenRate = 5; // Stamina regenerated per second

        // ------------------------------------------------------------------------------------------------------------------
        // Actions ----------------------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------------------------------
        public Action<StaminaComponent> updateStamina;

        public bool CanUseStamina(int amount) => currentStamina >= amount;
        public void InitializeStaminaComponent(Stats stats)
        {
            maxStamina = Mathf.Max(1, stats.STR * 2 + stats.DEX * 2 + 10);
            staminaRegenRate = Mathf.Max(0, (int)(maxStamina /100) + (int)(stats.DEX / 10));
            currentStamina = maxStamina;
            updateStamina?.Invoke(this);
        }   

        public void RegenerateStamina(int amount)
        {
            currentStamina = Mathf.Min(currentStamina + amount, maxStamina);
            updateStamina?.Invoke(this);
        }

        public void UseStamina(int amount)
        {
            currentStamina = Mathf.Max(currentStamina - amount, 0);
            updateStamina?.Invoke(this);
        }
        public void UpdateStamina(Stats data, bool refill = true)
        {
            maxStamina = Mathf.Max(1, data.STR * 2 + data.DEX * 2 + 10);
            staminaRegenRate = Mathf.Max(0, (int)(maxStamina / 100) + (int)(data.DEX / 10));
            if (refill) currentStamina = maxStamina;
            updateStamina?.Invoke(this);
        }
#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] int staminaUsage = 25;

        [Button]
        public void ReduceStamina()
        {
            if (currentStamina < staminaUsage)
            {
                Debug.Log("Entity is depleted of stamina.");
                return;
            }

            UseStamina(staminaUsage);
            Debug.Log($"Stamina now: {currentStamina}");
        }

        [Button]
        public void AddStamina()
        {
            if (currentStamina >= maxStamina)
            {
                Debug.Log("Entity already has max stamina.");
                return;
            }

            RegenerateStamina(staminaRegenRate);
            Debug.Log($"Stamina now: {currentStamina}");
        }
#endif
    }
}