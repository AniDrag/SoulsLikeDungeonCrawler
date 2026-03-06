using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AniDrag.Core;
/// <summary>
/// Remember it has a Editor file so make sure u dont change names without ctrl + R,
/// also make sure to assign the spawn points and enemy prefabs in the inspector, and set the ground layers for proper placement. 
/// The spawner will handle the rest!
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpawnInfo
    {
        public GameObject enemyPrefab;
        [Range(0.01f, 100)] public float spawnChance;          // weight for random selection
    }

    [Header("Spawn Points")]
    [SerializeField] public List<Transform> spawnpoints = new List<Transform>();
    [SerializeField] public bool showSpawnPoints = true;      // toggle gizmos

    [Header("Enemy Pool")]
    [SerializeField] public List<EnemySpawnInfo> enemiesToSpawn = new List<EnemySpawnInfo>();

    [Header("Spawn Limits")]
    [Tooltip("Maximum enemies alive at the same time")]
    [SerializeField] public int maxEnemiesSpawned = 5;
    [Tooltip("Total enemies to spawn before stopping")]
    [SerializeField] public int totalEnemiesToSpawn = 20;
    [SerializeField, Range(1, 10)] public float spawnInterval = 2f;
    [SerializeField, Range(1, 150)] public int enemyLevel = 1;

    [Header("Editor Placement")]
    [SerializeField] public float groundPlaneY = 0f;          // fallback Y when no geometry hit
    [Tooltip("Layers considered as ground for placement and snapping")]
    [SerializeField] public LayerMask groundLayers = ~0;      // Everything by default

    private int enemiesSpawnedCount = 0;
    private int currentEnemyCount = 0;


    private void Start()
    {
        if (enemiesToSpawn.Count == 0 || spawnpoints.Count == 0)
        {
            Debug.LogWarning("EnemySpawner: No enemies or spawnpoints assigned.");
            return;
        }
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (enemiesSpawnedCount < totalEnemiesToSpawn)
        {
            yield return new WaitForSeconds(spawnInterval);
            if (currentEnemyCount < maxEnemiesSpawned)
            {
                SpawnEnemy();
            }
        }
    }

    private void SpawnEnemy()
    {
        // Choose random spawn point
        Transform spawnPoint = spawnpoints[UnityEngine.Random.Range(0, spawnpoints.Count)];

        // Choose enemy prefab using weighted random
        GameObject prefab = GetWeightedRandomEnemy();
        if (prefab == null) return;

        GameObject enemy = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        enemiesSpawnedCount++;
        currentEnemyCount++;

        // Try to set enemy level (optional – you can remove this if you handle it elsewhere)
        // For example, if your enemy has a SetLevel method from an interface like IEnemyLevel.
        // var levelSetter = enemy.GetComponent<IEnemyLevel>();
        // if (levelSetter != null) levelSetter.SetLevel(enemyLevel);

        // Subscribe to death event via IDamagable
        var damagable = enemy.GetComponent<IDamagable>();
        if (damagable != null)
        {
            // Use a local variable to capture the enemy instance correctly
            IDamagable capturedDamagable = damagable;
            Action handler = null;
            handler = () =>
            {
                currentEnemyCount--;
                capturedDamagable.DeathEvent -= handler;
            };
            damagable.DeathEvent += handler;
        }
        else
        {
            Debug.LogWarning($"Enemy {enemy.name} does not implement IDamagable. Its death will not affect the spawn limit.", enemy);
        }
    }

    private GameObject GetWeightedRandomEnemy()
    {
        float totalWeight = 0f;
        foreach (var info in enemiesToSpawn)
            totalWeight += info.spawnChance;

        float rand = UnityEngine.Random.Range(0, totalWeight);
        float cumulative = 0f;
        foreach (var info in enemiesToSpawn)
        {
            cumulative += info.spawnChance;
            if (rand <= cumulative)
                return info.enemyPrefab;
        }
        return enemiesToSpawn.Count > 0 ? enemiesToSpawn[0].enemyPrefab : null;
    }

    private void OnDrawGizmos()
    {
        if (!showSpawnPoints) return;
        Gizmos.color = Color.green;
        foreach (Transform point in spawnpoints)
        {
            if (point != null)
                Gizmos.DrawSphere(point.position, 0.3f);
        }
    }
}