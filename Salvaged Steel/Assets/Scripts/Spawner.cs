using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Enemy prefab to spawn
    public List<GameObject> enemyPrefabList = new List<GameObject>();
    public GameObject dropPod;

    // List of spawn points
    public Transform[] spawnPoints;

    // Minimum and maximum time between spawns (randomized)
    public float minSpawnCooldown = 2f;
    public float maxSpawnCooldown = 5f;

    public int maxEnemies = 10;

    public bool isSpawning = true;
    private float spawnCoolDown;

    private void Update()
    {
        if (isSpawning)
            SpawnEnemies();
    }

    // Coroutine to spawn enemies at random times
    private void SpawnEnemies() //make this into coroutine?
    {
        if (GameManager.instance.player == null) //dont spawn any enemies if the player has not been initialized yet
            return;

        spawnCoolDown -= Time.deltaTime;
        if (spawnCoolDown > 0)
            return;

        if (GameManager.instance.enemies >= maxEnemies)
            return;
        
        int randomSpawns = Random.Range(3, 7); //choose how many enemies to spawn
        List<Transform> pointsVisited = new List<Transform>();
        for (int i = 0; i < randomSpawns; i++)
        {
            int spawnPoint = Random.Range(0, GameManager.instance.spawnPointList.Count);
            for (int p = 0; p < pointsVisited.Count; p++)
            {
                if (pointsVisited[p] == GameManager.instance.spawnPointList[spawnPoint])
                {
                    pointsVisited.Add(GameManager.instance.spawnPointList[spawnPoint]);
                    continue;
                }
            }
            if (GameManager.instance.enemies < maxEnemies)
                SpawnEnemy(spawnPoint);
        }

        spawnCoolDown = Random.Range(minSpawnCooldown, maxSpawnCooldown);
    }

    // Spawn an enemy at a specific spawn point
    private void SpawnEnemy(int spawnPointIndex)
    {
        // Get a random index from the enemyPrefabList
        int randomIndex = Random.Range(0, enemyPrefabList.Count);

        // Get the random prefab path
        GameObject randomPrefabPath = enemyPrefabList[randomIndex];

        //Spawn drop pod
        Vector3 dropPodSpawn = GameManager.instance.spawnPointList[spawnPointIndex].position;
        dropPodSpawn.y = 50;
        GameObject newPod = Instantiate(dropPod, dropPodSpawn, Quaternion.identity);
        newPod.GetComponent<DropPod>().SpawnDropPod(randomPrefabPath, GameManager.instance.spawnPointList[spawnPointIndex].position, false);
        GameManager.instance.enemies++;
    }
}