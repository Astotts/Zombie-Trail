using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : NetworkBehaviour
{
    //Declaration
    [SerializeField] protected int maxHealth;
    protected NetworkVariable<int> currentHealth = new();
    public override void OnNetworkSpawn()
    {
        if (!IsHost)
            return;
        base.OnNetworkSpawn();
        currentHealth.Value = maxHealth;
        currentHealth.OnValueChanged += OnHealthChanged;
    }

    public virtual void AlterHealth(int amount)
    {
        AlterHealthRpc(amount);
    }

    [Rpc(SendTo.Server)]
    public virtual void AlterHealthRpc(int amount)
    {
        currentHealth.Value += amount;
    }

    void OnHealthChanged(int previous, int current)
    {
        if (current <= 0)
            Die();
    }

    public virtual void Die()
    {
        gameObject.SetActive(false);
    }
}
