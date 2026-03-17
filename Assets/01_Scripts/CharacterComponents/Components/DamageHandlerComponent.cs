using System;
using AniDrag.Core;
using UnityEngine;
using UnityEngine.Events;

namespace AniDrag.CharacterComponents
{
    /// <summary>
    /// Central damage processing component.
    /// Attach to any entity that can take damage.
    /// It will automatically find and use Defense, Shield, and Health components.
    /// </summary>
    [RequireComponent(typeof(IHealth))] // Health is mandatory
    public class DamageHandlerComponent: MonoBehaviour, IDamagable
    {
        private IHealth _health;
        private IShield _shield;
        private IDefense _defense;

        [Header("Optional Component References")]
        [SerializeField] private MonoBehaviour _shieldComponent;
        [SerializeField] private MonoBehaviour _defenseComponent;

        [Header("Events")]
        public UnityEvent<int, GameObject> OnDamageTaken;
        public UnityEvent<DamageResult> OnDamageProcessed;

        // IDamagable.DeathEvent implementation
        private Action _deathEvent;
        public Action DeathEvent
        {
            get => _deathEvent;
            set => _deathEvent = value;
        }
        private void Awake()
        {
            _health = GetComponent<IHealth>();
            if (_health == null)
            {
                Debug.LogError($"DamageHandler on {gameObject.name} requires an IHealth component!");
                return;
            }

            if (_shieldComponent != null)
                _shield = _shieldComponent as IShield;
            else
                _shield = GetComponent<IShield>();

            if (_defenseComponent != null)
                _defense = _defenseComponent as IDefense;
            else
                _defense = GetComponent<IDefense>();
        }
        private void OnEnable()
        {
            if (_health != null)
                _health.OnDeath += HandleHealthDeath;
        }
        private void OnDisable()
        {
            if (_health != null)
                _health.OnDeath -= HandleHealthDeath;
        }
        public void TakeDamage(int amount, GameObject source = null)
        {
            if (amount <= 0) return;
            if (_health == null) return;

            int originalDamage = amount;
            int damageAfterDefense = amount;
            int shieldAbsorbed = 0;
            int healthDamage = 0;

            if (_defense != null)
            {
                damageAfterDefense = Mathf.Max(0, amount - _defense.DefenseValue);
            }

            if (_shield != null && !_shield.IsShieldDepleted)
            {
                int shieldCurrent = _shield.CurrentShield;
                shieldAbsorbed = Mathf.Min(damageAfterDefense, shieldCurrent);
                _shield.TakeShieldDamage(shieldAbsorbed);
                damageAfterDefense -= shieldAbsorbed;
            }

            if (damageAfterDefense > 0)
            {
                healthDamage = damageAfterDefense;
                _health.TakeDamage(healthDamage, source);
            }

            DamageResult result = new DamageResult
            {
                OriginalDamage = originalDamage,
                DefenseReduction = originalDamage - damageAfterDefense - shieldAbsorbed,
                ShieldAbsorbed = shieldAbsorbed,
                HealthDamage = healthDamage,
                Source = source
            };

            OnDamageTaken?.Invoke(originalDamage, source);
            OnDamageProcessed?.Invoke(result);
        }
        private void HandleHealthDeath(GameObject source)
        {
            _deathEvent?.Invoke();
        }
    }

    /// <summary>
    /// Detailed breakdown of how damage was applied.
    /// Useful for UI (e.g., showing numbers: "10 (-2) [5]" etc.)
    /// </summary>
    [System.Serializable]
    public struct DamageResult
    {
        public int OriginalDamage;
        public int DefenseReduction;
        public int ShieldAbsorbed;
        public int HealthDamage;
        public GameObject Source;
    }
}