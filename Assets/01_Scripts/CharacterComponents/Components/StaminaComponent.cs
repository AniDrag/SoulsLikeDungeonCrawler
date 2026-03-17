using UnityEngine;
using UnityEngine.Events;
using System;
using AniDrag.Core;

namespace AniDrag.CharacterComponents
{
    public class StaminaComponent : MonoBehaviour, IStamina
    {
        [Header("Settings")]
        [SerializeField] private int _baseMaxStamina = 100;
        [SerializeField] private int _regenRate = 5; // per second

        private int _currentStamina;
        private int _maxStamina;
        private float _regenTimer;

        // IStamina implementation
        public int CurrentStamina => _currentStamina;
        public int MaxStamina => _maxStamina;

        public event Action<IStamina> OnStaminaChanged;
        public UnityEvent<int> OnStaminaChangedUnity;

        private void Awake()
        {
            _currentStamina = _maxStamina = _baseMaxStamina;
        }
        public void SetRegenRate(int rate) => _regenRate = rate;
        public void Initialize(int maxStamina, int regenRate)
        {
            _maxStamina = maxStamina;
            _regenRate = regenRate;
            _currentStamina = _maxStamina;
            OnStaminaChanged?.Invoke(this);
            OnStaminaChangedUnity?.Invoke(_currentStamina);
        }

        private void Update()
        {
            if (_currentStamina < _maxStamina)
            {
                _regenTimer += Time.deltaTime;
                if (_regenTimer >= 1f)
                {
                    _regenTimer = 0f;
                    RegenerateStamina(_regenRate);
                }
            }
        }

        public bool CanUseStamina(int amount) => _currentStamina >= amount;

        public void UseStamina(int amount)
        {
            if (!CanUseStamina(amount)) return;
            _currentStamina -= amount;
            OnStaminaChanged?.Invoke(this);
            OnStaminaChangedUnity?.Invoke(_currentStamina);
        }

        public void RegenerateStamina(int amount)
        {
            _currentStamina = Mathf.Min(_maxStamina, _currentStamina + amount);
            OnStaminaChanged?.Invoke(this);
            OnStaminaChangedUnity?.Invoke(_currentStamina);
        }

        public void SetMaxStamina(int newMax, bool refill = true)
        {
            _maxStamina = newMax;
            if (refill) _currentStamina = _maxStamina;
            else _currentStamina = Mathf.Min(_currentStamina, _maxStamina);
            OnStaminaChanged?.Invoke(this);
            OnStaminaChangedUnity?.Invoke(_currentStamina);
        }
    }
}