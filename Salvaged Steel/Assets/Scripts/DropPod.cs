using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPod : MonoBehaviour
{
    public GameObject particleEffect;

    private GameObject enemyPrefab;
    private Vector3 spawnLocation;
    private bool hasArrived = false;
    private bool playerPod;
    private float dropSpeed = 28f;

    void Update()
    {
        if (spawnLocation != null)
            transform.position = Vector3.MoveTowards(transform.position, spawnLocation, dropSpeed * Time.deltaTime);

        if (transform.position.y <= 0 && hasArrived == false)
        {
            hasArrived = true;
            if (!playerPod)
                SpawnTank();
            else
                SpawnPlayer();
        }
    }
    private void SpawnTank()
    {
        Instantiate(enemyPrefab, spawnLocation, Quaternion.identity);
        DestroyDropPod();
    }

    private void SpawnPlayer()
    {
        if (GameManager.instance.player != null)
            GameManager.instance.player.gameObject.SetActive(true);

        GameManager.instance.player.Respawn(spawnLocation);
        DestroyDropPod();
    }

    public void SpawnDropPod(GameObject enemy, Vector3 location, bool isPlayer)
    {
        enemyPrefab = enemy;
        spawnLocation = location;
        playerPod = isPlayer;
    }

    private void DestroyDropPod()
    {
        Vector3 particleLocation = spawnLocation;
        particleLocation.y = 0.01f;
        Instantiate(particleEffect, particleLocation, Quaternion.identity);

        Destroy(gameObject);
    }
}
