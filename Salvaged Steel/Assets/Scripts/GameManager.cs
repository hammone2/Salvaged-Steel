using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float postGameTime;

    [Header("Players")]
    public GameObject playerPrefab;
    public PlayerController player;
    public Transform[] spawnPoints;
    public int alivePlayers;

    [Header("Game Settings")]
    public float respawnTime = 3f;

    // instance
    public static GameManager instance;
    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        int index = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[index];
        Instantiate(playerPrefab, spawnPoint);
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