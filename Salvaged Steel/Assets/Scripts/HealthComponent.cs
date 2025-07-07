using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour
{

    public float health = 30f;
    [HideInInspector] public float maxHealth;

    public UnityEvent OnDeath;
    public UnityEvent OnDamage;


    private void Start()
    {
        maxHealth = health;
    }

    public void TakeDamage(float damage)
    {
        if (health <= 0) 
            return;
        health -= damage;
        OnDamage.Invoke();

        if (health <= 0)
            OnDeath.Invoke();
    }
}
