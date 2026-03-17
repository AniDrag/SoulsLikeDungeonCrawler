using System;
using AniDrag.Core;
using UnityEngine;
using UnityEngine.Events;

namespace AniDrag.CharacterComponents
{
    public class ManaComponent : MonoBehaviour, IMana
    {
        [Header("Settings")]
        [SerializeField] private int _baseMaxMana = 100;
        [SerializeField] private int _regenRate = 5; // per second

        private int _currentMana;
        private int _maxMana;
        private float _regenTimer;

        // IMana implementation
        public int CurrentMana => _currentMana;
        public int MaxMana => _maxMana;
        public void SetRegenRate(int rate) => _regenRate = rate;

        public event Action<IMana> OnManaChanged;
        public UnityEvent<int> OnManaChangedUnity;

        private void Awake()
        {
            _currentMana = _maxMana = _baseMaxMana;
        }

        public void Initialize(int maxMana, int regenRate)
        {
            _maxMana = maxMana;
            _regenRate = regenRate;
            _currentMana = _maxMana;
            OnManaChanged?.Invoke(this);
            OnManaChangedUnity?.Invoke(_currentMana);
        }

        private void Update()
        {
            if (_currentMana < _maxMana)
            {
                _regenTimer += Time.deltaTime;
                if (_regenTimer >= 1f)
                {
                    _regenTimer = 0f;
                    RegenerateMana(_regenRate);
                }
            }
        }

        public bool CanUseMana(int amount) => _currentMana >= amount;

        public void UseMana(int amount)
        {
            if (!CanUseMana(amount)) return;
            _currentMana -= amount;
            OnManaChanged?.Invoke(this);
            OnManaChangedUnity?.Invoke(_currentMana);
        }

        public void RegenerateMana(int amount)
        {
            _currentMana = Mathf.Min(_maxMana, _currentMana + amount);
            OnManaChanged?.Invoke(this);
            OnManaChangedUnity?.Invoke(_currentMana);
        }

        public void SetMaxMana(int newMax, bool refill = true)
        {
            _maxMana = newMax;
            if (refill) _currentMana = _maxMana;
            else _currentMana = Mathf.Min(_currentMana, _maxMana);
            OnManaChanged?.Invoke(this);
            OnManaChangedUnity?.Invoke(_currentMana);
        }
    }
}