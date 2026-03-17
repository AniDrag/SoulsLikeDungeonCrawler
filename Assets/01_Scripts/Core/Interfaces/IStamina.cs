using System;

namespace AniDrag.Core
{
    public interface IStamina
    {
        int CurrentStamina { get; }
        int MaxStamina { get; }
        bool CanUseStamina(int amount);
        void UseStamina(int amount);
        void RegenerateStamina(int amount);
        void SetMaxStamina(int newMax, bool refill = true);
        void SetRegenRate(int rate);
        
        event Action<IStamina> OnStaminaChanged;
    }
}