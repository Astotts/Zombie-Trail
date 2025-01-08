using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using static EventManager;

public class PlayerHealthSystem : AbstractHealthSystem
{
    public override HealthSystemStats Stats => stats;

    [SerializeField] HealthSystemStats stats;
    [SerializeField] NetworkObject networkObject;
    [SerializeField] PlayerHealthEffect effect;

    private float currentHealth;

    // private IEnumerator healthFlashing;
    // private IEnumerator hideHealth;
    private bool IsDead => currentHealth <= 0;

    // Server Behaviours

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            currentHealth = stats.MaxHealth;

        base.OnNetworkSpawn();
    }

    public override void Damage(float amount)
    {
        float previous = currentHealth;
        currentHealth -= amount;
        PlayOnDamagedEffectClientRpc(currentHealth, stats.MaxHealth);

        PlayerDamagedEventArgs eventArgs = new()
        {
            PlayerID = OwnerClientId,
            PreviousHealth = previous,
            CurrentHealth = currentHealth
        };

        EventManager.EventHandler.OnPlayerDamaged(eventArgs);

        if (IsDead)
            Die();
    }

    public void Die()
    {
        GameObject prefab = stats.Prefab;

        NetworkObjectPool.Singleton.ReturnNetworkObject(networkObject, prefab);

        networkObject.Despawn(false);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void PlayOnDamagedEffectClientRpc(float current, float max)
    {
        effect.PlayOnDamagedEffect(current, max);
    }

    public void LoadData(PlayerData data)
    {
        currentHealth = data.CurrentHealth;
    }

    public void SaveData(ref PlayerData data)
    {
        data.CurrentHealth = currentHealth;
    }
}
