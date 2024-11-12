using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{

    public Transform bulletSpawner;
    public GameObject bulletPrefab;
    public float bulletSpeed = 32f;
    public float fireRate = 0.15f;
    public float shakeMagnitude = 0.15f;

    private float timerValue;
    private CameraShake cameraShake;

    private void Awake()
    {
        timerValue = fireRate;

        Transform cameraTransform = transform.parent.transform.parent.Find("Camera"); //get the player's camera
        if (cameraTransform != null)
        {
            cameraShake = cameraTransform.GetComponent<CameraShake>();
        }
    }

    private void Update()
    {
        if (timerValue < fireRate)
        {
            timerValue += Time.deltaTime;
            if (timerValue > fireRate) 
            {
                timerValue = fireRate;
            }
        }
    }

    public void Shoot()
    {
        if (timerValue < fireRate)
            return;
        var bullet = Instantiate(bulletPrefab, bulletSpawner.position, bulletSpawner.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bulletSpawner.forward * bulletSpeed;
        timerValue = 0;
        if (cameraShake != null)
            cameraShake.shakeMagnitude = shakeMagnitude;
    }
}
