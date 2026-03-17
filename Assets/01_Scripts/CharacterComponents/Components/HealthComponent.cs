using UnityEngine;
using UnityEngine.Events;
using System;
using AniDrag.Core;

namespace AniDrag.CharacterComponents
{
    public class HealthComponent : MonoBehaviour, IHealth
    {
        [SerializeField] private int _maxHealth = 100;
        private int _currentHealth;

        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;

        public event System.Action<IHealth> OnHealthChanged;
        public event System.Action<GameObject> OnDeath;
        public System.Action DeathEvent { get; set; } // for IDamagable

        private void Awake() => _currentHealth = _maxHealth;

        public void Initialize(int maxHealth)
        {
            _maxHealth = maxHealth;
            _currentHealth = maxHealth;
            OnHealthChanged?.Invoke(this);
        }

        // This method is now ONLY called by DamageHandler (or other systems that directly modify health)
        public void TakeDamage(int amount, GameObject source = null)
        {
            if (_currentHealth <= 0) return;
            _currentHealth = Mathf.Max(0, _currentHealth - amount);
            OnHealthChanged?.Invoke(this);
            if (_currentHealth <= 0)
            {
                HandleDeath(source);
            }
        }

        public void Heal(int amount)
        {
            if (_currentHealth <= 0) return;
            _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
            OnHealthChanged?.Invoke(this);
        }

        public void SetMaxHealth(int newMax, bool fullHeal = true)
        {
            _maxHealth = newMax;
            if (fullHeal) _currentHealth = _maxHealth;
            else _currentHealth = Mathf.Min(_currentHealth, _maxHealth);
            OnHealthChanged?.Invoke(this);
        }
        private void HandleDeath(GameObject killer)
        {
            // Publish death event for quests
            Services.EventBus?.Publish(new DeathEvent
            {
                Source = gameObject,
                Target = killer,
                XpReward = 0 // You can set XP value if needed
            });

            OnDeath?.Invoke(killer);
        }
    }
}