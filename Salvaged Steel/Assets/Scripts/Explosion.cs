using UnityEngine;

public class Explosion : MonoBehaviour
{

    public float damage;
    private int attackerId;
    private bool isMine;
    public float explosionRadius = 5f; // The radius in which damage is applied
    public LayerMask layersToHit;
    public float shakeAmount;

    void Start()
    {
        ApplySplashDamage();
    }

    public void ApplySplashDamage()
    {
        // Create a sphere-shaped overlap to detect all colliders within the radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, layersToHit);

        // Loop through all the colliders hit by the explosion
        foreach (var hitCollider in hitColliders)
        {
            DealDamage.ApplyDamage(hitCollider, damage);
        }
    }
}
