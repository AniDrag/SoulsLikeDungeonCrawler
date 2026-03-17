using System;

namespace AniDrag.Core
{
    public interface IShield
    {
        int CurrentShield { get; }
        int MaxShield { get; }
        bool IsShieldDepleted { get; }
        void TakeShieldDamage(int amount);
        void RegenerateShield(int amount);
        void SetMaxShield(int newMax, bool refill = true);
        
        event Action<IShield> OnShieldChanged;
        event Action OnShieldDepleted;
    }
}