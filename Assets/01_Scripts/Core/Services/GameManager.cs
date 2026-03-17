using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace AniDrag.Core
{
     public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Service Prefabs")]
        [SerializeField] private GameObject inputServicePrefab;
        [SerializeField] private GameObject gameStateServicePrefab;
        [SerializeField] private GameObject sceneServicePrefab;
        [SerializeField] private GameObject settingsServicePrefab;

        [Header("Player")]
        public GameObject PlayerPrefab;
        public List<Transform> spawnpoints = new List<Transform>();

        private List<GameObject> _players = new List<GameObject>();

        private WorldTickService _worldTick;

        private void FixedUpdate()
        {
            _worldTick?.Tick(Time.deltaTime);
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            Services.RegisterEventBus(new EventBus());
            
            _worldTick = new WorldTickService();
            Services.RegisterWorldTick(_worldTick);
            

            InitializeServices();
            SceneManager.sceneLoaded += OnSceneLoaded;
            SpawnPlayerIfNeeded();
        }

        private void InitializeServices()
        {
            IInputService input = GetComponentInChildren<IInputService>();
            if (input == null && inputServicePrefab != null)
                input = Instantiate(inputServicePrefab, transform).GetComponent<IInputService>();

            IGameStateService gameState = GetComponentInChildren<IGameStateService>();
            if (gameState == null && gameStateServicePrefab != null)
                gameState = Instantiate(gameStateServicePrefab, transform).GetComponent<IGameStateService>();

            ISceneService scene = GetComponentInChildren<ISceneService>();
            if (scene == null && sceneServicePrefab != null)
                scene = Instantiate(sceneServicePrefab, transform).GetComponent<ISceneService>();

            ISettingsService settings = GetComponentInChildren<ISettingsService>();
            if (settings == null && settingsServicePrefab != null)
                settings = Instantiate(settingsServicePrefab, transform).GetComponent<ISettingsService>();

            if (input != null) Services.RegisterInput(input);
            else Debug.LogError("Failed to register Input Service.");

            if (gameState != null) Services.RegisterGameState(gameState);
            else Debug.LogError("Failed to register GameState Service.");

            if (scene != null) Services.RegisterScene(scene);
            else Debug.LogError("Failed to register Scene Service.");

            if (settings != null) Services.RegisterSettings(settings);
            else Debug.LogError("Failed to register Settings Service.");

            foreach (var initializable in GetComponentsInChildren<IInitializableService>())
            {
                initializable.Initialize();
            }
        }

        private void SpawnPlayerIfNeeded()
        {
            _players.RemoveAll(p => p == null);
            if (_players.Count > 0) return;

            if (PlayerPrefab == null)
            {
                Debug.LogError("PlayerPrefab not assigned in GameManager!");
                return;
            }

            Transform spawn = GetRandomSpawnpoint();
            if (spawn == null) return;

            GameObject player = Instantiate(PlayerPrefab, spawn.position, spawn.rotation);
            _players.Add(player);
        }

        private Transform GetRandomSpawnpoint()
        {
            if (spawnpoints == null || spawnpoints.Count == 0)
            {
                Debug.LogWarning("No spawnpoints assigned!");
                return null;
            }
            int index = Random.Range(0, spawnpoints.Count);
            return spawnpoints[index];
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SpawnPlayerIfNeeded();

            if (_players.Count > 0)
            {
                Transform spawn = GetRandomSpawnpoint();
                if (spawn != null)
                    _players[0].transform.SetPositionAndRotation(spawn.position, spawn.rotation);
            }
        }

        private void OnDestroy()
        {
            Services.RegisterWorldTick(null); 
            Services.RegisterEventBus(null);
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}