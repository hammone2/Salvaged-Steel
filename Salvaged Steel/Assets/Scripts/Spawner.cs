using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Enemy prefab to spawn
    public GameObject enemyPrefab;

    // List of spawn points
    public Transform[] spawnPoints;

    // Minimum and maximum time between spawns (randomized)
    public float minSpawnCooldown = 2f;
    public float maxSpawnCooldown = 5f;

    public bool isSpawning = false;

    // Cooldown for each spawn point (to prevent double-spawning from same point)
    private float[] spawnPointCooldowns;

    // Start is called before the first frame update
    void Start()
    {
        spawnPointCooldowns = new float[spawnPoints.Length];

        // Start spawning enemies
        StartCoroutine(SpawnEnemies());
    }

    // Coroutine to spawn enemies at random times
    private IEnumerator SpawnEnemies()
    {
        //if (!isSpawning)
            //return;
        // Keep spawning enemies
        while (true)
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

            // Wait for a frame before checking again
            yield return null;
        }
    }

    // Spawn an enemy at a specific spawn point
    private void SpawnEnemy(int spawnPointIndex)
    {
        // Instantiate the enemy at the chosen spawn point's position and rotation
        Instantiate(enemyPrefab, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
        Debug.Log("Enemy spawned at spawn point: " + spawnPointIndex);
    }

    public void BeginSpawning()
    {
        isSpawning = true;
    }
}