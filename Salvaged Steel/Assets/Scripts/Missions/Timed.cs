using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Missions/Timed")]
public class Timed : Mission
{
    public int missionTime;

    public override void Register()
    {
        GameManager.instance.StartMissionCoroutine(MissionTimer());
    }

    public override void Unregister()
    {

    }

    private IEnumerator MissionTimer()
    {
        Debug.Log("Mission started");
        yield return new WaitForSeconds(missionTime);
        MissionComplete();
    }
}
