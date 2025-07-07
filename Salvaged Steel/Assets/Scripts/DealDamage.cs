using UnityEngine;

public struct DealDamage
{
    public static void ApplyDamage(Collider other, float damage)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = GameManager.instance.GetPlayer();
            player.TakeDamage(damage);
        }
        else
        {
            HealthComponent healthComponent = other.GetComponent<HealthComponent>();
            if (healthComponent != null)
            {
                healthComponent.TakeDamage(damage);
            }
        }
    }
}
