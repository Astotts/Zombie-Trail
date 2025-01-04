using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Tools.NetStatsMonitor;
using Unity.Netcode;
using UnityEngine;

public class WalkerZombie : NetworkBehaviour, IDamageable
{
    [SerializeField] GameObject prefab;
    [SerializeField] NetworkObject networkObject;
    [SerializeField] ZombieStats stats;

    private float currentHealth;
    private bool IsDeath { get { return currentHealth <= 0; } }

    void OnValidate()
    {
        if (networkObject == null)
            networkObject = GetComponent<NetworkObject>();

        if (prefab == null)
            prefab = this.gameObject;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        currentHealth = stats.MaxHealth;
    }

    public void Damage(float amount)
    {
        currentHealth -= amount;

        if (IsDeath)
            Despawn();
    }

    void Despawn()
    {
        NetworkObjectPool.Singleton.ReturnNetworkObject(networkObject, prefab);
        networkObject.Despawn(false);
    }
}
