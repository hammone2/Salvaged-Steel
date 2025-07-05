using UnityEngine;

public abstract class Mission : ScriptableObject
{
    public void MissionComplete()
    {
        GameManager.instance.missionComplete = true;
        HUD.instance.missionProgressText.SetText("Mission Complete!");
        Unregister();
        ResetMission();
    }

    public void SetProgressBar(float current, float whole)
    {
        HUD.instance.missionProgressBar.fillAmount = current/whole;
    }
    public virtual void ResetMission()
    {
        //using a virtual so Im not forced to have this in missions that dont need it
    }
    public abstract void Register();
    public abstract void Unregister();
}
