using UnityEngine;
using System;
using AniDrag.Core;

namespace AniDrag.CharacterComponents
{
    public class Entity : MonoBehaviour, IEffectReceiver,IStatsProvider
    {
        [Header("Entity Info")]
        [SerializeField] protected string _entityName = "Entity";

        [Header("Stats & Leveling (Optional)")]
        [SerializeField] private CharacterClass _characterClass;
        [SerializeField] private int _level = 1;

        // Core component interfaces
        public IHealth Health { get; private set; }
        public IStamina Stamina { get; private set; }
        public IMana Mana { get; private set; }
        public IShield Shield { get; private set; }
        public IDefense Defense { get; private set; }
        public IXp Xp { get; private set; }
        public IEquipmentUser Equipment { get; private set; }
        
        public Stats GetBaseStats() => _statsBlock.BaseStats;
        public Stats GetTotalStats() => _statsBlock.CurrentStats; 

        private StatsBlock _statsBlock;
        public int Level => _level;
        public bool HasStats => _characterClass != null;

        public event Action<Entity> OnEntityChanged;
        public event Action<Entity, GameObject> OnEntityDied;

        public string EntityName => _entityName;

        protected virtual void Awake()
        {
            FindComponents();
            
            if (HasStats)
            {
                InitializeStats();
                ApplyStatsToComponents();
            }
            
            SubscribeToEvents();
        }

        protected virtual void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void FindComponents()
        {
            Health = GetComponent<IHealth>();
            Stamina = GetComponent<IStamina>();
            Mana = GetComponent<IMana>();
            Shield = GetComponent<IShield>();
            Defense = GetComponent<IDefense>();
            Xp = GetComponent<IXp>();
            Equipment = GetComponent<IEquipmentUser>();
        }

        private void SubscribeToEvents()
        {
            if (Health != null)
                Health.OnDeath += HandleDeath;
            
            if (Xp != null)
                Xp.OnLevelUp += HandleLevelUp;
            
            if (Equipment != null)
                Equipment.OnEquipmentChanged += HandleEquipmentChanged;
        }

        private void UnsubscribeFromEvents()
        {
            if (Health != null)
                Health.OnDeath -= HandleDeath;
            
            if (Xp != null)
                Xp.OnLevelUp -= HandleLevelUp;
            
            if (Equipment != null)
                Equipment.OnEquipmentChanged -= HandleEquipmentChanged;
        }

        #region Stats & Leveling
        private void InitializeStats()
        {
            if (_characterClass == null) return;
            _statsBlock = new StatsBlock(_characterClass.baseStats, _characterClass.growthFactors);
            _statsBlock.UpdateStats(_level);
        }

        private void ApplyStatsToComponents()
        {
            if (!HasStats || _statsBlock == null) return;

            Stats stats = _statsBlock.CurrentStats;

            Health?.SetMaxHealth(StatsCalculator.MaxHealth(stats, _level), true);

            if (Stamina != null)
            {
                int maxStamina = StatsCalculator.MaxStamina(stats);
                Stamina.SetMaxStamina(maxStamina, true);
                Stamina.SetRegenRate(StatsCalculator.StaminaRegenRate(stats, maxStamina));
            }

            if (Mana != null)
            {
                int maxMana = StatsCalculator.MaxMana(stats);
                Mana.SetMaxMana(maxMana, true);
                Mana.SetRegenRate(StatsCalculator.ManaRegenRate(stats, maxMana));
            }

            Shield?.SetMaxShield(StatsCalculator.MaxShield(stats), true);
            Defense?.SetBaseDefense(StatsCalculator.BaseDefense(stats));
            Xp?.Initialize(_level);
        }

        protected virtual void HandleLevelUp(int newLevel)
        {
            if (!HasStats) return;
            _level = newLevel;
            _statsBlock.UpdateStats(_level);
            ApplyStatsToComponents();
            NotifyEntityChanged();
        }

        protected virtual void HandleEquipmentChanged(IEquipmentUser equipmentUser)
        {
            if (!HasStats) return;
            if (equipmentUser is ManagerEquipment manager)
            {
                Stats equipmentStats = manager.GetTotalStats();
                _statsBlock.ApplyEquipmentBonuses(equipmentStats);
                ApplyStatsToComponents();
                NotifyEntityChanged();
            }
        }

        public void RecalculateStats()
        {
            if (!HasStats) return;
            _statsBlock.UpdateStats(_level);
            ApplyStatsToComponents();
            NotifyEntityChanged();
        }
        #endregion

        #region IEffectReceiver Implementation
        public void ApplyEffect(ConsumableEffectType effectType, int amount, float duration, GameObject owner = null)
        {
            switch (effectType)
            {
                case ConsumableEffectType.Health:
                    if (amount > 0)
                        Health?.Heal(amount);
                    else if (amount < 0)
                        Health?.TakeDamage(-amount, owner); // negative amount = damage, pass owner for kill credit
                    break;

                case ConsumableEffectType.Stamina:
                    if (amount > 0)
                        (Stamina as StaminaComponent)?.RegenerateStamina(amount);
                    else
                        (Stamina as StaminaComponent)?.UseStamina(-amount);
                    break;

                case ConsumableEffectType.Mana:
                    if (amount > 0)
                        (Mana as ManaComponent)?.RegenerateMana(amount);
                    else
                        (Mana as ManaComponent)?.UseMana(-amount);
                    break;

                // Add other effect types as needed (Strength buff, etc.)
                default:
                    Debug.LogWarning($"Unhandled effect type: {effectType}");
                    break;
            }
        }
        #endregion

        #region Death Handling
        private void HandleDeath(GameObject killer)
        {
            OnEntityDied?.Invoke(this, killer);
        }

        public virtual void Die(GameObject killer = null)
        {
            Health?.TakeDamage(Health.CurrentHealth, killer);
        }
        #endregion

        protected void NotifyEntityChanged() => OnEntityChanged?.Invoke(this);
        public void SetEntityName(string newName) { _entityName = newName; NotifyEntityChanged(); }
    }
}