using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPod : MonoBehaviour
{
    public GameObject particleEffect;

    private GameObject enemyPrefab;
    private Vector3 spawnLocation;
    private bool hasArrived = false;
    private float dropSpeed = 28f;

    void Update()
    {
        if (spawnLocation != null)
            transform.position = Vector3.MoveTowards(transform.position, spawnLocation, dropSpeed * Time.deltaTime);

        if (transform.position.y <= 0 && hasArrived == false)
        {
            hasArrived = true;
            SpawnTank();
        }
    }
    private void SpawnTank()
    {
        Instantiate(enemyPrefab, spawnLocation, Quaternion.identity);
        Vector3 particleLocation = spawnLocation;
        particleLocation.y = 0.01f;
        Instantiate(particleEffect, particleLocation, Quaternion.identity);

        Destroy(gameObject);
    }

    public void SpawnDropPod(GameObject enemy, Vector3 location)
    {
        enemyPrefab = enemy;
        spawnLocation = location;
    }
}
