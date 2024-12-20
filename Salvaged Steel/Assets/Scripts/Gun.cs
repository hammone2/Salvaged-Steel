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
    public float bulletLifeTime = 5f;
    public int ammo = 200;

    [SerializeField, Range(0f, 100f)]
    public float accuracy = 100f;  // Accuracy of the shot (0 to 100)

    public float[] spreadAngles = new float[] { 0f }; // The angles for shotgun spread

    private float lastShootTime;
    private CameraShake cameraShake;

    [PunRPC]
    public void Shoot(int id, bool isMine, bool isPlayer) // the ammo for some reason is not being synced across clients
    {
        if (Time.time - lastShootTime < fireRate)
            return;
        if (ammo <= 0)
            return;

        /*var bullet = Instantiate(bulletPrefab, bulletSpawner.position, bulletSpawner.rotation);
        if (bullet.GetComponent<Bullet>() != null) // doing this so we can have bullet prefabs that aren't bullets in the traditional sense
        {
            bullet.GetComponent<Rigidbody>().velocity = bulletSpawner.forward * bulletSpeed;
            bullet.GetComponent<Bullet>().Initialize(damage, id, isMine, bulletLifeTime);
        }*/

        // Calculate the deviation based on accuracy
        float deviation = 100f - accuracy; // Convert percentage into a factor

        foreach (float angle in spreadAngles)
        {
            float randomYDeviation = 0f;
            if (accuracy < 100f)
                randomYDeviation = Random.Range(-deviation, deviation);
            // Calculate the direction of the bullet based on the angle
            Quaternion rotation = Quaternion.Euler(0f, angle + randomYDeviation, 0f); // Rotate around the Y-axis
            Vector3 direction = rotation * bulletSpawner.forward; // Forward direction with applied angle

            var bullet = Instantiate(bulletPrefab, bulletSpawner.position, Quaternion.LookRotation(direction));
            bullet.GetComponent<Rigidbody>().velocity = direction * bulletSpeed;
            bullet.GetComponent<Bullet>().Initialize(damage, id, isMine, bulletLifeTime);
        }

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
