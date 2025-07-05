using NUnit.Framework;
using UnityEngine;
using System;
using System.Collections;
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
    public Mission[] missionList;
    [HideInInspector] public Mission mission;
    public static event Action<int> OnEnemyKilled;
    private int enemiesKilled;

    private int _enemies;
    public int enemies
    {
        get { return _enemies; } 
        set
        {
            if (_enemies > value) //has an enemy been killed?
            {
                enemiesKilled += 1;
                OnEnemyKilled?.Invoke(enemiesKilled);
            }
            _enemies = value;
        }
    }

    [Header("Other")]
    [SerializeField] private GameObject dropPod;

    //Gamemode stuff
    private bool _missionComplete = false;
    public bool missionComplete
    {
        get { return _missionComplete;}
        set 
        { 
            _missionComplete = value;
            if (_missionComplete)
            {
                Spawner.instance.isSpawning = false;
                Spawner.instance.SpawnBoss(new Vector3(128f, 0, 0)); //temporary vector solution
            }
        }
    }

    // instance
    public static GameManager instance;
    void Awake()
    {
        instance = this;

        //select a mission type for the level
        int missionIndex = UnityEngine.Random.Range(0, missionList.Length);
        mission = missionList[missionIndex];
        mission.Register();
    }

    public void InitializePlayer(Vector3 spawnPos)
    {
        Instantiate(playerPrefab, spawnPos, Quaternion.identity); //using vector 3 instead of transform so player isnt parented to the level
    }

    public void SpawnPlayer()
    {
        int index = UnityEngine.Random.Range(0, spawnPointList.Count);
        Vector3 dropPodSpawn = spawnPointList[index].position;
        dropPodSpawn.y = 100;
        GameObject newPod = Instantiate(dropPod, dropPodSpawn, Quaternion.identity);
        newPod.GetComponent<DropPod>().SpawnDropPod(playerPrefab, spawnPointList[index].position, true);
        PlayerCamera.instance.NewPosition(newPod.transform);

        if (player == null)
            PlayerCamera.instance.transform.position = newPod.transform.position;
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

    public void StartMissionCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    private void OnDestroy()
    {
        mission.Unregister();
    }
}