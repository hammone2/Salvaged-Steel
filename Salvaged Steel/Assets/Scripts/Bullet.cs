using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float lifeTime = 5f;
    public LayerMask layersToHit;
    [HideInInspector] public float damage;
    public bool explosive = false;
    private int attackerId;
    private bool isMine;
    public float explosionRadius = 5f; // The radius in which damage is applied
    public float force = 10f; // The force of the explosion, can knock back objects
    public GameObject hitSpark;

    public LayerMask damageableLayer;

    public virtual void Initialize(float damage, int attackerId, float lifeTime)
    {
        this.damage = damage;
        this.attackerId = attackerId;
        this.lifeTime = lifeTime;
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        DealDamage.ApplyDamage(other,damage);
        

        // Check if the collider is in one of the specified layers
        if (((1 << other.gameObject.layer) & layersToHit) != 0)
        {
            if (explosive)
                ApplySplashDamage(transform.position);
            Instantiate(hitSpark, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

    // Method to be called when splash damage occurs
    public void ApplySplashDamage(Vector3 explosionPoint)
    {
        // Create a sphere-shaped overlap to detect all colliders within the radius
        Collider[] hitColliders = Physics.OverlapSphere(explosionPoint, explosionRadius, layersToHit);

        // Loop through all the colliders hit by the explosion
        foreach (var hitCollider in hitColliders)
        {
            DealDamage.ApplyDamage(hitCollider, damage);
        }
    }

    // Draw the explosion radius in the editor for visualization
    private void OnDrawGizmos()
    {
        if (explosive)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}
