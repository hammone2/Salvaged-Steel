using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Gun : MonoBehaviourPun
{
    public Texture2D crosshair;
    public Transform bulletSpawner;
    public GameObject bulletPrefab;
    public float bulletSpeed = 32f;
    public float fireRate = 0.15f;
    public float shakeMagnitude = 0.15f;
    public float damage = 5f;
    public int ammo = 200;

    private float timerValue;
    private CameraShake cameraShake;

    private void Awake()
    {
        timerValue = fireRate;

        Transform cameraTransform = transform.parent.transform.parent.transform.parent.Find("Camera"); //get the player's camera
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

    [PunRPC]
    public void Shoot(int id, bool isMine)
    {
        if (timerValue < fireRate)
            return;
        if (ammo <= 0)
            return;
        var bullet = Instantiate(bulletPrefab, bulletSpawner.position, bulletSpawner.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bulletSpawner.forward * bulletSpeed;
        bullet.GetComponent<Bullet>().Initialize(damage, id, isMine);
        timerValue = 0;
        ammo -= 1;
        HUD.instance.UpdateAmmoText();
        if (cameraShake != null)
            cameraShake.shakeMagnitude = shakeMagnitude;
    }

    public void GetCamera(Camera camera)
    {
        Transform cameraTransform = camera.transform;
        if (cameraTransform != null)
        {
            cameraShake = cameraTransform.GetComponent<CameraShake>();
        }
    }
}
