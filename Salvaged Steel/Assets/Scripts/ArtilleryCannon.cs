using System.Collections;
using UnityEngine;

public class ArtilleryCannon : MonoBehaviour
{
    public Transform target;              // The target to fire at
    public GameObject projectilePrefab;   // Prefab of the projectile
    public float desiredHeight = 50f;     // Desired peak height of the arc (in meters)
    public float gravity = 9.81f;         // Gravitational acceleration (in m/s^2)

    private void Start()
    {
        FireCannon();
    }

    void FireCannon()
    {
        // Get the target's position and calculate the horizontal distance
        Vector3 targetPosition = target.position;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        // 1. Calculate the launch parameters (angle and velocity)
        (float angle, float velocity) = CalculateLaunchParameters(distanceToTarget, desiredHeight);

        // 2. Instantiate the projectile
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        // 3. Move the projectile along the parabolic path
        StartCoroutine(MoveProjectileAlongParabola(projectile, angle, velocity, distanceToTarget));
    }

    // Calculate the launch angle and velocity required to hit the target with the desired height
    (float, float) CalculateLaunchParameters(float distance, float height)
    {
        // Calculate the launch angle using projectile motion equations
        // Derivation of the angle from the standard projectile motion equations
        float angle = Mathf.Atan((Mathf.Pow(gravity * distance, 2) + Mathf.Sqrt(Mathf.Pow(gravity * distance, 2) - 4 * gravity * distance * (-2 * height))) / (2 * distance * gravity));

        // Calculate the initial velocity using the angle and horizontal distance
        float velocity = Mathf.Sqrt(gravity * distance / Mathf.Sin(2 * angle));

        return (angle, velocity);
    }

    // Coroutine to move the projectile along the parabolic path
    IEnumerator MoveProjectileAlongParabola(GameObject projectile, float angle, float velocity, float distance)
    {
        // Calculate the total flight time
        float flightTime = distance / (velocity * Mathf.Cos(angle));  // Time to reach the target horizontally
        float elapsedTime = 0f;

        // Move the projectile along the parabolic path
        while (elapsedTime < flightTime)
        {
            elapsedTime += Time.deltaTime;

            // Calculate horizontal position (constant horizontal velocity)
            float x = velocity * Mathf.Cos(angle) * elapsedTime;

            // Calculate vertical position (affected by gravity)
            float y = transform.position.y + velocity * Mathf.Sin(angle) * elapsedTime - 0.5f * gravity * Mathf.Pow(elapsedTime, 2);

            // Update the projectile's position
            projectile.transform.position = new Vector3(transform.position.x + x, y, transform.position.z);

            // If the projectile has reached or passed the target distance, stop
            if (x >= distance)
            {
                projectile.transform.position = target.position;  // Snap to the target's position if overshoot
                break;
            }

            yield return null;  // Wait until the next frame
        }

        // Destroy the projectile after it reaches the target
        Destroy(projectile);
    }
}
