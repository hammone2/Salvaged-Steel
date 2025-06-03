using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    public float shakeMagnitude = 0f;
    private float shakeFalloff = 0.01f;
    [HideInInspector] public Vector3 initialPos;

    void Awake()
    {
        initialPos = transform.localPosition;
    }

    void Update()
    {
        if (shakeMagnitude > 0)
            shakeMagnitude -= shakeFalloff;
        if (shakeMagnitude < 0)
            shakeMagnitude = 0;

        transform.position += Random.insideUnitSphere * shakeMagnitude;
    }

    public void ScreenShake(float mag)
    {
        shakeMagnitude = mag;
        HUD.instance.ScreenShake(shakeMagnitude);
    }
}
