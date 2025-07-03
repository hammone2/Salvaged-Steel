using UnityEngine;

public abstract class Mission : ScriptableObject
{
    public void MissionComplete()
    {
        GameManager.instance.missionComplete = true;
        Unregister();
        ResetMission();
    }
    public virtual void ResetMission()
    {
        //using a virtual so Im not forced to have this in missions that dont need it
    }
    public abstract void Register();
    public abstract void Unregister();
}
