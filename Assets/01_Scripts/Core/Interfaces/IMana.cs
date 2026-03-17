using System;
namespace AniDrag.Core
{
    public interface IMana
    {
        int CurrentMana { get; }
        int MaxMana { get; }
        
        bool CanUseMana(int amount);
        void UseMana(int amount);
        void RegenerateMana(int amount);
        void SetMaxMana(int newMax, bool refill = true);
        void SetRegenRate(int rate);
        
        event System.Action<IMana> OnManaChanged;
    }
}