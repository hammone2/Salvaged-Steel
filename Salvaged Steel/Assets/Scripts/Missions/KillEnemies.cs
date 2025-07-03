using UnityEngine;

[CreateAssetMenu(menuName = "Missions/Kill Enemies")]
public class KillEnemies : Mission
{
    public int targetKills = 1;

    public override void Register()
    {
        GameManager.OnEnemyKilled += HandleEnemyKilled;
    }

    public override void Unregister()
    {
        GameManager.OnEnemyKilled -= HandleEnemyKilled;
    }

    void HandleEnemyKilled(int totalKills)
    {
        if (totalKills >= targetKills)
        {
            MissionComplete();
        }
    }
}