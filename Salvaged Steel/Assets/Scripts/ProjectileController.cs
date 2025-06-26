using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ProjectileController : MonoBehaviour
{
    public float rotationSpeed = 360f;

    private List<Vector3> parabolaPoints;  // The parabola path the projectile will follow
    private float speed;                   // Speed of the projectile
    private int currentPointIndex = 0;     // Current point the projectile is traveling to
    private float journeyLength;           // Total length of the path
    private float traveledDistance = 0f;   // Distance traveled so far

    private bool isMoving = false;         // Whether the projectile is still moving

    public void Initialize(List<Vector3> parabolaPoints, float speed)
    {
        this.parabolaPoints = parabolaPoints;
        this.speed = speed;

        // Set projectile's position to the first point
        transform.position = parabolaPoints[0];
        traveledDistance = 0f;
        currentPointIndex = 0;

        // Calculate total path length
        journeyLength = 0f;
        for (int i = 0; i < parabolaPoints.Count - 1; i++)
        {
            journeyLength += Vector3.Distance(parabolaPoints[i], parabolaPoints[i + 1]);
        }

        isMoving = true;

        Debug.Log("Shell spawned");
    }

    private void Update()
    {
        if (isMoving && parabolaPoints.Count > 1)
        {
            // Get the current point and next point
            Vector3 currentPoint = parabolaPoints[currentPointIndex];
            Vector3 nextPoint = parabolaPoints[Mathf.Min(currentPointIndex + 1, parabolaPoints.Count - 1)];

            // Calculate the distance to move this frame
            float distanceThisFrame = speed * Time.deltaTime;

            // Move the projectile towards the next point
            traveledDistance += distanceThisFrame;

            // Move the projectile between the two points using Lerp for smooth movement
            float t = Mathf.Clamp01(traveledDistance / Vector3.Distance(currentPoint, nextPoint));
            transform.position = Vector3.Lerp(currentPoint, nextPoint, t);

            // Ensure the projectile is always facing the next point
            if (t < 1f)
            {
                Vector3 direction = (nextPoint - transform.position).normalized;
                //transform.rotation = Quaternion.LookRotation(direction);
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
            else
            {
                // Reached the current point, move to the next point
                if (currentPointIndex < parabolaPoints.Count - 1)
                {
                    currentPointIndex++;
                    traveledDistance = 0f;  // Reset the traveled distance for the next segment
                }
                else
                {
                    isMoving = false;  // End the movement when all points are covered
                }
            }
        }
    }
}
