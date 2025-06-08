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

            if (hitCollider.CompareTag("Player"))
            {
                PlayerController player = GameManager.instance.GetPlayer();

                //if (player.id != attackerId)
                player.TakeDamage(attackerId, damage);
                player.playerCamera.GetComponent<CameraShake>().ScreenShake(shakeAmount);
            }
            else if (hitCollider.CompareTag("Enemy"))
            {
                // might do a GetEnemy() func in GameManager
                Enemy enemy = hitCollider.GetComponent<Enemy>();
                enemy.TakeDamage(attackerId, damage);
            }
        }
    }
}
