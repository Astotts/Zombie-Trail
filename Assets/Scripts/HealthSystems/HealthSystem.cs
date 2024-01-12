using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    //Declaration
    [SerializeField] protected int maxHealth;
    protected int currentHealth;

    public virtual void Awake()
    {
        // Assigning currentHealth & healthBar to the value of maxHealth
        currentHealth = maxHealth;
    }

    public virtual void AlterHealth(int amount)
    {
        currentHealth += amount;

        // Check for death
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die(){
        gameObject.SetActive(false);
    }
}
