using System;
using AniDrag.Core;
using UnityEngine;
using UnityEngine.Events;

namespace AniDrag.CharacterComponents
{
    public class ShieldComponent : MonoBehaviour, IShield
    {
        [Header("Settings")]
        [SerializeField] private int _baseMaxShield = 50;
        [SerializeField] private bool _autoRegenerate = true;
        [SerializeField] private int _regenRate = 5;      // per second
        [SerializeField] private float _regenDelay = 3f;  // seconds after last damage

        private int _currentShield;
        private int _maxShield;
        private float _lastDamageTime = -999f;

        // IShield implementation
        public int CurrentShield => _currentShield;
        public int MaxShield => _maxShield;
        public bool IsShieldDepleted => _currentShield <= 0;

        public event Action<IShield> OnShieldChanged;
        public event Action OnShieldDepleted;

        public UnityEvent<int> OnShieldChangedUnity;   // current value
        public UnityEvent OnShieldDepletedUnity;

        private void Awake()
        {
            _currentShield = _maxShield = _baseMaxShield;
        }

        private void Update()
        {
            if (_autoRegenerate && _currentShield < _maxShield && Time.time >= _lastDamageTime + _regenDelay)
            {
                RegenerateShield(_regenRate);
            }
        }

        public void TakeShieldDamage(int amount)
        {
            if (_currentShield <= 0) return;

            _currentShield = Mathf.Max(0, _currentShield - amount);
            _lastDamageTime = Time.time;

            OnShieldChanged?.Invoke(this);
            OnShieldChangedUnity?.Invoke(_currentShield);

            if (_currentShield <= 0)
            {
                OnShieldDepleted?.Invoke();
                OnShieldDepletedUnity?.Invoke();
            }
        }

        public void RegenerateShield(int amount)
        {
            if (_currentShield >= _maxShield) return;
            _currentShield = Mathf.Min(_maxShield, _currentShield + amount);
            OnShieldChanged?.Invoke(this);
            OnShieldChangedUnity?.Invoke(_currentShield);
        }

        public void SetMaxShield(int newMax, bool refill = true)
        {
            _maxShield = newMax;
            if (refill) _currentShield = _maxShield;
            else _currentShield = Mathf.Min(_currentShield, _maxShield);
            OnShieldChanged?.Invoke(this);
            OnShieldChangedUnity?.Invoke(_currentShield);
        }

#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] private int _debugDamage = 10;

        [ContextMenu("Debug Take Damage")]
        private void DebugTakeDamage() => TakeShieldDamage(_debugDamage);

        [ContextMenu("Debug Regenerate")]
        private void DebugRegenerate() => RegenerateShield(_regenRate);
#endif
    }
}