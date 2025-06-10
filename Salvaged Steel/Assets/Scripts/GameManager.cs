using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic; //for list

public class GameManager : MonoBehaviour
{
    public float postGameTime;

    [Header("Players")]
    public GameObject playerPrefab;
    public PlayerController player;
    public Transform[] spawnPoints;
    public List<Transform> spawnPointList;
    public int alivePlayers;

    [Header("Game Settings")]
    public float respawnTime = 3f;
    public bool isRandomlyGenerated = false;
    public int enemies;

    [Header("Other")]
    [SerializeField] private GameObject dropPod;

    // instance
    public static GameManager instance;
    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (isRandomlyGenerated)
            return;

        InitializePlayer();
    }

    public void InitializePlayer()
    {
        int index = Random.Range(0, spawnPointList.Count);
        Transform spawnPoint = spawnPointList[index];
        Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity); //using vector 3 instead of transform so player isnt parented to the level
    }

    public void SpawnPlayer()
    {
        int index = Random.Range(0, spawnPointList.Count);
        Vector3 dropPodSpawn = spawnPointList[index].position;
        dropPodSpawn.y = 50;
        GameObject newPod = Instantiate(dropPod, dropPodSpawn, Quaternion.identity);
        newPod.GetComponent<DropPod>().SpawnDropPod(playerPrefab, spawnPointList[index].position, true);
    }

    public void CheckLoseCondition()
    {
        if (alivePlayers <= 0)
            LoseGame();
    }

    void WinGame(int winningPlayer)
    {
        // set the UI win text
        Invoke("GoBackToMenu", postGameTime);
    }

    void LoseGame()
    {
        Invoke("GoBackToMenu", postGameTime);
    }

    void GoBackToMenu()
    {
        //NetworkManager.instance.ChangeScene("MainMenu");
    }

    public PlayerController GetPlayer()
    {
        return player;
    }
}