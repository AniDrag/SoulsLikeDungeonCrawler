using System;
using UnityEngine;

namespace AniDrag.Core
{
    /// <summary>
    /// Objects that can take damage (e.g., characters, destructibles).
    /// </summary>
    public interface IDamagable
    {
        void TakeDamage(int amount, GameObject owner = null);
        Action DeathEvent { get; set; }
    }
}