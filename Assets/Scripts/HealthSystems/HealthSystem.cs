using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : NetworkBehaviour
{
    //Declaration
    [SerializeField] protected int maxHealth;
    protected int currentHealth;

    public virtual void Awake()
    {
        // Assigning currentHealth & healthBar to the value of maxHealth
        currentHealth = maxHealth;
    }
    public void AlterHealth(int amount) {
        AlterHealthServerRpc(amount);
    }

    [Rpc(SendTo.Server)]
    public virtual void AlterHealthServerRpc(int amount)
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
