using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float lifeTime = 5f;
    public LayerMask layersToHit;
    [HideInInspector] public float damage;
    private int attackerId;
    private bool isMine;

    void Awake()
    {
        Destroy(gameObject, lifeTime);
    }

    /*public void Initialize(int damage, int attackerId, bool isMine)
    {
        this.damage = damage;
        this.attackerId = attackerId;
        this.isMine = isMine;
        Destroy(gameObject, lifeTime);
    }*/

    private void OnTriggerEnter(Collider other)
    {
        // Try to get the Health component from the other game object
        HealthComponent healthComponent = other.GetComponent<HealthComponent>();
        PlayerController playerController = other.GetComponent<PlayerController>();

        // If Health component exists, call TakeDamage
        if (healthComponent != null)
        {
            healthComponent.TakeDamage(damage);
        }
        else if (playerController != null)
        {
            playerController.TakeDamage(damage);
        }

        // Check if the collider is in one of the specified layers
        if (((1 << other.gameObject.layer) & layersToHit) != 0)
        {
            Destroy(this.gameObject);
        }
    }
}
