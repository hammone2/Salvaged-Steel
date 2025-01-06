using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spawner : MonoBehaviourPun
{
    // Enemy prefab to spawn
    public List<string> enemyPrefabList = new List<string>();

    // List of spawn points
    public Transform[] spawnPoints;

    // Minimum and maximum time between spawns (randomized)
    public float minSpawnCooldown = 2f;
    public float maxSpawnCooldown = 5f;

    public bool isSpawning = true;

    // Cooldown for each spawn point (to prevent double-spawning from same point)
    private float[] spawnPointCooldowns;

    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        spawnPointCooldowns = new float[spawnPoints.Length];
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (isSpawning)
            SpawnEnemies();
    }

    // Coroutine to spawn enemies at random times
    private void SpawnEnemies()
    {
        // Check all spawn points and attempt to spawn at each one
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPointCooldowns[i] <= 0) // If cooldown is finished, spawn an enemy
            {
                SpawnEnemy(i);  // Spawn an enemy at the i-th spawn point
                // Set random cooldown for the spawn point
                spawnPointCooldowns[i] = Random.Range(minSpawnCooldown, maxSpawnCooldown);
            }
            else
            {
                // Reduce cooldown for each spawn point
                spawnPointCooldowns[i] -= Time.deltaTime;
            }
        }  
    }

    // Spawn an enemy at a specific spawn point
    private void SpawnEnemy(int spawnPointIndex)
    {
        // Get a random index from the enemyPrefabList
        int randomIndex = Random.Range(0, enemyPrefabList.Count);

        // Get the random prefab path
        string randomPrefabPath = enemyPrefabList[randomIndex];

        GameObject enemy = PhotonNetwork.Instantiate(randomPrefabPath, spawnPoints[spawnPointIndex].position, Quaternion.identity);
    }
}