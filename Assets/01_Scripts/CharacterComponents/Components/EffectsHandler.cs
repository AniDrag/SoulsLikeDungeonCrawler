using System.Collections.Generic;
using UnityEngine;
using AniDrag.Core;

namespace AniDrag.CharacterComponents
{
    /// <summary>
    /// Manages ongoing effects (buffs, debuffs, DoT, HoT) on an entity.
    /// Attach to any GameObject that has IHealth, IStamina, etc.
    /// Uses the global WorldTickService for tick updates.
    /// </summary>
    public class EffectsHandler : MonoBehaviour, IEffectReceiver, ITickable
    {
        // Cached components
        private IHealth _health;
        private IStamina _stamina;
        private IMana _mana;
        private IDefense _defense;

        private List<ActiveEffect> _activeEffects = new List<ActiveEffect>();

        private void Awake()
        {
            _health = GetComponent<IHealth>();
            _stamina = GetComponent<IStamina>();
            _mana = GetComponent<IMana>();
            _defense = GetComponent<IDefense>();
        }

        private void OnEnable()
        {
            Services.WorldTick?.Register(this);
        }

        private void OnDisable()
        {
            if (Services.WorldTick != null)
                Services.WorldTick.Unregister(this);
        }

        /// <summary>
        /// Apply an effect (immediate or over time).
        /// </summary>
        public void ApplyEffect(ConsumableEffectType effectType, int amount, float duration, GameObject source = null)
        {
            if (duration <= 0f)
            {
                ApplyInstantEffect(effectType, amount, source);
            }
            else
            {
                _activeEffects.Add(new ActiveEffect
                {
                    Type = effectType,
                    AmountPerTick = amount,
                    DurationRemaining = duration,
                    TickInterval = 1f, // Could be per-effect
                    TickTimer = 0f,
                    Source = source
                });
            }
        }

        private void ApplyInstantEffect(ConsumableEffectType type, int amount, GameObject source)
        {
            switch (type)
            {
                case ConsumableEffectType.Health:
                    if (amount > 0)
                        _health?.Heal(amount);
                    else if (amount < 0)
                        _health?.TakeDamage(-amount, source);
                    break;

                case ConsumableEffectType.Stamina:
                    if (amount > 0)
                        _stamina?.RegenerateStamina(amount);
                    else if (amount < 0)
                        _stamina?.UseStamina(-amount);
                    break;

                case ConsumableEffectType.Mana:
                    if (amount > 0)
                        _mana?.RegenerateMana(amount);
                    else if (amount < 0)
                        _mana?.UseMana(-amount);
                    break;

                case ConsumableEffectType.Defense:
                    if (amount > 0)
                        _defense?.AddDefense(amount);
                    else if (amount < 0)
                        _defense?.RemoveDefense(-amount);
                    break;

                // Stat buffs (temporary) – you'd need to implement a system for modifying core stats
                case ConsumableEffectType.Strength:
                case ConsumableEffectType.Dexterity:
                case ConsumableEffectType.Intelligence:
                case ConsumableEffectType.Agility:
                    Debug.LogWarning($"Stat effect {type} not implemented yet.");
                    break;

                default:
                    Debug.LogWarning($"Unhandled effect type: {type}");
                    break;
            }
        }

        public void OnWorldTick()
        {
            float tickDelta = Services.WorldTick.TickInterval; // Global tick interval

            for (int i = _activeEffects.Count - 1; i >= 0; i--)
            {
                var effect = _activeEffects[i];

                effect.TickTimer += tickDelta;
                while (effect.TickTimer >= effect.TickInterval)
                {
                    effect.TickTimer -= effect.TickInterval;
                    ApplyInstantEffect(effect.Type, effect.AmountPerTick, effect.Source);
                }

                effect.DurationRemaining -= tickDelta;
                if (effect.DurationRemaining <= 0f)
                {
                    // Fast removal (swap with last)
                    int lastIndex = _activeEffects.Count - 1;
                    _activeEffects[i] = _activeEffects[lastIndex];
                    _activeEffects.RemoveAt(lastIndex);
                }
            }
        }

        /// <summary>
        /// Remove all effects of a specific type.
        /// </summary>
        public void RemoveEffects(ConsumableEffectType type)
        {
            for (int i = _activeEffects.Count - 1; i >= 0; i--)
            {
                if (_activeEffects[i].Type == type)  // Fixed: was EffectType, now Type
                {
                    int lastIndex = _activeEffects.Count - 1;
                    _activeEffects[i] = _activeEffects[lastIndex];
                    _activeEffects.RemoveAt(lastIndex);
                }
            }
        }

        public void ClearAllEffects() => _activeEffects.Clear();

        // IEffectReceiver explicit implementation
        void IEffectReceiver.ApplyEffect(ConsumableEffectType effectType, int amount, float duration, GameObject owner)
            => ApplyEffect(effectType, amount, duration, owner);

        // Private class for active effect data
        [System.Serializable]
        private class ActiveEffect
        {
            public ConsumableEffectType Type;
            public int AmountPerTick;
            public float DurationRemaining;
            public float TickInterval;
            public float TickTimer;
            public GameObject Source;
        }
    }
}