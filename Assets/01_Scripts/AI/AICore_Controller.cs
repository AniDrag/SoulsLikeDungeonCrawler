using AniDrag.AI;
using AniDrag.CharacterComponents;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AICore_Sense))]
public class AICore_Controller : MonoBehaviour
{
    [Header("Components")]
    public AICore_Sense sense;
    public NavMeshAgent agent;

    [Header("Targeting")]
    public AICore_TargetingStrategy targetingStrategy;

    [Header("Movement")]
    public float wanderRadius = 10f;
    public float wanderTimer = 5f;
    public float detectionStopDistance = 2f;

    [Header("Optimization")]
    [Tooltip("How often (in seconds) the AI updates its senses. Higher = less CPU.")]
    public float senseUpdateInterval = 0.2f;
    [Tooltip("How often (in seconds) the AI updates its senses when out of range.")]
    public float farWanderInterval = 0.1f;  
    [Tooltip("If distance to player > this, sense updates are skipped entirely (optional).")]
    public float maxUpdateDistance = 50f;

    private float timer;
    private Transform currentTarget;
    private float targetLostTime = 0f;
    public float targetMemoryDuration = 0.5f;

    private float lastSenseUpdateTime = 0f;
    private Transform player;  

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        sense = GetComponent<AICore_Sense>();
        player = GameManager.Instance.Players.Count > 0 ? GameManager.Instance.Players[0].transform : null;

        if (targetingStrategy != null)
        {
            targetingStrategy = Instantiate(targetingStrategy);
            targetingStrategy.Initialize(this);
        }

        SetNewWanderDestination();
    }

    void Update()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) > maxUpdateDistance)
        {
            if (Time.time - lastSenseUpdateTime >= farWanderInterval)
            {
                currentTarget = null;
                HandleNoTarget();
                lastSenseUpdateTime = Time.time;
            }
            return;
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

        // Behavior
        if (currentTarget != null)
            HandleTargetDetected();
        else
            HandleNoTarget();
    }

    void HandleTargetDetected()
    {
        float distance = Vector3.Distance(transform.position, currentTarget.position);
        if (distance <= detectionStopDistance)
        {
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(currentTarget.position);
        }
    }

    void HandleNoTarget()
    {
        agent.isStopped = false;

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

    // Optional: disable the entire script when far away to save even more CPU
    // (enable it again when player approaches)
    void OnBecameVisible() { enabled = true; }
    void OnBecameInvisible() { enabled = false; }
}