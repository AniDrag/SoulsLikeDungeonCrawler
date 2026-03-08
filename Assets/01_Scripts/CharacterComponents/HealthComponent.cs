using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using AniDrag.Utility;
using AniDrag.Core;
using UnityEngine.UI;
namespace AniDrag.CharacterComponents
{
   
    /// <summary>
    /// This component manages the health and shield of a character or object.
    /// It can be attached to any GameObject that requires health management / is destructible.
    /// </summary>
    public class HealthComponent : MonoBehaviour, IDamagable
    {
        [Header("========================\n" +
                "    Debug      \n" +
                "========================")]
        [SerializeField] private bool HAS_CHARACTER_COMPONENT = false;
        [SerializeField] private bool USE_SHILD = false;
        [SerializeField] private bool USE_REGENERATION = false;
        [SerializeField] private float regenDelay = 10f; // Stop regen for X seconds after damage
        [SerializeField] private float regenInterval = 1f; // How often regen ticks (every 1 second)

        [Header("========================\n" +
      "    Health details      \n" +
      "========================")]
        [field: SerializeField] public bool regenerateHealth;
        [field: SerializeField] public int maxHealth { get; private set; } = 100;
        public int currentHealth { get; private set; }
        [SerializeField] private int healtRegen = 25;

        [Header("========================\n" +
      "    Shild details      \n" +
      "========================")]
        [field: SerializeField] public bool regenerateShild;
        [field: SerializeField] public int maxShield { get; private set; } = 100;
        public int currentShield { get; private set; }

        [Header("========================\n" +
      "    Defense details      \n" +
      "========================")]
        [field: SerializeField] public int defense { get; private set; } = 1;

        [Header("========================\n" +
      "    Event Connectors      \n" +
      "========================")]
        public UnityEvent<GameObject> onDeath;
        public UnityEvent<int> onHealthChange;



        // ------------------------------------------------------------------------------------------------------------------
        // Private variables ------------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------------------------------
        private bool tookDamage;
        private bool isRegenerating = false;
        private Coroutine regenRoutine;
        private float lastDamageTime = -999f;


        // ------------------------------------------------------------------------------------------------------------------
        // Actions ----------------------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------------------------------
        public Action<HealthComponent> onHealthChanged;
        public Action DeathEvent { get; set; }





        public void InitializeHealthComponent(int vit, int level)
        {
            maxHealth = 100 + vit * (25 + level);
            defense = vit * 2;
            maxShield = 100;
            currentHealth = maxHealth;
            currentShield = maxShield;
            healtRegen = maxHealth / 100;// 1% of max health per heal
            TryStartRegen();
            onHealthChange?.Invoke(currentHealth);
        }
        public void InitializeHealthComponent(Stats data, int level)
        {
            maxHealth = 100 + data.VIT * (25 + level);
            defense = data.VIT * 2;
            maxShield = 100;
            currentHealth = maxHealth;
            currentShield = maxShield;
            healtRegen = maxHealth / 100;// 1% of max health per heal
            TryStartRegen();
            onHealthChange?.Invoke(currentHealth);
        }

        void Start()
        {
            if (!HAS_CHARACTER_COMPONENT)
            {
                currentHealth = maxHealth;
                currentShield = maxShield;
                TryStartRegen();
            }
        }

        #region bools 
        public bool AtMaxHealth => currentHealth >= maxHealth;
        public bool AtMaxShield => currentShield >= maxShield;
        public bool IsDead => currentHealth <= 0;
        #endregion

        #region Public API
        public void AffectHealth(int amount, bool damage = false)
        {
            if(!damage)
            RegenerateHealth(amount);
            else
                TakeDamage(amount);
            Debug.Log($"Health now: {currentHealth}, Shield: {currentShield}");
        }
        public void RegenerateHealth(int amount)
        {
            if (amount <= 0) amount = maxHealth / 100;
            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
            onHealthChanged?.Invoke(this);
            onHealthChange?.Invoke(currentHealth);
        }
        public void RegenerateShield(int amount)
        {
            if (amount <= 0) amount = maxShield / 100;
            currentShield = Mathf.Min(currentShield + amount, maxShield);
            onHealthChanged?.Invoke(this);
        }
        public void TakeDamage(int amount, GameObject owner = null)
        {
            // Stop regen timer
            lastDamageTime = Time.time;

            // If regen is active, stop it
            if (isRegenerating)
            {
                StopCoroutine(regenRoutine);
                isRegenerating = false;
            }
            int finalDamage = DamageCalculation(amount);
            currentHealth = Mathf.Max(currentHealth - finalDamage, 0);

            if (currentHealth <= 0)
            {
                DeathEvent?.Invoke();
                onDeath?.Invoke(owner);
            }

            onHealthChanged?.Invoke(this);
            onHealthChange?.Invoke(currentHealth);

            // Restart regen monitoring
            TryStartRegen();
#if UNITY_EDITOR
            Debug.Log(
                $"Character/Object: {this.gameObject.name} took {finalDamage} damage.\n"+
                $"Health now: {currentHealth}, Shield: {currentShield}"
            );
#endif
        }

