using System;
using UnityEngine;
using UnityEngine.Events;
using AniDrag.Core;

namespace AniDrag.CharacterComponents
{
    /// <summary>
    /// All entities in the game (players, NPCs, enemies) derive from this class dirivitive character Entity.
    /// Distructable objects with lower complexity might also use this directly. like boxes, barrels, etc.
    /// They contain basic information like name, class, stats, and health.
    /// Would be marked as abstract if this woould have been only a blueprint for other entities but some enteties might use this directly.
    /// </summary>
    public class Entity : MonoBehaviour // mybe interace IStatBlock here ?
    {
        [Header("========================\n" +
      "    Primitive Refrences      \n" +
      "========================")]
        [field: SerializeField] public HealthComponent entityHealth { get; private set; }

        [Header("========================\n" +
      "    Primitive Entity Details      \n" +
      "========================")]
        [field: SerializeField] public string entityName { get; private set; }

        [Header("========================\n" +
     "    Primitive Connector Events      \n" +
     "========================")]
        public UnityEvent onEntityDeath;
        // ------------------------------------------------------------------------------------------------------------------
        // Actions ----------------------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------------------------------

        public event Action<Entity> OnEntityChanges;// will send info to UI and Then probably to network manager in future.

        #region Getters / Setters
        public void SetEntityName(string pName) => entityName = pName;
        protected void InvokeEntityChanges() => OnEntityChanges?.Invoke(this);// used to notify changes to ui and is protected so subclasses can call it after changes.

        #endregion

        protected virtual void Awake()
        {
            Initialize();
        }

        #region Shared Functions
        protected virtual void Initialize()
        {
           HealthInitialization();
            InvokeEntityChanges();
        }
       
        // This is called by the HealthComponent when health reaches zero.
        public virtual void OnDeath()
        {
            Debug.Log($"{entityName} has died.");
            // Additional death logic can be implemented in derived classes.
            InvokeEntityChanges();
            this.gameObject.SetActive(false);
        }
       
        #endregion
        protected void HealthInitialization()// base health initialization for all entities.
        {
            entityHealth = GetComponent<HealthComponent>();
            if (entityHealth != null)
            {
                entityHealth.onDeath.AddListener(OnDeath);
            }
        }


    }
}
