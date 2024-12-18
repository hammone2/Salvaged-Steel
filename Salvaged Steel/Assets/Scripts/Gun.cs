using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Gun : MonoBehaviourPun
{
    [Header("Components")]
    public Texture2D crosshair;
    public Transform bulletSpawner;
    public GameObject bulletPrefab;

    [Header("Weapon Stats")]
    public float bulletSpeed = 32f;
    public float fireRate = 0.15f;
    public float shakeMagnitude = 0.15f;
    public float damage = 5f;
    public int ammo = 200;

    private float lastShootTime;
    private CameraShake cameraShake;

    [PunRPC]
    public void Shoot(int id, bool isMine, bool isPlayer) // the ammo for some reason is not being synced across clients
    {
        if (Time.time - lastShootTime < fireRate)
            return;
        if (ammo <= 0)
            return;
        var bullet = Instantiate(bulletPrefab, bulletSpawner.position, bulletSpawner.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bulletSpawner.forward * bulletSpeed;
        bullet.GetComponent<Bullet>().Initialize(damage, id, isMine);
        lastShootTime = Time.time;
        if (isPlayer)
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

    [PunRPC]
    public void DisconnectCamera()
    {
        if (cameraShake != null)
            cameraShake = null;
    }
}
