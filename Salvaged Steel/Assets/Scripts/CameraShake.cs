using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    public float shakeMagnitude = 0.05f;
    private float shakeFalloff = 0.01f;
    private Vector3 initialPos;

    void Awake()
    {
        initialPos = transform.position;
    }

    void Update()
    {
        if (shakeMagnitude > 0)
            shakeMagnitude -= shakeFalloff;
        if (shakeMagnitude < 0)
            shakeMagnitude = 0;
        transform.position += Random.insideUnitSphere * shakeMagnitude;
    }
}
