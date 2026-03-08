using UnityEngine;
using System.Collections.Generic;
using AniDrag.Utility;
namespace AniDrag.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public CameraSettings cameraSettings;
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SpawnPlayer();
        }

        public GameObject PlayerPrefab;
        public List<GameObject> Players = new List<GameObject>();
        public List<Transform> spawnpoints = new List<Transform>();

        [Button]
        public void SpawnPlayer()
        {
            if(Players.Count > 0) return;
            int rnd = Random.Range(0, spawnpoints.Count - 1);

            GameObject player = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
            Players.Add(player.transform.GetChild(0).gameObject);
        }
        public void OnLevelWasLoaded(int level)
        {
            foreach(var player in Players)
            {
                player.transform.position = Vector3.zero;//spawnpoints[Random.Range(0, spawnpoints.Count - 1)].position;
            }
        }
    }
}