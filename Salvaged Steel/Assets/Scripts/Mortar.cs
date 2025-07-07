using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class Mortar : Gun
{
    [Header("Parabola Parameters")]
    public float height = 10f;      // b - peak height
    public float interval = 0.1f;   // step size along x-axis
    public Vector3 endPoint = new Vector3(10f, 0f, 0f);

    [Header("Gun Components")]
    [SerializeField] private GameObject gunBarrel;

    [HideInInspector]
    public List<Vector3> parabolaPoints = new List<Vector3>();

    public override void Shoot(int id, bool isPlayer)
    {
        if (Time.time - lastShootTime < fireRate)
            return;
        if (ammo <= 0)
            return;

        
        GenerateParabola();
        SpawnProjectile(id);

        lastShootTime = Time.time;
        if (isPlayer)
            UpdateStats();
        HUD.instance.UpdateAmmoText();
        if (cameraShake != null)
            cameraShake.ScreenShake(shakeMagnitude);
        muzzleFlash.Play();
        PlayShootAnimation();
    }

    void GenerateParabola()
    {
        parabolaPoints.Clear();

        float t = Vector3.Distance(transform.position, endPoint); //distance;
        float b = height;
        float c = t / 2f;
        float a = b / (c * c);

        Vector3 direction = (endPoint - transform.position).normalized;
        float step = interval;
        int numSteps = Mathf.CeilToInt(t / step);

        for (int i = 0; i <= numSteps; i++)
        {
            float x = i * step;
            float y = -a * Mathf.Pow((x - c), 2) + b;
            Vector3 point = transform.position + direction * x + Vector3.up * y;
            parabolaPoints.Add(point);
        }

        // Barrel rotation
        float slope = 2 * a * c;

        // Get the horizontal direction toward the target
        Vector3 horizontalDirection = (endPoint - transform.position);
        horizontalDirection.y = 0f;
        horizontalDirection.Normalize();

        // Build a forward vector with the slope applied
        Vector3 forwardWithSlope = horizontalDirection * 1f + Vector3.up * slope;
        forwardWithSlope.Normalize();

        // Set barrel rotation to match this forward direction
        gunBarrel.transform.rotation = Quaternion.LookRotation(forwardWithSlope);
    }

    void SpawnProjectile(int attackerId)
    {
        if (parabolaPoints.Count < 2) return;

        // Spawn projectile at the starting point
        GameObject projectile = Instantiate(bulletPrefab, bulletSpawner.position /*parabolaPoints[0]*/, Quaternion.identity);
        List<Vector3> parabolaPointsCopy = new List<Vector3>(parabolaPoints);

        // Start moving the projectile
        ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
        projectileController.Initialize(parabolaPointsCopy, bulletSpeed, damage);
    }

    void OnDrawGizmos() //draw the curve
    {
        if (parabolaPoints.Count > 1)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < parabolaPoints.Count - 1; i++)
            {
                Gizmos.DrawLine(parabolaPoints[i], parabolaPoints[i + 1]);
            }
        }
    }
}
