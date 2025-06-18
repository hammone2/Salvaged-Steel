using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    [SerializeField] float initialVelocity;
    [SerializeField] float angle;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] float step;
    [SerializeField] Transform firePoint;
    [SerializeField] LayerMask layerMask;

    [SerializeField] Projectile projectile;
    [SerializeField] GameObject gun;
    [SerializeField] GameObject gunBase;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        var (success, position) = GetMousePosition();
        if (success)
        {
            Vector3 direction = position - firePoint.position;
            Vector3 groundDirection = new Vector3(direction.x, 0, direction.z);


            Vector3 targetPos = new Vector3(groundDirection.magnitude, direction.y, 0);
            float height = targetPos.y + targetPos.magnitude / 2f;
            height = Mathf.Max(0.01f, height);
            float v0;
            float time;
            float _angle;
            CalculatePathWithHeight(targetPos, height, out v0, out _angle, out time);


            // Rotate the cannon to aim upwards
            Vector3 velocityDirection = new Vector3(Mathf.Cos(_angle), Mathf.Sin(_angle), 0);
            Quaternion targetRotation = Quaternion.LookRotation(velocityDirection);
            gun.transform.localRotation = Quaternion.Slerp(gun.transform.rotation, targetRotation, Time.deltaTime * 5f);

            gunBase.transform.localRotation = Quaternion.LookRotation(groundDirection);



            DrawPath(groundDirection.normalized, v0, _angle, time, step);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                projectile.StopAllCoroutines();
                projectile.StartCoroutine(projectile.Movement(firePoint, groundDirection.normalized, v0, _angle, time));
            }
        }
    }

    private void DrawPath(Vector3 direction, float v0, float angle, float time, float step)
    {
        step = Mathf.Max(0.01f, step);
        lineRenderer.positionCount = (int)(time / step) + 2;
        int count = 0;
        for (float i = 0; i < time; i += step)
        {
            float x = v0 * i * Mathf.Cos(angle);
            float y = v0 * i * Mathf.Sin(angle) - 0.5f * -Physics.gravity.y * Mathf.Pow(i, 2);
            lineRenderer.SetPosition(count, firePoint.position + direction * x + Vector3.up * y);
            count++;
        }

        float xFinal = v0 * time * Mathf.Cos(angle);
        float yFinal = v0 * time * Mathf.Sin(angle) - 0.5f * -Physics.gravity.y * Mathf.Pow(time, 2);
        lineRenderer.SetPosition(count, firePoint.position + direction * xFinal + Vector3.up * yFinal);
    }

    private float QuadraticEquation(float a, float b, float c, float sign)
    {
        return (-b + sign * Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
    }

    private void CalculatePathWithHeight(Vector3 targetPos, float h, out float v0, out float _angle, out float time)
    {
        float xt = targetPos.x;
        float yt = targetPos.y;
        float g = -Physics.gravity.y;

        float b = Mathf.Sqrt(2 * g * h);
        float a = (-0.5f * g);
        float c = -yt;

        float tplus = QuadraticEquation(a, b, c, 1);
        float tmin = QuadraticEquation(a, b, c, -1);
        time = tplus > tmin ? tplus : tmin;

        _angle = Mathf.Atan(b * time / xt);
        v0 = b / Mathf.Sin(_angle);
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
}
