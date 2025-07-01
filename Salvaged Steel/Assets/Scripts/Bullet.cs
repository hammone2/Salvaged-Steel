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
        // Try to get the Health component from the other game object
        HealthComponent healthComponent = other.GetComponent<HealthComponent>();
        PlayerController playerController = other.GetComponent<PlayerController>();

        // did we hit a player or enemy?
        // if this is the local player's bullet, damage the hit enemy
        // if its not the local player, then damage the local player
        // we're using client side hit detection
        if (other.CompareTag("Player"))
        {
            PlayerController player = GameManager.instance.GetPlayer();
            player.TakeDamage(attackerId, damage);
        }
        else if (other.CompareTag("Enemy"))
        {
            // might do a GetEnemy() func in GameManager
            Enemy enemy = other.GetComponent<Enemy>();
            enemy.TakeDamage(attackerId, damage);
        }

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

            if (hitCollider.CompareTag("Player"))
            {
                PlayerController player = GameManager.instance.GetPlayer();

                if (player.id != attackerId)
                    player.TakeDamage(attackerId, damage);
            }
            else if (hitCollider.CompareTag("Enemy"))
            {
                // might do a GetEnemy() func in GameManager
                Enemy enemy = hitCollider.GetComponent<Enemy>();
                enemy.TakeDamage(attackerId, damage);
            }
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
