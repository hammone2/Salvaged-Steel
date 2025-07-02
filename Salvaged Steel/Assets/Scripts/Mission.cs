using UnityEngine;

public class Mission : MonoBehaviour
{
    public void MissionComplete()
    {
        GameManager.instance.missionComplete = true;
    }


    //make all this stuff into a child script later
    public int targetKills = 1;

    void OnEnable()
    {
        GameManager.OnEnemyKilled += HandleEnemyKilled;
    }

    void OnDisable()
    {
        GameManager.OnEnemyKilled -= HandleEnemyKilled;
    }

    void HandleEnemyKilled(int totalKills)
    {
        if (totalKills >= targetKills)
        {
            Debug.Log("Mission complete! " + totalKills + " enemies killed.");
            MissionComplete();
        }
    }
}
