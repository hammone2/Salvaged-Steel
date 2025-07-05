using UnityEngine;

[CreateAssetMenu(menuName = "Missions/Kill Enemies")]
public class KillEnemies : Mission
{
    public int targetKills = 1;

    public override void Register()
    {
        GameManager.OnEnemyKilled += HandleEnemyKilled;
        HUD.instance.missionName.SetText("Kill Enemies");
        HUD.instance.missionProgressText.SetText("0/"+targetKills);
    }

    public override void Unregister()
    {
        GameManager.OnEnemyKilled -= HandleEnemyKilled;
    }

    void HandleEnemyKilled(int totalKills)
    {
        HUD.instance.missionProgressText.SetText(totalKills + "/" + targetKills);
        SetProgressBar(totalKills, targetKills);
        if (totalKills >= targetKills)
        {
            MissionComplete();
        }
    }
}