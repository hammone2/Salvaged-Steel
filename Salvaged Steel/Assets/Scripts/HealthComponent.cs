using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HealthComponent : MonoBehaviourPun
{

    public float health = 30f;

    [PunRPC]
    public void TakeDamage(float damage)
    {
        if (health <= 0)
            return;
        health -= damage;
    }
}
