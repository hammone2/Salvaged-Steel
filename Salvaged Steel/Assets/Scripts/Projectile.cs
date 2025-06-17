using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform firePoint;
    
    public IEnumerator Movement(Transform firePoint, Vector3 direction, float v0, float _angle, float time)
    {
        float t = 0;
        while (t < time)
        {
            float x = v0 * t * Mathf.Cos(_angle);
            float y = v0 * t * Mathf.Sin(_angle) - (1f/2f) * -Physics.gravity.y * Mathf.Pow(t, 2);
            transform.position = firePoint.position + direction * x + Vector3.up * y;
            t += Time.deltaTime;

            yield return null;
        }
    }
}