        public void UpdateHealth(int vit, int level)
        {
            maxHealth = 100 + vit * (25 + level);
            defense = vit * 2;

            // Optionally, fully heal and recharge shield on level up
            maxShield = maxShield;
            currentHealth = maxHealth;
            currentShield = maxShield;
            healtRegen = maxHealth / 100;// 1% of max health per heal
            // Notify the UI about health and shield changes
            onHealthChanged?.Invoke(this);
            onHealthChange?.Invoke(currentHealth);
        }
        public void UpdateHealth(Stats data, int level)
        {
            maxHealth = 100 + data.VIT * (25 + level);
            defense = data.VIT * 2;

            // Optionally, fully heal and recharge shield on level up
            maxShield = maxShield;
            currentHealth = maxHealth;
            currentShield = maxShield;
            healtRegen = maxHealth / 100;// 1% of max health per heal
            // Notify the UI about health and shield changes
            onHealthChanged?.Invoke(this);
            onHealthChange?.Invoke(currentHealth);
        }

        #endregion

        #region Helper Functions
        private IEnumerator RegenRoutine()
        {
            isRegenerating = true;

            // Wait until 10s after last damage
            while (Time.time < lastDamageTime + regenDelay)
                yield return null;

            while (!IsDead)
            {
                bool didSomething = false;

                if (regenerateHealth && currentHealth < maxHealth)
                {
                    RegenerateHealth(healtRegen);
                    didSomething = true;
                }

                if (regenerateShild && currentShield < maxShield)
                {
                    RegenerateShield(maxShield / 100);
                    didSomething = true;
                }

                // Stop regen if nothing is left to regenerate
                if (!didSomething)
                    break;

                yield return new WaitForSeconds(regenInterval);
            }

            isRegenerating = false;
        }
        private void TryStartRegen()
        {
            if (!USE_REGENERATION) return;

            // No regen if dead
            if (IsDead) return;

            // Already regenerating
            if (isRegenerating) return;

            regenRoutine = StartCoroutine(RegenRoutine());
        }
        int DamageCalculation(int amount)
        {
            // 1. Apply defense
            amount = Mathf.Max(amount - defense, 0);

            // 2. Shield absorbs damage first
            if (USE_SHILD && amount > 0)
            {
                int shieldDamage = Mathf.Min(currentShield, amount);
                currentShield -= shieldDamage;
                amount -= shieldDamage;
            }

            // 3. Remaining goes to health
            int finalDamage = amount > 0? amount : 0;



            return finalDamage;
        }
        #endregion
#if UNITY_EDITOR
        [SerializeField] int debugDamage = 50;
        [SerializeField] int debugRegenAmount = 50;

        [Button]
        public void Debug_TakeDamage()
        {
            if (IsDead)
            {
                Debug.Log("Entity is already dead.");
                return;
            }
            TakeDamage(debugDamage);
            Debug.Log($"Health now: {currentHealth}, Shield: {currentShield}");
        }

        [Button]
        public void Debug_Heal()
        {
            if (IsDead)
            {
                Debug.Log("Entity is dead. Cannot heal naturally.");
                return;
            }
            RegenerateHealth(debugRegenAmount);
            Debug.Log($"Health now: {currentHealth}");
        }

        [Button]
        public void Debug_HealShield()
        {
            if (IsDead)
            {
                Debug.Log("Entity is dead. Cannot regenerate shield.");
                return;
            }
            RegenerateShield(debugRegenAmount);
            Debug.Log($"Shield now: {currentShield}");
        }

        [Button]
        public void Debug_StartRegen()
        {
            if (!USE_REGENERATION)
            {
                Debug.Log("Regeneration system disabled.");
                return;
            }

            if (!isRegenerating)
            {
                TryStartRegen();
                Debug.Log("Regen started manually.");
            }
            else
            {
                Debug.Log("Regen already running.");
            }
        }

        [Button]
        public void Debug_StopRegen()
        {
            if (isRegenerating)
            {
                StopCoroutine(regenRoutine);
                isRegenerating = false;
                Debug.Log("Regen stopped manually.");
            }
            else
            {
                Debug.Log("Regen not active.");
            }
        }
#endif
    }
}