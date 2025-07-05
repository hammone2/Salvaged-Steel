using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Missions/Timed")]
public class Timed : Mission
{
    public int missionTime;

    public override void Register()
    {
        GameManager.instance.StartMissionCoroutine(MissionTimer());
        HUD.instance.missionName.SetText("Survive");
    }

    public override void Unregister()
    {

    }

    private IEnumerator MissionTimer()
    {
        int timeRemaining = missionTime;

        while (timeRemaining > 0)
        {
            int minutes = timeRemaining / 60;
            int seconds = timeRemaining % 60;
            HUD.instance.missionProgressText.SetText($"Time Left: {minutes}:{seconds:D2}");

            yield return new WaitForSeconds(1f);
            timeRemaining--;
            SetProgressBar(timeRemaining,missionTime);
        }
        MissionComplete();
    }
}
