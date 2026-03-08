using UnityEngine;
using UnityEngine.Events;
using AniDrag.Core;
using AniDrag.Quest;

namespace AniDrag.CharacterComponents
{
    public class CharacterEntity : Entity
    {
        [Header("========================\n" +
       "    Character Entity details      \n" +
       "========================")]
        [field: SerializeField, Range(1, 200)] public int entityLevel { get; private set; } = 1;// player cap is 100 but some enemies might be higher level.
        [field: SerializeField] public CharacterClass entityClass { get; private set; } // will be mostly used for class.Neme after initialization of the entity.
        [field: SerializeField] public StatsBlock entityStats { get; private set; } // only visible in inspector for debugging. !FUTURE: Remove [field: SerializeField] after implementing stat generation system.

        [Header("========================\n" +
       "    Character Entity Refrences      \n" +
       "========================")]
        [field: SerializeField] public ManagerEquipment entityEquipment { get; private set; }
        [field: SerializeField] public XpComponent entityXp { get; private set; }


        protected override void Initialize()
        {
            HealthInitialization();
            entityStats = new StatsBlock(entityClass.baseStats, entityClass.growthFactors);
            entityEquipment = GetComponent<ManagerEquipment>();

            //equipment component initialization
            if (entityEquipment != null)
            {
                //entityEquipment.OnEquipmentChanged += OnEquipmentChanged;
                entityEquipment.Initialized();
            }
            else
            {
                Debug.LogWarning($"{entityName} does not have an EquipmentManager attached.");
            }
            // Xp component initialization
            if (entityXp != null)
            {
                entityXp.InitializeXpComponent(entityLevel);
                entityXp.onLevelUp.AddListener(OnLevelUp);
            }
            // Health component initialization
            if (entityHealth != null)
            {
                entityHealth.InitializeHealthComponent(entityStats, entityLevel);
            }
            else
            {
                Debug.LogWarning($"{entityName} does not have a HealthComponent attached.");
            }

            InvokeEntityChanges();
        }
        public virtual void OnLevelUp(int level)
        {
            entityLevel = level;
            BaseEntityLevelUpDetails();
            InvokeEntityChanges();
        }
        protected void BaseEntityLevelUpDetails()
        {
            entityStats.UpdateStats(entityLevel);
            entityHealth.UpdateHealth(entityStats, entityLevel);
        }
        public virtual void OnEquipmentChanged(ManagerEquipment managerEquipment)
        {
            //entityStats.UpdateStats(entityLevel, managerEquipment.totalEquipmentStats);
            entityHealth.UpdateHealth(entityStats, entityLevel);
            InvokeEntityChanges();
        }
        protected void SetEntityLevel(int level) => entityLevel = level;
        public override void OnDeath(GameObject owner)
        {
            base.OnDeath(owner);

            QuestBus.Instance.Enqueue(new DeathEvent(this, owner.GetComponent<Entity>()));

            if (owner != null)
            {
                var xpComp = owner.GetComponent<XpComponent>();
                if (xpComp != null)
                {
                    xpComp.GainXp(100 * entityLevel);
                    Debug.Log($"{entityName} has died and granted XP to the killer: {owner.name}.");
                }
                else
                {
                    Debug.LogWarning($"Owner {owner.name} has no XpComponent. No XP granted.");
                }
            }
            else
            {
                Debug.LogWarning($"{entityName} died but owner is null. No XP granted.");
            }

        }
    }
}