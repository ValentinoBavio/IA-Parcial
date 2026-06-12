using UnityEngine;

public interface IDamageable 
{
    void Damageable(float damageAmount);

    void Die();

    float maxHealth { get; set; }

    float currentHealth { get; set; }

}
