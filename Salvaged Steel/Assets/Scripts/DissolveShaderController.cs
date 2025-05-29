using UnityEngine;

public class DissolveShaderController : MonoBehaviour
{

    public ParticleSystem _particleSystem;

    void Start()
    {
        var renderer = _particleSystem.GetComponent<Renderer>();
        renderer.material.SetFloat("_StartTime", Time.time);
    }
}
