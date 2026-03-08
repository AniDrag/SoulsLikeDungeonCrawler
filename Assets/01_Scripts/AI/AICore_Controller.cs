using UnityEngine;
using UnityEngine.AI;
using AniDrag.WeaponPack;
using AniDrag.CharacterComponents;
namespace AniDrag.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(AICore_Sense))]
    [RequireComponent(typeof(Entity))]
    public class AICore_Controller : MonoBehaviour
    {
        [Header("Components")]
        public AICore_Sense sense;
        public NavMeshAgent agent { get; private set; }
        public Entity entity { get; private set; }

        [Header("Targeting")]
        public AICore_TargetingStrategy targetingStrategy;
        private bool wasInAttackRange = false;

        [Header("Movement Strategy")]
        public AICore_MovementStrategy movementStrategy;

        [Header("Attack Strategy")]
        public AICore_AttackStrategy attackStrategy;
        public WeaponCore weapon;                  // Reference to the AI's weapon (assign in Inspector or instantiate)
        public float attackRange = 2f;

        [Header("Movement Fallback (used if no movement strategy)")]
        public float wanderRadius = 10f;
        public float wanderTimer = 5f;
        public float detectionStopDistance = 2f;

        [Header("Optimization")]
        [Tooltip("How often (in seconds) the AI updates its senses. Higher = less CPU.")]
        public float senseUpdateInterval = 0.2f;
        [Tooltip("If distance to player > this, sense updates are skipped entirely.")]
        public float maxUpdateDistance = 50f;

        [Header("Target Memory")]
        public float targetMemoryDuration = 0.5f;

        // Private fields
        private float timer;
        private Transform currentTarget;
        private float targetLostTime = 0f;
        private float lastSenseUpdateTime = 0f;
        private Transform player;   // cached for distance culling

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            sense = GetComponent<AICore_Sense>();
            entity = GetComponent<Entity>();
            if (GameManager.Instance != null && GameManager.Instance.Players.Count > 0)
                player = GameManager.Instance.Players[0].transform;

            if (targetingStrategy != null)
            {
                targetingStrategy = Instantiate(targetingStrategy);
                targetingStrategy.Initialize(this);
            }

            if (movementStrategy != null)
            {
                movementStrategy = Instantiate(movementStrategy);
                movementStrategy.Initialize(this);
            }

            if (attackStrategy != null)
            {
                attackStrategy = Instantiate(attackStrategy);
                attackStrategy.Initialize(this);
            }
            if (entity != null)
                entity.onEntityDeath.AddListener(() => enabled = false); // disable AI on death

            if (weapon == null)
                weapon = GetComponentInChildren<WeaponCore>();
            agent.stoppingDistance = attackRange - 1;
            SetNewWanderDestination();
        }

        void Update()
        {
            // Distance culling – if player is too far, skip senses and just wander
            if (player != null && Vector3.Distance(transform.position, player.position) > maxUpdateDistance)
            {
                // We still want to wander, but we can do it less often
                if (Time.time - lastSenseUpdateTime >= 0.1f) // arbitrary throttle
                {
                    currentTarget = null; // forget any target
                    HandleNoTarget();     // wander
                    lastSenseUpdateTime = Time.time;
                }
                return; // skip the rest of Update
            }

            // Throttled sense update
            if (Time.time - lastSenseUpdateTime >= senseUpdateInterval)
            {
                sense.Core_Update();
                lastSenseUpdateTime = Time.time;
            }

            // Targeting
            Transform newlyDetected = null;
            if (targetingStrategy != null)
                newlyDetected = targetingStrategy.UpdateTarget(this);
            else
                newlyDetected = sense.detectedTargets.Count > 0 ? sense.detectedTargets[0].transform : null;

            if (newlyDetected != null)
            {
                currentTarget = newlyDetected;
                targetLostTime = 0f;
            }
            else
            {
                if (currentTarget != null)
                {
                    targetLostTime += Time.deltaTime;
                    if (targetLostTime >= targetMemoryDuration)
                        currentTarget = null;
                }
            }

            // Behavior based on target
            if (currentTarget != null)
                HandleTargetDetected();
            else
                HandleNoTarget();
        }

        void HandleTargetDetected()
        {
            // 1. Movement
            if (movementStrategy != null)
                movementStrategy.UpdateMovement(this, currentTarget);
            else
            {
                // fallback simple movement
                float distance = Vector3.Distance(transform.position, currentTarget.position);
                if (distance <= detectionStopDistance)
                    agent.isStopped = true;
                else
                {
                    agent.isStopped = false;
                    agent.SetDestination(currentTarget.position);
                }
            }

            // 2. Attack logic
            if (attackStrategy != null && weapon != null)
            {
                float distToTarget = Vector3.Distance(transform.position, currentTarget.position);
                bool inRange = distToTarget <= attackRange;

                if (inRange)
                {
                    if (!wasInAttackRange)
                    {
                        attackStrategy.OnEnterAttackRange(this, currentTarget);
                        wasInAttackRange = true;
                    }
                    attackStrategy.UpdateAttack(this, currentTarget);
                }
                else
                {
                    if (wasInAttackRange)
                    {
                        attackStrategy.OnExitAttackRange(this, currentTarget);
                        wasInAttackRange = false;
                    }
                }
            }
        }

        void HandleNoTarget()
        {
            agent.isStopped = false; // ensure agent can move

            timer += Time.deltaTime;
            if (timer >= wanderTimer)
            {
                SetNewWanderDestination();
                timer = 0f;
            }
        }

        void SetNewWanderDestination()
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += transform.position;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
                agent.SetDestination(hit.position);
        }

        // Optional: disable script when off-screen to save performance
        void OnBecameVisible() { enabled = true; }
        void OnBecameInvisible() { enabled = false; }
    }
}