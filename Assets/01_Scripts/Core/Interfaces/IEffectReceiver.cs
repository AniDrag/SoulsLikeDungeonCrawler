using UnityEngine;

namespace AniDrag.Core
{
    /// <summary>
    /// Objects that can receive consumable effects (e.g., healing, buffs).
    /// </summary>
    public interface IEffectReceiver
    {
        void ApplyEffect(ConsumableEffectType effectType, int amount, float duration,GameObject owner = null);
    }
}