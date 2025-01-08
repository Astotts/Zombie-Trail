using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ZombieHealthSystem : AbstractHealthSystem, IDamageable
{
    public override HealthSystemStats Stats => stats;

    [SerializeField] HealthSystemStats stats;
    [SerializeField] NetworkObject networkObject;
    [SerializeField] HealthEffect effect;

    private float currentHealth;

    // private IEnumerator healthFlashing;
    // private IEnumerator hideHealth;
    private bool IsDead => currentHealth <= 0;

    Coroutine redFlashCoroutine;
    bool isRedFlashing;

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
        PlayOnDamagedEffectClientRpc(previous, currentHealth, stats.MaxHealth);

        if (IsDead)
            Die();
    }

    public void Die()
    {
        // StopCoroutine(healthFlashing);
        // StopCoroutine(hideHealth);
        if (isRedFlashing)
            StopCoroutine(redFlashCoroutine);

        GameObject prefab = stats.Prefab;

        NetworkObjectPool.Singleton.ReturnNetworkObject(networkObject, prefab);

        networkObject.Despawn(false);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void PlayOnDamagedEffectClientRpc(float previous, float current, float max)
    {
        effect.PlayOnDamagedEffect(previous, current, max);
    }
}