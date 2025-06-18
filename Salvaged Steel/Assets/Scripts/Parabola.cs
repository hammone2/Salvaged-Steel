using System.Collections.Generic;
using UnityEngine;

public class ParabolaGenerator : MonoBehaviour
{
    [Header("Parabola Parameters")]
    public float height = 10f;      // b - peak height
    public float interval = 0.1f;   // step size along x-axis

    [Header("Start and End Points")]
    public Vector3 startPoint = Vector3.zero;
    public Vector3 endPoint = new Vector3(10f, 0f, 0f);

    [Header("Gun Components")]
    [SerializeField] private GameObject gunBarrel;
    [SerializeField] private GameObject gunBase;

    [HideInInspector]
    public List<Vector3> parabolaPoints = new List<Vector3>();

    void Start()
    {
        GenerateParabola();
    }

    void GenerateParabola()
    {
        parabolaPoints.Clear();

        float t = Vector3.Distance(startPoint, endPoint); //distance;
        float b = height;
        float c = t / 2f;
        float a = b / (c * c);

        Vector3 direction = (endPoint - startPoint).normalized;
        float step = interval;
        int numSteps = Mathf.CeilToInt(t / step);

        for (int i = 0; i <= numSteps; i++)
        {
            float x = i * step;
            float y = -a * Mathf.Pow((x - c), 2) + b;
            Vector3 point = startPoint + direction * x + Vector3.up * y;
            parabolaPoints.Add(point);
        }

        gunBase.transform.rotation = Quaternion.LookRotation(endPoint);

        float slope = 2 * a * c;
        Vector3 tangent = new Vector3(1f, slope, 0f).normalized; // direction along the curve
        gunBarrel.transform.rotation = Quaternion.LookRotation(tangent);
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
