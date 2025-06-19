using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private List<Vector3> parabolaPoints;  // The parabola path the projectile will follow.
    private float speed;                   // Speed at which the projectile moves along the parabola
    private float travelTime;              // Time for the projectile to travel along the entire parabola path

    private int currentPointIndex = 0;     // Index to track the current point in the parabola
    private float timeAlongPath = 0f;      // Time elapsed since the projectile started moving
    private bool isMoving = false;         // Whether the projectile is moving along the parabola

    public void Initialize(List<Vector3> parabolaPoints, float speed)
    {
        this.parabolaPoints = parabolaPoints;
        this.speed = speed;
        this.travelTime = Vector3.Distance(parabolaPoints[0], parabolaPoints[parabolaPoints.Count - 1]) / speed;
        isMoving = true;
        timeAlongPath = 0f;  // Reset the time when the projectile is initialized
        currentPointIndex = 0;  // Start at the first point
        transform.position = parabolaPoints[0];  // Place the projectile at the start point
    }

    private void Update()
    {
        if (isMoving && parabolaPoints.Count > 1)
        {
            // Update the time along the path based on speed
            timeAlongPath += Time.deltaTime;

            // Calculate normalized time (t) for the projectile's position along the parabola
            float t = timeAlongPath / travelTime;
            t = Mathf.Clamp01(t);  // Ensure t doesn't exceed 1

            // Move the projectile along the parabola
            Vector3 currentPoint = GetParabolaPointAtT(t);
            transform.position = currentPoint;

            // If we're not at the final point, make sure the projectile faces the direction of movement
            if (t < 1f)
            {
                float nextT = Mathf.Min(t + 0.01f, 1f);  // Look ahead to next point
                Vector3 nextPoint = GetParabolaPointAtT(nextT);
                Vector3 direction = (nextPoint - currentPoint).normalized;
                transform.rotation = Quaternion.LookRotation(direction);
            }
            else
            {
                // Ensure the projectile ends at the final point
                transform.position = parabolaPoints[parabolaPoints.Count - 1];
                isMoving = false;  // Stop movement once the projectile reaches the end
            }
        }
    }

    private Vector3 GetParabolaPointAtT(float t)
    {
        // Calculate the index of the point on the parabola path for the given t
        int pointIndex = Mathf.FloorToInt(t * (parabolaPoints.Count - 1));
        return parabolaPoints[pointIndex];
    }
}
