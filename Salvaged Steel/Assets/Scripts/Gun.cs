using UnityEngine;
using UnityEngine.Rendering;

public class Gun : MonoBehaviour
{
    [Header("Components")]
    public Texture2D crosshair;
    public Transform bulletSpawner;
    public GameObject bulletPrefab;
    public ParticleSystem muzzleFlash;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioClip shootSound;

    [Header("Weapon Stats")]
    public float bulletSpeed = 32f;
    public float fireRate = 0.15f;
    public float shakeMagnitude = 0.15f;
    public float damage = 5f;
    public float bulletLifeTime = 5f;
    public int ammo, maxAmmo = 200;

    [SerializeField, Range(0f, 100f)]
    public float accuracy = 100f;  // Accuracy of the shot (0 to 100)

    public float[] spreadAngles = new float[] { 0f }; // The angles for shotgun spread

    [HideInInspector] public float lastShootTime;
    [HideInInspector] public CameraShake cameraShake;

    public virtual void Shoot(int id, bool isPlayer)
    {
        if (Time.time - lastShootTime < fireRate)
            return;
        if (ammo <= 0)
            return;

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

            SpawnBullet(id, direction);
        }

        lastShootTime = Time.time;
        if (isPlayer)
            UpdateStats();
        HUD.instance.UpdateAmmoText();
        if (cameraShake != null)
            cameraShake.ScreenShake(shakeMagnitude);
        muzzleFlash.Play();
        SoundFXManager.instance.PlaySoundFXClip(shootSound, bulletSpawner, 1f);
        PlayShootAnimation();
    }

    private void SpawnBullet(int id, Vector3 direction)
    {
        var bullet = Instantiate(bulletPrefab, bulletSpawner.position, Quaternion.LookRotation(direction));
        bullet.GetComponent<Rigidbody>().linearVelocity = direction * bulletSpeed;
        bullet.GetComponent<Bullet>().Initialize(damage, id, bulletLifeTime);
    }

    public void UpdateStats()
    {
        ammo -= 1;
    }

    public void GetCamera(Camera camera)
    {
        Transform cameraTransform = camera.transform;
        if (cameraTransform != null)
        {
            cameraShake = cameraTransform.GetComponent<CameraShake>();
        }
    }
    public void DisconnectCamera()
    {
        if (cameraShake != null)
            cameraShake = null;
    }

    public void PlayShootAnimation()
    {
        if (!animator)
            return;
        animator.SetTrigger("TriShoot");
    }
}
