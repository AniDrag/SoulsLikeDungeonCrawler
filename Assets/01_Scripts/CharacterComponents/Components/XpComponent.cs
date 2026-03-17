using UnityEngine;
using UnityEngine.Events;
using System;
using AniDrag.Core;

namespace AniDrag.CharacterComponents
{
    public class XpComponent : MonoBehaviour, IXp
    {
        [Header("Settings")]
        [SerializeField] private int _baseXP = 100;
        [SerializeField] private float _xpMultiplier = 1.5f;
        [SerializeField] private int _maxLevel = 100;

        private int _level = 1;
        private int _currentXP;
        private int _maxXP;

        // IXp implementation
        public int CurrentXp => _currentXP;
        public int MaxXp => _maxXP;
        public int Level => _level;

        public event Action<IXp> OnXpChanged;
        public event Action<int> OnLevelUp; // int = new level

        public UnityEvent<int> OnXpChangedUnity;
        public UnityEvent<int> OnLevelUpUnity;

        public void Initialize(int startLevel)
        {
            _level = startLevel;
            _currentXP = 0;
            RecalculateMaxXP();
            OnXpChanged?.Invoke(this);
        }

        public void GainXp(int amount)
        {
            if (_level >= _maxLevel) return;

            _currentXP += amount;

            while (_currentXP >= _maxXP && _level < _maxLevel)
            {
                _currentXP -= _maxXP;
                _level++;
                RecalculateMaxXP();
                OnLevelUp?.Invoke(_level);
                OnLevelUpUnity?.Invoke(_level);
            }

            OnXpChanged?.Invoke(this);
            OnXpChangedUnity?.Invoke(_currentXP);
        }

        private void RecalculateMaxXP()
        {
            _maxXP = Mathf.RoundToInt(_baseXP * Mathf.Pow(_level, _xpMultiplier));
        }

        // For save/load
        public void SetLevel(int level, int xp)
        {
            _level = level;
            _currentXP = xp;
            RecalculateMaxXP();
            OnXpChanged?.Invoke(this);
            OnXpChangedUnity?.Invoke(_currentXP);
        }
    }
}