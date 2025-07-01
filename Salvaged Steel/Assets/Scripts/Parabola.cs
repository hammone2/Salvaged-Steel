using System.Collections.Generic;
using UnityEngine;

public class ParabolaGenerator : MonoBehaviour
{
    [Header("Parabola Parameters")]
    public float height = 10f;      // b - peak height
    public float interval = 0.1f;   // step size along x-axis
    public Vector3 endPoint = new Vector3(10f, 0f, 0f);
    public float projectileSpeed = 5f; // Speed of the projectile
    [SerializeField] LayerMask layerMask;

    [Header("Gun Components")]
    [SerializeField] private GameObject gunBarrel;
    [SerializeField] private GameObject gunBase;
    [SerializeField] private GameObject projectilePrefab; // The projectile prefab

    [HideInInspector]
    public List<Vector3> parabolaPoints = new List<Vector3>();
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        GenerateParabola();
    }
    private void Update()
    {
        var (success, position) = GetMousePosition();
        if (success)
        {
            endPoint = position;
            GenerateParabola();

            if (Input.GetMouseButtonDown(0))
            {
                SpawnProjectile();
            }
        }
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

        gunBase.transform.rotation = Quaternion.LookRotation(endPoint);

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

    void SpawnProjectile()
    {
        if (parabolaPoints.Count < 2) return;

        // Spawn projectile at the starting point
        GameObject projectile = Instantiate(projectilePrefab, parabolaPoints[0], Quaternion.identity);
        List<Vector3> parabolaPointsCopy = new List<Vector3>(parabolaPoints);

        // Start moving the projectile
        ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
        projectileController.Initialize(parabolaPointsCopy, projectileSpeed, 1f, 0);
    }

    private (bool success, Vector3 position) GetMousePosition()
    {
        var ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, layerMask))
        {
            // The Raycast hit something, return with the position.
            return (success: true, position: hitInfo.point);
        }
        else
        {
            // The Raycast did not hit anything.
            return (success: false, position: Vector3.zero);
        }
    }

    void OnDrawGizmos()
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
