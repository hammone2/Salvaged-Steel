using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
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

    public LayerMask damageableLayer;

    public void Initialize(float damage, int attackerId, bool isMine, float lifeTime)
    {
        this.damage = damage;
        this.attackerId = attackerId;
        this.isMine = isMine;
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
        if (other.CompareTag("Player") && !isMine)
        {
            PlayerController player = GameManager.instance.GetPlayer(other.gameObject);

            if (player.id != attackerId)
                player.photonView.RPC("TakeDamage", player.photonPlayer, attackerId, damage);
        }
        else if (other.CompareTag("Enemy") && isMine)
        {
            // might do a GetEnemy() func in GameManager
            Enemy enemy = other.GetComponent<Enemy>();
            enemy.photonView.RPC("TakeDamage", RpcTarget.All, attackerId, damage);
        }

        /*// If Health component exists, call TakeDamage
        if (healthComponent != null)
        {
            // replace this with enemy's own unique dmg func
            //healthComponent.TakeDamage(attackerId, damage);
        }
        else if (playerController != null)
        {
            playerController.TakeDamage(attackerId, damage);
        }*/

        // Check if the collider is in one of the specified layers
        if (((1 << other.gameObject.layer) & layersToHit) != 0)
        {
            if (explosive)
                ApplySplashDamage(transform.position);
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

            if (hitCollider.CompareTag("Player") && !isMine)
            {
                PlayerController player = GameManager.instance.GetPlayer(hitCollider.gameObject);

                if (player.id != attackerId)
                    player.photonView.RPC("TakeDamage", player.photonPlayer, attackerId, damage);
            }
            else if (hitCollider.CompareTag("Enemy") && isMine)
            {
                // might do a GetEnemy() func in GameManager
                Enemy enemy = hitCollider.GetComponent<Enemy>();
                enemy.photonView.RPC("TakeDamage", RpcTarget.All, attackerId, damage);
            }


            // Check if the object hit has a health component (i.e., it is damageable)
            /*HealthComponent targetHealth = hitCollider.GetComponent<HealthComponent>();
            if (targetHealth != null)
            {
                // Apply damage to the object (you can scale this based on distance or other factors)
                targetHealth.TakeDamage(damage);
            }*/

            // Optionally, apply a force to rigidbodies to simulate an explosion effect (e.g., knockback)
            /*Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = hitCollider.transform.position - explosionPoint;
                rb.AddForce(direction.normalized * force, ForceMode.Impulse);
            }*/
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
