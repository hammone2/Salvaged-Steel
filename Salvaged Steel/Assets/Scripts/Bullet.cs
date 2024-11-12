using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float lifeTime = 5f;
    private float damage;

    void Awake()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        //Destroy(collision.gameObject);
        Destroy(gameObject);
    }
}
