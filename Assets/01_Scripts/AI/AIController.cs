using AniDrag.CharacterComponents;
using AniDrag.Core;
using UnityEngine;

namespace AniDrag.AI
{
    /// <summary>
    /// Central AI controller. Inherits from your FSM class.
    /// Coordinates perception, memory, combat, movement, and strategies.
    /// </summary>

    [RequireComponent(typeof(Entity))]
    [RequireComponent(typeof(AIPerception))]
    [RequireComponent(typeof(AIMemory))]
    [RequireComponent(typeof(AICombat))]
    public class AIController : FSM
    {
        [Header("Core References")]
        public Entity entity;

        [Header("AI Components")]
        public AIPerception perception;
        public AIMemory memory;
        public AICombat combat;
        public AIMovement movement;

        [Header("Strategies (ScriptableObjects)")]
        public AITargetingStrategy targetingStrategy;
        public AICombatStrategy combatStrategy;
        // Optional: movement strategy for patrol points, etc.
        public AIMovementStrategy movementStrategy;

        [Header("Settings")]
        [Tooltip("How often to update perception and targeting (seconds).")]
        public float updateInterval = 0.2f;
        private float nextUpdateTime;

        // Create state instances (pass controller and animator)
        IdleState idle;
        PatrolState patrol;
        ChaseState chase;
        AttackState attack;
        SearchState search;

        protected void Awake()
        {
            SetupFSM();
            InitializeAI();
        }

        private void InitializeAI()
        {
            // Auto-find missing components
            if (entity == null) entity = GetComponent<Entity>();
            if (perception == null) perception = GetComponent<AIPerception>();
            if (memory == null) memory = GetComponent<AIMemory>();
            if (combat == null) combat = GetComponent<AICombat>();
            if (movement == null) movement = GetComponent<AIMovement>();

            // Initialize strategies with this AI reference
            targetingStrategy?.Initialize(this);
            combatStrategy?.Initialize(this);
            movementStrategy?.Initialize(this);

            // Initialize combat with the currently equipped weapon (if any)
            // You'll need to get the weapon from EquipmentManager or similar.
            // For now, we assume the weapon is set elsewhere (e.g., on spawn).
            // If you have a way to get the equipped weapon, do it here:
            // WeaponCore weapon = GetComponentInChildren<WeaponCore>();
            // if (weapon != null) combat.Initialize(weapon);

            SetupStates();
        }

        private void SetupStates()
        {
            // Create state instances (pass controller and animator)
            idle = new IdleState(this, animator);
            patrol = new PatrolState(this, animator);
            chase = new ChaseState(this, animator);
            attack = new AttackState(this, animator);
            search = new SearchState(this, animator);

            // Add transitions using your FSM's methods
            AddAnyTransition(chase, new FuncPredicate(FoundTarget));

            AddTransition(idle, patrol, new FuncPredicate(GoToNewPatrolPoint));

            AddTransition(patrol, chase, new FuncPredicate(FoundTarget));

            AddTransition(chase, attack, new FuncPredicate(InAttackRange));

            AddTransition(attack, chase, new FuncPredicate(() => !InAttackRange()));

            AddTransition(chase, search, new FuncPredicate(NoTarget));

            AddTransition(search, patrol, new FuncPredicate(LostTarget));

            // Set initial state
            SetState(idle);
            Debug.Log("AIController: FSM states and transitions set up. curre");
        }

        private bool InAttackRange()
        {
            if (memory.currentTarget == null) return false;
            float dist = Vector3.Distance(transform.position, memory.currentTarget.transform.position);
            return dist <= combatStrategy.GetAttackRange(this);
        }

        protected override void Update()
        {
            base.Update(); // Let FSM handle state updates

            if (Time.time >= nextUpdateTime)
            {
                nextUpdateTime = Time.time + updateInterval;
                UpdateAI();
            }
        }

        private void UpdateAI()
        {
            perception.Core_Update();
            memory.MemoryUpdate(perception.detectedTargets);
            targetingStrategy?.UpdateTarget(this, memory);
        }

        // Public methods for states to use
        public void MoveTo(Vector3 position) => movement.MoveTo(position);
        public void StopMoving() => movement.Stop();
        public void FaceTarget(Entity target) => movement.FaceTarget(target);
        public void Attack(Entity target) => combatStrategy?.Attack(this, target);

        // Conditional methods
        bool FoundTarget() => memory.currentTarget != null;
        bool LostTarget(){ 
            return Time.time >= search.searchEndTime || movement.HasReachedDestination(); 
        }
        bool NoTarget() => memory.currentTarget == null;
        bool GoToNewPatrolPoint() => Time.time >= idle.waitEndTime;
    }
}