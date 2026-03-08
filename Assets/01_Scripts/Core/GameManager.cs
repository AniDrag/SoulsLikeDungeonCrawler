using UnityEngine;
using System.Collections.Generic;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public GameObject PlayerPrefab;
    public List<GameObject> Players = new List<GameObject>();
    public List<Transform> spawnpoints = new List<Transform>();

    public void SpawnPlayer()
    {
        int rnd = Random.Range(0, spawnpoints.Count - 1);

        GameObject player = Instantiate(PlayerPrefab, spawnpoints[rnd].position, Quaternion.identity);
        Players.Add(player);
    }
}
