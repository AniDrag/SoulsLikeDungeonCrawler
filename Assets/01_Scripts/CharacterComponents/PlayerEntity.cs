using AniDrag.Core;
using UnityEngine;
namespace AniDrag.CharacterComponents
{
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(StaminaComponent))]
    [RequireComponent(typeof(XpComponent))]
    [RequireComponent(typeof(ManagerEquipment))]
    public class PlayerEntity : CharacterEntity, IEffectReceiver
    {
        [Header("========================\n" +
      "    Player Entity Refrences      \n" +
      "========================")]
        [field: SerializeField] public StaminaComponent entityStamina { get; private set; }
        protected override void Initialize()
        {
            base.Initialize();
            entityStamina = GetComponent<StaminaComponent>();

            if (entityStamina != null)
            {
                entityStamina.InitializeStaminaComponent(entityStats);
                //entityStamina.updateStamina += (stamina) => OnEntityChanges?.Invoke(this);
            }
            else
            {
                Debug.LogWarning($"{entityName} does not have a StaminaComponent attached.");
            }
        }
        public override void OnLevelUp(int level)
        {
            SetEntityLevel(level);
            BaseEntityLevelUpDetails();
            PlayerEntityLevelUpEevent();
            InvokeEntityChanges();
        }
        public void PlayerEntityLevelUpEevent()
        {
            entityStats.UpdateStats(entityLevel);
            entityHealth.UpdateHealth(entityStats, entityLevel);
            entityStamina.UpdateStamina(entityStats);
        }
        public void ApplyEffect(ConsumableEffectType effectType, int amount, float duration)
        {
            switch (effectType)
            {
                case ConsumableEffectType.Health:
                    if (amount > 0)
                        entityHealth.RegenerateHealth(amount);
                    else
                        entityHealth.TakeDamage(-amount); // negative amount = damage
                    break;
                case ConsumableEffectType.Stamina:
                    // If you have a stamina component, use it
                    entityStamina?.RegenerateStamina(amount);
                    break;
                    // ... other effects ...
            }
        }
    }
}