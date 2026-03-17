using System;
using AniDrag.Core;
using UnityEngine;
using UnityEngine.Events;

namespace AniDrag.CharacterComponents
{
    public class DefenseComponent : MonoBehaviour, IDefense
    {
        [Header("Base Defense")]
        [SerializeField] private int _baseDefense = 0;

        private int _currentDefense; // base + temporary modifiers

        public int DefenseValue => _currentDefense;

        public event Action<IDefense> OnDefenseChanged;
        public UnityEvent<int> OnDefenseChangedUnity;

        private void Awake()
        {
            _currentDefense = _baseDefense;
        }

        public void SetBaseDefense(int value)
        {
            _baseDefense = value;
            RecalculateDefense();
        }

        public void AddDefense(int amount)
        {
            _currentDefense += amount;
            OnDefenseChanged?.Invoke(this);
            OnDefenseChangedUnity?.Invoke(_currentDefense);
        }

        public void RemoveDefense(int amount)
        {
            _currentDefense = Mathf.Max(0, _currentDefense - amount);
            OnDefenseChanged?.Invoke(this);
            OnDefenseChangedUnity?.Invoke(_currentDefense);
        }

        private void RecalculateDefense()
        {
            // In a more advanced system, you might sum base + temporary buffs.
            // For now, just set to base.
            _currentDefense = _baseDefense;
            OnDefenseChanged?.Invoke(this);
            OnDefenseChangedUnity?.Invoke(_currentDefense);
        }

#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] private int _debugAdd = 5;

        [ContextMenu("Debug Add Defense")]
        private void DebugAddDefense() => AddDefense(_debugAdd);

        [ContextMenu("Debug Remove Defense")]
        private void DebugRemoveDefense() => RemoveDefense(_debugAdd);
#endif
    }
}