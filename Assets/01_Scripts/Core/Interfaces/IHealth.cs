using System;
using UnityEngine;

namespace AniDrag.Core
{
    public interface IHealth
    {
        int CurrentHealth { get; }
        int MaxHealth { get; }
        void TakeDamage(int amount, GameObject source = null);
        void Heal(int amount);
        void SetMaxHealth(int newMax, bool fullHeal = true);
        
        event Action<IHealth> OnHealthChanged;
        event Action<GameObject> OnDeath; // GameObject = killer
    }
}