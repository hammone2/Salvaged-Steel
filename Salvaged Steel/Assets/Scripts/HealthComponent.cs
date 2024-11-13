using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{

    public float health = 30f;

    public void TakeDamage(float damage)
    {
        if (health <= 0)
            return;
        health -= damage;
    }
}
