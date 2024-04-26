using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : NetworkBehaviour
{
    //Declaration
    [SerializeField] protected int maxHealth;
    [SerializeField] protected NetworkVariable<int> currentHealth = new();

    public virtual void Start()
    {
        // Assigning currentHealth.Value & healthBar to the value of maxHealth
        currentHealth.Value = maxHealth;
    }
    public virtual void AlterHealth(int amount) {
        AlterHealthServerRpc(amount);
    }

    [Rpc(SendTo.Server)]
    public virtual void AlterHealthServerRpc(int amount)
    {
        currentHealth.Value += amount;

        // Check for death
        if (currentHealth.Value <= 0)
        {
            Die();
        }
    }

    public virtual void Die(){
        gameObject.SetActive(false);
    }
}
