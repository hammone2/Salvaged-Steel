using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    public float shakeMagnitude = 0.05f;
    private float shakeFalloff = 0.01f;
    private Vector3 initialPos;
    //private float xPosition;
    //private float zPosition;

    void Awake()
    {
        initialPos = transform.position;
        //xPosition = transform.position.x;
        //zPosition = transform.position.z;
    }

    void Update()
    {
        if (shakeMagnitude > 0)
            shakeMagnitude -= shakeFalloff;
        if (shakeMagnitude < 0)
            shakeMagnitude = 0;
        //xPosition += Random.Range(-1f, 1f) * shakeMagnitude;
        //zPosition += Random.Range(-1f, 1f) * shakeMagnitude;

        transform.position += Random.insideUnitSphere * shakeMagnitude;
    }
}
