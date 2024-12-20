using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float lifeTime = 5f;
    public LayerMask layersToHit;
    [HideInInspector] public float damage;
    private int attackerId;
    private bool isMine;

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
            Destroy(this.gameObject);
        }
    }
}
