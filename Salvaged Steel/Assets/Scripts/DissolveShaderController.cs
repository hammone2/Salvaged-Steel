using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class DissolveShaderController : MonoBehaviour
{

    public ParticleSystem _particleSystem;
    private float lastTime;

    private Renderer _renderer;

    void Start()
    {
        _renderer = _particleSystem.GetComponent<Renderer>();

        lastTime = _particleSystem.time;  // Store the initial time
    }

    void Update()
    {
        // If the particle system is playing
        if (_particleSystem.isPlaying)
        {
            // Check if it has just started a new loop (time has looped back to 0)
            if (_particleSystem.time < lastTime)
            {
                // The time has wrapped around, so trigger the event
                SetStartTime();
            }

            // Update lastTime to the current time for the next frame
            lastTime = _particleSystem.time;
        }
    }

    void SetStartTime()
    {
        _renderer.material.SetFloat("_StartTime", Time.time);
        Debug.Log("Starttime set!");
    }
}
